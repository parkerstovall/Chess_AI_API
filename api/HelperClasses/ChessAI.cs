using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using api.models.api;
using api.models.db;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    public class ChessAI
    {
        private readonly int Max_Depth;
        private readonly string? CheckColor;
        private readonly string max_color = "black";
        private readonly string min_color = "white";

        public ChessAI(string? CheckColor = null, bool isBlack = true, int Max_Depth = 3)
        {
            this.Max_Depth = Max_Depth;
            this.CheckColor = CheckColor;
            if (!isBlack)
            {
                max_color = "white";
                min_color = "black";
            }
        }

        public Board GetMove(Board board, out Move? foundMove, out string? checkColor)
        {
            int score = int.MinValue;
            int[] move = new int[4];

            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece != null && square.Piece.Color == max_color)
                    {
                        foreach (
                            int[] localMove in MoveHelper.GetMovesFromPiece(
                                board,
                                square.Coords,
                                CheckColor
                            )
                        )
                        {
                            Board newBoard = CopyBoard(board);

                            MoveHelper.MovePiece(
                                new int[] { square.Coords[0], square.Coords[1] },
                                new int[] { localMove[0], localMove[1] },
                                ref newBoard
                            );

                            int tempScore = MinMax(
                                newBoard,
                                false,
                                CheckColor,
                                0,
                                int.MinValue,
                                int.MaxValue
                            );

                            if (tempScore > score)
                            {
                                score = tempScore;
                                move =
                                [
                                    square.Coords[0],
                                    square.Coords[1],
                                    localMove[0],
                                    localMove[1]
                                ];
                            }
                        }
                    }
                }
            }

            checkColor = MoveHelper.MovePiece([move[0], move[1]], [move[2], move[3]], ref board);

            foundMove = null;
            IPiece? piece = board.Rows[move[2]].Squares[move[3]].Piece;
            if (piece != null)
            {
                foundMove = new()
                {
                    From = [move[0], move[1]],
                    To = [move[2], move[3]],
                    PieceColor = piece.Color,
                    PieceType = piece.GetType().Name
                };
            }

            return board;
        }

        private int MinMax(
            Board board,
            bool max,
            string? checkedColor,
            int depth,
            double alpha,
            double beta
        )
        {
            string color = max ? max_color : min_color;

            if (depth == Max_Depth)
            {
                return GetBoardScore(board, color);
            }

            List<BoardSquare> moveSquares = new();

            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    if (square.Piece != null && square.Piece.Color == color)
                    {
                        if (square.Piece is King king && king.InCheck)
                        {
                            checkedColor = color;

                            if (king.InCheckMate)
                            {
                                return color == max_color ? int.MinValue : int.MaxValue;
                            }
                        }

                        moveSquares.Add(square);
                    }
                }
            }

            List<int[]> moves = new();
            foreach (BoardSquare square in moveSquares)
            {
                if (square.Piece == null)
                {
                    continue;
                }

                List<int[]> localMoves = MoveHelper.GetMovesFromPiece(
                    board,
                    square.Coords,
                    CheckColor
                );

                foreach (int[] localMove in localMoves)
                {
                    moves.Add(
                        new int[] { square.Coords[0], square.Coords[1], localMove[0], localMove[1] }
                    );
                }
            }

            int score = max ? int.MinValue : int.MaxValue;

            if (!moves.Any())
            {
                return 0;
            }

            foreach (int[] move in moves)
            {
                Board newBoard = CopyBoard(board);

                MoveHelper.MovePiece(
                    new int[] { move[0], move[1] },
                    new int[] { move[2], move[3] },
                    ref newBoard
                );

                int tempScore = MinMax(newBoard, !max, checkedColor, depth + 1, alpha, beta);

                if ((tempScore > score && max) || (tempScore < score && !max))
                {
                    score = tempScore;
                }

                if (max)
                {
                    alpha = Math.Max(score, alpha);
                }
                else
                {
                    beta = Math.Min(score, beta);
                }

                if (beta <= alpha)
                {
                    break;
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
                    if (piece != null)
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

        public Board CopyBoard(Board board)
        {
            Board newBoard = new();
            int i = 0;
            foreach (BoardRow row in board.Rows)
            {
                newBoard.Rows.Add(new());
                foreach (BoardSquare square in row.Squares)
                {
                    newBoard.Rows[i].Squares.Add(square.Copy());
                }
                i++;
            }

            return newBoard;
        }
    }
}
