using api.models.api;

namespace api.pieces
{
    public class Bishop : IPiece
    {
        public string Color { get; set; }
        public int[] Coords { get; set; }

        public Bishop(string Color, int[] Coords)
        {
            this.Color = Color;
            this.Coords = Coords;
        }

        public List<int[]> GetPaths(Board board, bool check)
        {
            List<int[]> moves = new();

            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { -1, -1, 1, 1 };
            int[] rowInc = { 1, -1, -1, 1 };
            Vector[] vectors =
            {
                Vector.FromBottomLeftToTopRight,
                Vector.FromTopLeftToBottomRight,
                Vector.FromBottomLeftToTopRight,
                Vector.FromTopLeftToBottomRight,
            };

            for (int i = 0; i < 4; i++, col = Coords[0], row = Coords[1])
            {
                if (
                    board.Rows[col].Squares[row].PinnedDirection != Vector.None
                    && board.Rows[col].Squares[row].PinnedDirection != vectors[i]
                )
                {
                    continue;
                }

                col += colInc[i];
                row += rowInc[i];

                while (
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

                        if (square.Piece != null)
                        {
                            break;
                        }
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
            int[] colInc = { -1, -1, 1, 1 };
            int[] rowInc = { 1, -1, -1, 1 };

            for (int i = 0; i < 4; i++, col = Coords[0], row = Coords[1])
            {
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
                return Color + "|Bishop";
            }

            return Color + "Bishop";
        }
    }
}
