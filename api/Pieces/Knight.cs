using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.models.api;

namespace api.pieces
{
    public class Knight : IPiece
    {
        public string Color { get; set; }
        public string Type { get; set; }
        public int[] Coords { get; set; }

        public Knight(string Color, int[] Coords)
        {
            this.Color = Color;
            this.Type = "Knight";
            this.Coords = Coords;
        }

        public List<int[]> GetPaths(Board board, bool check)
        {
            List<int[]> moves = new();
            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            if (board.Rows[col].Squares[row].PinnedDirection != Vector.None)
            {
                return moves;
            }

            for (int i = 0; i < 8; i++, col = Coords[0], row = Coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (
                    col >= 0
                    && row >= 0
                    && col < board.Rows.Count
                    && row < board.Rows[col].Squares.Count
                )
                {
                    BoardSquare square = board.Rows[col].Squares[row];
                    if (square.Piece == null || square.Piece.Color != this.Color)
                    {
                        if (check)
                        {
                            if (square.CheckBlockingColor == this.Color)
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

        public List<int[]> GetPressure(Board board)
        {
            List<int[]> moves = new();
            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            for (int i = 0; i < 8; i++, col = Coords[0], row = Coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (
                    col >= 0
                    && row >= 0
                    && col < board.Rows.Count
                    && row < board.Rows[col].Squares.Count
                )
                {
                    moves.Add(new int[] { col, row });
                }
            }
            return moves;
        }

        public string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return Color + "|Knight";
            }

            return Color + "Knight";
        }
    }
}
