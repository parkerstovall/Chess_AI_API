using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using ChessApi.Models.API;
using ChessApi.Models.DB;
using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;

namespace ChessApi.HelperClasses.Chess
{
    public class ChessAI
    {
        private readonly int Max_Depth;
        private readonly string max_color = "black";
        private readonly string min_color = "white";

        public ChessAI(bool isBlack = true, int Max_Depth = 3)
        {
            this.Max_Depth = Max_Depth;
            if (!isBlack)
            {
                max_color = "white";
                min_color = "black";
            }
        }

        public void GetMove(ref Game game, out Move? foundMove)
        {
            int score = int.MinValue;
            int[] move = new int[4];

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

            OrderPossibleMoves(possibleMoves);

            foreach (var pMove in possibleMoves)
            {
                Game newGame = CopyGame(game);

                MoveHelper.MovePiece(
                    [pMove.MoveFrom[0], pMove.MoveFrom[1]],
                    [pMove.MoveTo[0], pMove.MoveTo[1]],
                    ref newGame
                );

                int tempScore = MinMax(newGame, false, 0, int.MinValue, int.MaxValue);

                if (tempScore > score)
                {
                    score = tempScore;
                    move = [pMove.MoveFrom[0], pMove.MoveFrom[1], pMove.MoveTo[0], pMove.MoveTo[1]];
                }
            }

            MoveHelper.MovePiece([move[0], move[1]], [move[2], move[3]], ref game);

            foundMove = null;
            IPiece? piece = game.Board.Rows[move[2]].Squares[move[3]].Piece;
            if (piece is not null)
            {
                foundMove = new()
                {
                    GameID = game.GameID,
                    From = [move[0], move[1]],
                    To = [move[2], move[3]],
                    PieceColor = piece.Color,
                    PieceType = piece.GetType().Name
                };
            }
        }

        private int MinMax(Game game, bool max, int depth, double alpha, double beta)
        {
            string color = max ? max_color : min_color;
            if (depth == Max_Depth)
            {
                return GetBoardScore(game.Board, color);
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

        public int GetBoardScore(Board board, string color)
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

                        if (piece.Color == color)
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
