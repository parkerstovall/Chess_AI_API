using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ChessApi.Models.API;
using ChessApi.Models.DB;
using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChessApi.HelperClasses.Chess
{
    public class ChessAI
    {
        internal class ThreadResources
        {
            internal int alpha { get; set; } = int.MinValue;
            internal PossibleMove move { get; set; } = new();
        }

        private readonly int Max_Depth;
        private readonly string max_color = "black";
        private readonly string min_color = "white";

        public ChessAI(bool isBlack = true, int Max_Depth = 4)
        {
            this.Max_Depth = Max_Depth;
            if (!isBlack)
            {
                max_color = "white";
                min_color = "black";
            }
        }

        public Move GetMove(Game game)
        {
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

            OrderPossibleMoves(possibleMoves);

            // https://www.chessprogramming.org/Young_Brothers_Wait_Concept
            // We run the first move to get a baseline score and hopefully
            // speed up the rest of the moves when they are run in parralel
            // (Alpha Beta Pruning uses the initial score to compare moves)
            threadResources.move = possibleMoves[0];
            possibleMoves.RemoveAt(0);

            Game firstMove = CopyGame(game);

            MoveHelper.MovePiece(
                [threadResources.move.MoveFrom[0], threadResources.move.MoveFrom[1]],
                [threadResources.move.MoveTo[0], threadResources.move.MoveTo[1]],
                ref firstMove
            );

            threadResources.alpha = MinMax(firstMove, false, 0, int.MinValue, int.MaxValue);

            Parallel.ForEach(
                possibleMoves,
                pMove =>
                {
                    var newGame = CopyGame(game);

                    MoveHelper.MovePiece(
                        [pMove.MoveFrom[0], pMove.MoveFrom[1]],
                        [pMove.MoveTo[0], pMove.MoveTo[1]],
                        ref newGame
                    );

                    int tempScore = MinMax(newGame, false, 0, threadResources.alpha, int.MaxValue);
                    if (tempScore > threadResources.alpha)
                    {
                        lock (threadResources)
                        {
                            threadResources.alpha = tempScore;
                            threadResources.move = pMove;
                        }
                    }
                }
            );

            Move foundMove =
                new()
                {
                    GameID = game.GameID,
                    From = [threadResources.move.MoveFrom[0], threadResources.move.MoveFrom[1]],
                    To = [threadResources.move.MoveTo[0], threadResources.move.MoveTo[1]],
                    PieceColor = "",
                    PieceType = ""
                };

            IPiece? piece = game.Board
                .Rows[threadResources.move.MoveTo[0]]
                .Squares[threadResources.move.MoveTo[1]]
                .Piece;
            if (piece is not null)
            {
                foundMove.PieceColor = piece.Color;
                foundMove.PieceType = piece.GetType().Name;
            }

            return foundMove;
        }

        private int MinMax(Game game, bool max, int depth, double alpha, double beta)
        {
            string color = max ? max_color : min_color;
            if (depth == Max_Depth)
            {
                return GetBoardScore(game.Board);
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

                MoveHelper.MovePiece(
                    [pMove.MoveFrom[0], pMove.MoveFrom[1]],
                    [pMove.MoveTo[0], pMove.MoveTo[1]],
                    ref newGame
                );

                int tempScore = MinMax(newGame, !max, depth + 1, alpha, beta);

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
                possibleMoves.Where(p => p.CaptureValue is not null).OrderBy(p => p.CaptureValue)
            );

            orderedMoves.AddRange(
                possibleMoves
                    .Where(p => p.CaptureValue is null)
                    .OrderBy(p =>
                    {
                        // Pieces that have already moved should be prioritized
                        if (p.HasMoved)
                        {
                            return p.PieceValue + 1;
                        }

                        return p.PieceValue;
                    })
            );

            return orderedMoves;
        }
    }
}
