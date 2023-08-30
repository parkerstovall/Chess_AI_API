using api.models.api;
using Microsoft.AspNetCore.Routing.Matching;

namespace api.pieces
{
    public class King : IPiece
    {
        public bool HasMoved { get; set; } = false;
        public string Color { get; set; }
        public int[] Coords { get; set; }

        public King(string Color, int[] Coords)
        {
            this.Color = Color;
            this.Coords = Coords;
        }

        public List<int[]> GetPaths(Board board, bool check)
        {
            List<int[]> moves = new();

            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

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
                    if (
                        (square.Piece == null || square.Piece.Color != this.Color)
                        && (
                            (
                                this.Color == "white"
                                && board.Rows[col].Squares[row].BlackPressure == 0
                            )
                            || (
                                this.Color == "black"
                                && board.Rows[col].Squares[row].WhitePressure == 0
                            )
                        )
                    )
                    {
                        moves.Add(new int[] { col, row });
                    }
                }
            }

            CheckCastle(board, ref moves);

            return moves;
        }

        public List<int[]> GetPressure(Board board)
        {
            List<int[]> moves = new();
            int col = Coords[0];
            int row = Coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

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
                return Color + "|King";
            }

            return Color + "King";
        }

        private void CheckCastle(Board board, ref List<int[]> moves)
        {
            if (this.HasMoved)
            {
                return;
            }

            int col = Coords[0];
            int row = Coords[1];

            if (CheckCastleDir(board, true, col, row))
            {
                moves.Add(new int[] { col, row - 2 });
            }

            if (CheckCastleDir(board, false, col, row))
            {
                moves.Add(new int[] { col, row + 2 });
            }
        }

        private bool CheckCastleDir(Board board, bool left, int col, int row)
        {
            int offset = left ? -1 : 1;

            if (
                board.Rows[col].Squares[row + (1 * offset)].Piece == null
                && board.Rows[col].Squares[row + (2 * offset)].Piece == null
                && board.Rows[col].Squares[row + (3 * offset)].Piece == null
                && board.Rows[col].Squares[row + (4 * offset)].Piece is Rook r
                && r.Color == this.Color
                && !r.HasMoved
            )
            {
                if (
                    this.Color == "white"
                    && board.Rows[col].Squares[row].BlackPressure == 0
                    && board.Rows[col].Squares[row + (1 * offset)].BlackPressure == 0
                    && board.Rows[col].Squares[row + (2 * offset)].BlackPressure == 0
                    && board.Rows[col].Squares[row + (3 * offset)].BlackPressure == 0
                )
                {
                    return true;
                }
                else if (
                    this.Color == "black"
                    && board.Rows[col].Squares[row].WhitePressure == 0
                    && board.Rows[col].Squares[row + (1 * offset)].WhitePressure == 0
                    && board.Rows[col].Squares[row + (2 * offset)].WhitePressure == 0
                    && board.Rows[col].Squares[row + (3 * offset)].WhitePressure == 0
                )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
