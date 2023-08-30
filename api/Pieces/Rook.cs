using api.models.api;

namespace api.pieces
{
    public class Rook : IPiece
    {
        public string Color { get; set; }
        public int[] Coords { get; set; }
        public bool HasMoved { get; set; } = false;

        public Rook(string Color, int[] Coords)
        {
            this.Color = Color;
            this.Coords = Coords;
        }

        public List<int[]> GetPaths(Board board, bool check)
        {
            List<int[]> moves = new();

            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { 0, 0, 1, -1 };
            int[] rowInc = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++, col = Coords[0], row = Coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (
                    col >= 0
                    && row >= 0
                    && col < board.Rows.Count
                    && row < board.Rows[col].Squares.Count
                )
                {
                    if (board.Rows[col].Squares[row].Piece == null)
                    {
                        if (check)
                        {
                            if (board.Rows[col].Squares[row].CheckBlockingColor == this.Color)
                            {
                                moves.Add(new int[] { col, row });
                            }
                        }
                        else
                        {
                            moves.Add(new int[] { col, row });
                        }
                    }
                    else if (board.Rows[col].Squares[row].Piece?.Color != this.Color)
                    {
                        if (check)
                        {
                            if (board.Rows[col].Squares[row].CheckBlockingColor == this.Color)
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

        public List<int[]> GetPressure(Board board)
        {
            List<int[]> moves = new();

            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { 0, 0, 1, -1 };
            int[] rowInc = { 1, -1, 0, 0 };

            Vector[] dir =
            {
                Vector.FromLeftToRight,
                Vector.FromRightToLeft,
                Vector.FromTopToBottom,
                Vector.FromBottomToTop
            };

            for (int i = 0; i < 4; i++, col = Coords[0], row = Coords[1])
            {
                if (
                    board.Rows[col].Squares[row].PinnedDirection != Vector.None
                    && board.Rows[col].Squares[row].PinnedDirection != dir[i]
                )
                {
                    continue;
                }

                col += colInc[i];
                row += rowInc[i];

                while (col >= 0 && row >= 0 && col < 8 && row < 8)
                {
                    moves.Add(new int[] { col, row });

                    if (board.Rows[col].Squares[row].Piece != null)
                    {
                        if (board.Rows[col].Squares[row].Piece is King)
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

        public string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return Color + "|Rook";
            }

            return Color + "Rook";
        }
    }
}
