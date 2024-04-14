using api.helperclasses;
using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class King : IPieceHasMoved
    {
        public string Color { get; set; }
        public bool HasMoved { get; set; } = false;
        public bool InCheck { get; set; } = false;
        public bool InCheckMate { get; set; } = false;
        public Direction PinnedDir { get; set; } = Direction.None;

        public int[,] WhiteValues { get; } =
            new int[,]
            {
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -20, -30, -30, -40, -40, -30, -30, -20 },
                { -10, -20, -20, -20, -20, -20, -20, -10 },
                { 20, 20, 0, 0, 0, 0, 20, 20 },
                { 20, 30, 10, 0, 0, 10, 30, 20 }
            };

        public int[,] BlackValues { get; } =
            new int[,]
            {
                { 20, 30, 10, 0, 0, 10, 30, 20 },
                { 20, 20, 0, 0, 0, 0, 20, 20 },
                { -10, -20, -20, -20, -20, -20, -20, -10 },
                { -20, -30, -30, -40, -40, -30, -30, -20 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 },
                { -30, -40, -40, -50, -50, -40, -40, -30 }
            };
        public int Value { get; } = 20000;

        public King(string Color)
        {
            this.Color = Color;
        }

        public List<int[]> GetPaths(Board board, int[] coords, bool check)
        {
            List<int[]> moves = new();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

            for (int i = 0; i < 8; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                if (PieceHelper.IsInBoard(col, row))
                {
                    BoardSquare square = board.Rows[col].Squares[row];
                    if (
                        (square.Piece == null || square.Piece.Color != this.Color)
                        && SafeSquare(square)
                    )
                    {
                        moves.Add(new int[] { col, row });
                    }
                }
            }

            CheckCastle(board, coords, ref moves);

            return moves;
        }

        public List<int[]> GetPressure(Board board, int[] coords)
        {
            List<int[]> moves = new();
            int col = coords[0];
            int row = coords[1];
            int[] colInc = { 0, 0, 1, -1, 1, -1, 1, -1 };
            int[] rowInc = { 1, -1, 0, -0, 1, -1, -1, 1 };

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
            return Color + "King";
        }

        private void CheckCastle(Board board, int[] coords, ref List<int[]> moves)
        {
            if (this.HasMoved || this.InCheck)
            {
                return;
            }

            int col = coords[0];
            int row = coords[1];

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
            int len = left ? 4 : 3;
            bool add = true;
            for (int i = 1; i < len; i++)
            {
                BoardSquare square = board.Rows[col].Squares[row + (i * offset)];

                if (i + 1 == len)
                {
                    if (square.Piece is IPieceHasMoved hm && hm.HasMoved)
                    {
                        add = false;
                        break;
                    }
                }
                if (square.Piece != null || !SafeSquare(square))
                {
                    add = false;
                    break;
                }
            }

            return add;
        }

        private bool SafeSquare(BoardSquare square)
        {
            if (this.Color == "white")
            {
                return square.BlackPressure == 0;
            }
            else
            {
                return square.WhitePressure == 0;
            }
        }

        public IPiece Copy()
        {
            King newPiece =
                new(this.Color)
                {
                    PinnedDir = this.PinnedDir,
                    InCheck = this.InCheck,
                    InCheckMate = this.InCheckMate,
                    HasMoved = this.HasMoved
                };
            return newPiece;
        }
    }
}
