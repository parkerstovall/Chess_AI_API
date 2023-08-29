using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingWeb.ChessFiles
{
    public class Rook : Piece
    {
        public Rook(string color, int[] coords)
        {
            this.color = color;
            this.type = "Rook";
            this.coords = coords;
        }

        public override List<int[]> GetPaths(BoardSquare[,] board, bool check)
        {
            List<int[]> moves = new List<int[]>();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1 };
            int[] rowInc = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1))
                {
                    if (board[col, row].piece == null)
                    {
                        if (check)
                        {
                            if (board[col, row].blockCheckColor == this.color)
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                        else
                        {
                            moves.Add(new int[] { col, row });
                        }
                    }
                    else if (board[col, row].piece.color != this.color)
                    {
                        if (check)
                        {
                            if (board[col, row].blockCheckColor == this.color)
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                        else
                        {
                            moves.Add(new int[] { col, row });
                        }
                        break;
                    }
                    else
                    {
                        break;
                    }

                    col += colInc[i];
                    row += rowInc[i];
                }
            }

            return moves;
        }

        public override List<int[]> GetPressure(BoardSquare[,] board)
        {
            List<int[]> moves = new List<int[]>();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1 };
            int[] rowInc = { 1, -1, 0, 0 };
            string[] dir = { "lr", "lr", "tb", "tb" };

            for (int i = 0; i < 4; i++, col = coords[0], row = coords[1])
            {
                if (board[col, row].pinDir != "" && board[col, row].pinDir != dir[i])
                {
                    continue;
                }

                col += colInc[i];
                row += rowInc[i];

                while (col >= 0 && row >= 0 && col < 8 && row < 8)
                {
                    moves.Add(new int[] { col, row });

                    if (board[col, row].piece != null)
                    {
                        if (board[col, row].piece.type == "King")
                        {
                            col += colInc[i];
                            row += rowInc[i];
                            if (col >= 0 && row >= 0 && col < 8 && row < 8)
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }

                        break;
                    }

                    col += colInc[i];
                    row += rowInc[i];
                }
            }

            return moves;
        }

        public override string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return color + "|Rook";
            }

            return color + "Rook";
        }
    }
}
