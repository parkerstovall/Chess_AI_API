using System.Collections;
using System.Timers;
using ChessApi.Models.API;
using ChessApi.Models.DB;
using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;

namespace ChessApi.HelperClasses.Chess
{
    public class ChessAI
    {
        private int Max_Depth;
        private int Max_Time = 10000;
        private bool StopThinking = false;
        private System.Timers.Timer timer = new System.Timers.Timer();

        //private int totalMoves = 0;
        private readonly string max_color = "black";
        private readonly string min_color = "white";
        private readonly Hashtable Transpositions = new();
        private readonly long[,,] PositionHases;
        private readonly Dictionary<string, int> pieceHashes =
            new(StringComparer.CurrentCultureIgnoreCase)
            {
                { "br", 0 },
                { "bn", 1 },
                { "bb", 2 },
                { "bq", 3 },
                { "bk", 4 },
                { "bp", 5 },
                { "wr", 6 },
                { "wn", 7 },
                { "wb", 8 },
                { "wq", 9 },
                { "wk", 10 },
                { "wp", 11 },
            };

        public ChessAI(bool isBlack = true)
        {
            PositionHases = GeneratePositionHashes();
            if (!isBlack)
            {
                max_color = "white";
                min_color = "black";
            }

            timer.Interval = Max_Time;
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Elapsed += (o, e) => StopThinking = true;
        }

        public Move GetMove(Game game)
        {
            timer.Start();
            var possibleMoves = new List<PossibleMove>();

            foreach (BoardRow row in game.Board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece is not null && square.Piece?.Color == max_color)
                    {
                        var moves = MoveHelper.GetMovesFromPiece(
                            game.Board,
                            square.Coords,
                            game.CheckedColor
                        );

                        possibleMoves.AddRange(moves);
                    }
                }
            }

            if (possibleMoves.Count < 10)
            {
                Max_Depth = 5;
            }
            else if (possibleMoves.Count < 5)
            {
                Max_Depth = 6;
            }
            else
            {
                Max_Depth = 4;
            }

            OrderPossibleMoves(possibleMoves);

            int alpha = int.MinValue;
            PossibleMove? move = null;
            var boardHash = GenerateBoardHash(game.Board);

            foreach (var pMove in possibleMoves)
            {
                var newGame = CopyGame(game);
                var tempBoardHash = MovePiece(boardHash, pMove, ref newGame);
                int tempScore = MinMax(newGame, false, 0, alpha, int.MaxValue, tempBoardHash);

                if (tempScore > alpha)
                {
                    alpha = tempScore;
                    move = pMove;
                }
            }

            if (move is null)
            {
                throw new Exception("No suitable moves found.");
            }

            Move foundMove =
                new()
                {
                    GameID = game.GameID,
                    From = [move.MoveFrom[0], move.MoveFrom[1]],
                    To = [move.MoveTo[0], move.MoveTo[1]],
                    PieceColor = move.MovingPiece.Color,
                    PieceType = move.MovingPiece.GetType().Name
                };
            // Console.WriteLine($"Total Moves: {totalMoves}");
            // Console.WriteLine($"Unique Moves: {Transpositions.Count}");
            return foundMove;
        }

        public Move GetMoveParallel(Game game)
        {
            timer.Start();
            var possibleMoves = new List<PossibleMove>();
            var threadResources = new ThreadResources();

            foreach (BoardRow row in game.Board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece is not null && square.Piece?.Color == max_color)
                    {
                        var moves = MoveHelper.GetMovesFromPiece(
                            game.Board,
                            square.Coords,
                            game.CheckedColor
                        );

                        possibleMoves.AddRange(moves);
                    }
                }
            }

            if (possibleMoves.Count < 10)
            {
                Max_Depth = 6;
            }
            else if (possibleMoves.Count < 5)
            {
                Max_Depth = 7;
            }
            else
            {
                Max_Depth = 5;
            }

            OrderPossibleMoves(possibleMoves);

            // https://www.chessprogramming.org/Young_Brothers_Wait_Concept
            // We run the first move to get a baseline score and hopefully
            // speed up the rest of the moves when they are run in parralel
            // (Alpha Beta Pruning uses the initial score to compare moves)
            var boardHash = GenerateBoardHash(game.Board);
            var firstMove = possibleMoves[0];
            threadResources.Move = firstMove;
            possibleMoves.RemoveAt(0);

            Game firstGame = CopyGame(game);
            var firstTempBoardHash = MovePiece(boardHash, firstMove, ref firstGame);

            threadResources.Alpha = MinMax(
                firstGame,
                false,
                0,
                int.MinValue,
                int.MaxValue,
                firstTempBoardHash
            );

            Parallel.ForEach(
                possibleMoves,
                pMove =>
                {
                    var newGame = CopyGame(game);
                    var tempBoardHash = MovePiece(boardHash, pMove, ref newGame);

                    int tempScore = MinMax(
                        newGame,
                        false,
                        0,
                        threadResources.Alpha,
                        int.MaxValue,
                        tempBoardHash
                    );
                    if (tempScore > threadResources.Alpha)
                    {
                        lock (threadResources)
                        {
                            threadResources.Alpha = tempScore;
                            threadResources.Move = pMove;
                        }
                    }
                }
            );

            Move foundMove =
                new()
                {
                    GameID = game.GameID,
                    From = [threadResources.Move.MoveFrom[0], threadResources.Move.MoveFrom[1]],
                    To = [threadResources.Move.MoveTo[0], threadResources.Move.MoveTo[1]],
                    PieceColor = "",
                    PieceType = ""
                };

            IPiece? piece = game.Board
                .Rows[threadResources.Move.MoveTo[0]]
                .Squares[threadResources.Move.MoveTo[1]]
                .Piece;
            if (piece is not null)
            {
                foundMove.PieceColor = piece.Color;
                foundMove.PieceType = piece.GetType().Name;
            }

            return foundMove;
        }

        private int MinMax(Game game, bool max, int depth, int alpha, int beta, long boardHash)
        {
            //totalMoves++;
            string color = max ? max_color : min_color;
            if (
                Transpositions.ContainsKey(boardHash) && Transpositions[boardHash] is int boardScore
            )
            {
                //matches++;
                return boardScore;
            }
            else if (depth == Max_Depth || StopThinking)
            {
                boardScore = GetBoardScore(game.Board);

                lock (Transpositions)
                {
                    if (!Transpositions.ContainsKey(boardHash))
                    {
                        Transpositions.Add(boardHash, boardScore);
                    }
                }

                return boardScore;
            }

            List<BoardSquare> moveSquares = new();

            foreach (BoardRow row in game.Board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece is not null && square.Piece?.Color == color)
                    {
                        if (square.Piece is King king && king.InCheckMate)
                        {
                            return color == max_color ? int.MinValue : int.MaxValue;
                        }

                        moveSquares.Add(square);
                    }
                }
            }

            List<PossibleMove> possibleMoves = [];
            foreach (BoardSquare square in moveSquares)
            {
                if (square.Piece is null)
                {
                    continue;
                }

                List<PossibleMove> localMoves = MoveHelper.GetMovesFromPiece(
                    game.Board,
                    square.Coords,
                    game.CheckedColor
                );

                possibleMoves.AddRange(localMoves);
            }

            if (possibleMoves.Count == 0)
            {
                return 0;
            }

            OrderPossibleMoves(possibleMoves);

            int score = max ? int.MinValue : int.MaxValue;

            foreach (var pMove in possibleMoves)
            {
                Game newGame = CopyGame(game);
                var tempBoardHash = MovePiece(boardHash, pMove, ref newGame);
                int tempScore = MinMax(newGame, !max, depth + 1, alpha, beta, tempBoardHash);

                if ((tempScore > score && max) || (tempScore < score && !max))
                {
                    score = tempScore;
                }

                if (max)
                {
                    alpha = Math.Max(score, alpha);
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
                else
                {
                    beta = Math.Min(score, beta);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            lock (Transpositions)
            {
                if (!Transpositions.ContainsKey(boardHash))
                {
                    Transpositions.Add(boardHash, score);
                }
            }

            return score;
        }

        public int GetBoardScore(Board board)
        {
            int boardScore = 0;

            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    IPiece? piece = square.Piece;
                    if (piece is not null)
                    {
                        int[,] modifier =
                            piece.Color == "white" ? piece.WhiteValues : piece.BlackValues;
                        int pieceValue = piece.Value + modifier[square.Coords[0], square.Coords[1]];

                        if (piece.Color == max_color)
                        {
                            boardScore += pieceValue;
                        }
                        else
                        {
                            boardScore -= pieceValue;
                        }
                    }
                }
            }

            return boardScore;
        }

        public Game CopyGame(Game game)
        {
            Game newGame = new() { Board = new(), CheckedColor = game.CheckedColor };

            int i = 0;
            foreach (BoardRow row in game.Board.Rows)
            {
                newGame.Board.Rows.Add(new());
                foreach (BoardSquare square in row.Squares)
                {
                    newGame.Board.Rows[i].Squares.Add(square.Copy());
                }
                i++;
            }

            return newGame;
        }

        public List<PossibleMove> OrderPossibleMoves(List<PossibleMove> possibleMoves)
        {
            var orderedMoves = new List<PossibleMove>();

            orderedMoves.AddRange(
                possibleMoves
                    .Where(p => p.CapturedPiece is not null)
                    .OrderBy(p => p.CapturedPiece?.Value)
            );

            orderedMoves.AddRange(
                possibleMoves
                    .Where(p => p.CapturedPiece is null)
                    .OrderBy(p =>
                    {
                        // Pieces that have already moved should be prioritized
                        if (p.HasMoved)
                        {
                            return p.MovingPiece.Value + 1;
                        }

                        return p.MovingPiece.Value;
                    })
            );

            return orderedMoves;
        }

        private long[,,] GeneratePositionHashes()
        {
            long[,,] hashes = new long[8, 8, 12];
            Random rand = new();
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    foreach (var item in pieceHashes)
                    {
                        long value;

                        do
                        {
                            value = NextLong(rand);
                        } while (!ValidateHashPosition(hashes, value));

                        hashes[i, j, item.Value] = value;
                    }
                }
            }

            return hashes;
        }

        private long GenerateBoardHash(Board board)
        {
            long boardHash = 0;
            for (var i = 0; i < board.Rows.Count; i++)
            {
                var squareLen = board.Rows[i].Squares.Count;
                for (var j = 0; j < squareLen; j++)
                {
                    boardHash = AlterBoardHash(i, j, board, boardHash);
                }
            }

            return boardHash;
        }

        private long AlterBoardHash(int col, int row, Board board, long boardHash)
        {
            var piece = board.Rows[col].Squares[row].Piece;
            if (piece is null)
            {
                return boardHash;
            }

            var key = $"{piece.Color[0]}{piece.HashName}";
            var pieceHash = pieceHashes[key];
            return boardHash ^= PositionHases[col, row, pieceHash];
        }

        private long AlterBoardHash(int col, int row, IPiece piece, long boardHash)
        {
            var key = $"{piece.Color[0]}{piece.HashName}";
            var pieceHash = pieceHashes[key];
            return boardHash ^= PositionHases[col, row, pieceHash];
        }

        private long MovePiece(long boardHash, PossibleMove pMove, ref Game newGame)
        {
            // Remove the piece from its position in the hash
            long tempBoardHash = AlterBoardHash(
                pMove.MoveFrom[0],
                pMove.MoveFrom[1],
                pMove.MovingPiece,
                boardHash
            );

            // Move the piece on the actual board
            MoveHelper.MovePiece(
                [pMove.MoveFrom[0], pMove.MoveFrom[1]],
                [pMove.MoveTo[0], pMove.MoveTo[1]],
                ref newGame
            );

            // Add the piece to its new position in the hash
            tempBoardHash = AlterBoardHash(
                pMove.MoveTo[0],
                pMove.MoveTo[1],
                pMove.MovingPiece,
                tempBoardHash
            );

            // Account for pawn promotions
            if (pMove.MovingPiece is Pawn p)
            {
                if (pMove.MoveTo[0] == 0 && p.Color == "white")
                {
                    var pieceHash = pieceHashes["wq"];
                    tempBoardHash ^= PositionHases[pMove.MoveTo[0], pMove.MoveTo[1], pieceHash];
                }
                else if (pMove.MoveTo[0] == 7 && p.Color == "black")
                {
                    var pieceHash = pieceHashes["bq"];
                    tempBoardHash ^= PositionHases[pMove.MoveTo[0], pMove.MoveTo[1], pieceHash];
                }
            }

            // Remove the captured piece from the hash, if it exists.
            if (pMove.CapturedPiece is not null)
            {
                // Override in case of castling or en passant
                if (
                    pMove.CapturedMoveFromOverride is not null
                    || pMove.CapturedMoveToOverride is not null
                )
                {
                    if (pMove.CapturedMoveFromOverride is not null)
                    {
                        tempBoardHash = AlterBoardHash(
                            pMove.CapturedMoveFromOverride[0],
                            pMove.CapturedMoveFromOverride[1],
                            pMove.CapturedPiece,
                            tempBoardHash
                        );
                    }

                    if (pMove.CapturedMoveToOverride is not null)
                    {
                        tempBoardHash = AlterBoardHash(
                            pMove.CapturedMoveToOverride[0],
                            pMove.CapturedMoveToOverride[1],
                            pMove.CapturedPiece,
                            tempBoardHash
                        );
                    }
                }
                else
                {
                    // Normal capture
                    tempBoardHash = AlterBoardHash(
                        pMove.MoveTo[0],
                        pMove.MoveTo[1],
                        pMove.CapturedPiece,
                        tempBoardHash
                    );
                }
            }

            return tempBoardHash;
        }

        private static bool ValidateHashPosition(long[,,] hashes, long hashValue)
        {
            for (var i = 0; i < hashes.GetLength(0); i++)
            {
                for (var j = 0; j < hashes.GetLength(1); j++)
                {
                    for (var k = 0; k < hashes.GetLength(2); k++)
                    {
                        if (hashes[i, j, k] == hashValue)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static long NextLong(Random random)
        {
            byte[] longBuf = new byte[sizeof(long)];
            long longValue;

            do
            {
                random.NextBytes(longBuf);
                longValue = BitConverter.ToInt64(longBuf) & long.MaxValue;
            } while (longValue == long.MaxValue);

            return longValue;
        }
    }
}
