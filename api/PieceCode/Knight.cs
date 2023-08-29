using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.pieces
{
    public class Knight : Piece
    {
        public Knight(string color, int[] coords)
        {
            this.color = color;
            this.type = "Knight";
            this.coords = coords;
        }

        public override List<int[]> GetPaths(BoardSquare[,] board, bool check)
        {
            List<int[]> moves = new List<int[]>();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            if (board[col, row].pinDir != "")
            {
                return moves;
            }

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1))
                {
                    if (board[col, row].piece == null || board[col, row].piece.color != this.color)
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
                }
            }
            return moves;
        }

        public override List<int[]> GetPressure(BoardSquare[,] board)
        {
            List<int[]> moves = new List<int[]>();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (col >= 0 && row >= 0 && col < board.GetLength(0) && row < board.GetLength(1))
                {
                    moves.Add(new int[] { col, row });
                }
            }
            return moves;
        }

        public override string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return color + "|Knight";
            }

            return color + "Knight";
        }
    }
}
