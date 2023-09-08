using api.helperclasses;
using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Knight : IPiece
    {
        public string Color { get; set; }
        public string Type { get; set; }
        public Direction PinnedDir { get; set; } = Direction.None;

        public Knight(string Color)
        {
            this.Color = Color;
            this.Type = "Knight";
        }

        public List<int[]> GetPaths(Board board, int[] coords, bool check)
        {
            List<int[]> moves = new();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            if (PinnedDir != Direction.None)
            {
                return moves;
            }

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (PieceHelper.IsInBoard(col, row))
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

        public List<int[]> GetPressure(Board board, int[] coords)
        {
            List<int[]> moves = new();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -2, -2, 2, 2, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 1, -1, 2, 2, -2, -2 };

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (PieceHelper.IsInBoard(col, row))
                {
                    moves.Add(new int[] { col, row });
                }
            }
            return moves;
        }

        public override string ToString()
        {
            return Color + "Knight";
        }
    }
}
