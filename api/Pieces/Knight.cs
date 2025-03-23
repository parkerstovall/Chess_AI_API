using ChessApi.HelperClasses.Chess;
using ChessApi.Models.API;
using ChessApi.Pieces.Interfaces;

namespace ChessApi.Pieces
{
    public class Knight(byte Color) : IPieceDirectAttacker
    {
        public byte Color { get; set; } = Color;
        public Direction PinnedDir { get; set; } = Direction.None;
        public int[,] WhiteValues { get; } =
            new int[,]
            {
                { -50, -40, -30, -30, -30, -30, -40, -50 },
                { -40, -20, 0, 0, 0, 0, -20, -40 },
                { -30, 0, 10, 15, 15, 10, 0, -30 },
                { -30, 5, 15, 20, 20, 15, 5, -30 },
                { -30, 0, 15, 20, 20, 15, 0, -30 },
                { -30, 5, 10, 15, 15, 10, 5, -30 },
                { -40, -20, 0, 5, 5, 0, -20, -40 },
                { -50, -40, -30, -30, -30, -30, -40, -50 }
            };

        public int[,] BlackValues { get; } =
            new int[,]
            {
                { -50, -40, -30, -30, -30, -30, -40, -50 },
                { -40, -20, 0, 5, 5, 0, -20, -40 },
                { -30, 0, 10, 15, 15, 10, 0, -30 },
                { -30, 5, 15, 20, 20, 15, 5, -30 },
                { -30, 0, 15, 20, 20, 15, 0, -30 },
                { -30, 5, 10, 15, 15, 10, 5, -30 },
                { -40, -20, 0, 0, 0, 0, -20, -40 },
                { -50, -40, -30, -30, -30, -30, -40, -50 }
            };

        public int Value { get; } = 300;

        public List<PossibleMove> GetPaths(Board board, int[] coords, bool check)
        {
            List<PossibleMove> moves = [];
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
                    if (
                        (!check || square.CheckBlockingColor == this.Color)
                        && (square.Piece is null || square.Piece?.Color != this.Color)
                    )
                    {
                        //moves.Add(new int[] { col, row });
                        moves.Add(
                            new()
                            {
                                MoveTo = [col, row],
                                MoveFrom = [coords[0], coords[1]],
                                MovingPiece = this,
                                CapturedPiece = square.Piece
                            }
                        );
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

        public IPiece Copy()
        {
            Knight newPiece = new(this.Color) { PinnedDir = this.PinnedDir };
            return newPiece;
        }

        public string GetHashKey()
        {
            return $"n{Color}";
        }

        public override string ToString()
        {
            return (Color == 0 ? "white" : "black") + "Knight";
        }
    }
}
