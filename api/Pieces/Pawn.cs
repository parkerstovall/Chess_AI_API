using api.helperclasses;
using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Pawn : IPieceHasMoved
    {
        public bool HasMoved { get; set; } = false;
        public string Color { get; set; }
        public Direction PinnedDir { get; set; } = Direction.None;

        public int[,] WhiteValues { get; } =
            new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, },
                { 50, 50, 50, 50, 50, 50, 50, 50 },
                { 10, 10, 20, 30, 30, 20, 10, 10 },
                { 5, 5, 10, 25, 25, 10, 5, 5 },
                { 0, 0, 0, 20, 20, 0, 0, 0 },
                { 5, -5, -10, 0, 0, -10, -5, 5 },
                { 5, 10, 10, -20, -20, 10, 10, 5 },
                { 0, 0, 0, 0, 0, 0, 0, 0 }
            };

        public int[,] BlackValues { get; } =
            new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 5, 10, 10, -20, -20, 10, 10, 5 },
                { 5, -5, -10, 0, 0, -10, -5, 5 },
                { 0, 0, 0, 20, 20, 0, 0, 0 },
                { 5, 5, 10, 25, 25, 10, 5, 5 },
                { 10, 10, 20, 30, 30, 20, 10, 10 },
                { 50, 50, 50, 50, 50, 50, 50, 50 },
                { 0, 0, 0, 0, 0, 0, 0, 0, },
            };

        public int Value { get; } = 100;

        public Pawn(string Color)
        {
            this.Color = Color;
        }

        public List<int[]> GetPaths(Board board, int[] coords, bool check)
        {
            List<int[]> moves = new();
            int dir = (this.Color == "black") ? 1 : -1;

            int col = coords[0] + dir;
            int row = coords[1];

            bool canMove = (
                PinnedDir == Direction.None
                || PinnedDir == Direction.FromTopToBottom
                || PinnedDir == Direction.FromBottomToTop
            );

            bool canAttackLeft = (
                PinnedDir == Direction.None
                || PinnedDir == Direction.FromBottomRightToTopLeft
                || PinnedDir == Direction.FromTopLeftToBottomRight
            );

            bool canAttackRight = (
                PinnedDir == Direction.None
                || PinnedDir == Direction.FromBottomLeftToTopRight
                || PinnedDir == Direction.FromTopRightToBottomLeft
            );

            if (col > -1 && col < board.Rows.Count)
            {
                //Moving
                if (board.Rows[col].Squares[row].Piece == null && (canMove))
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

                    col += dir;

                    if (
                        col > -1
                        && col < board.Rows.Count
                        && !this.HasMoved
                        && board.Rows[col].Squares[row].Piece == null
                    )
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
                }

                col = coords[0] + dir;
                row -= 1;

                //Attacking
                if (row > -1)
                {
                    if (canAttackLeft)
                    {
                        BoardSquare left = board.Rows[col].Squares[row];

                        if (
                            (left.Piece != null && left.Piece.Color != this.Color)
                            || (left.EnPassantColor != "" && left.EnPassantColor != this.Color)
                        )
                        {
                            if (check)
                            {
                                if (left.CheckBlockingColor == this.Color)
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

                row += 2;

                if (row < board.Rows[col].Squares.Count)
                {
                    if (canAttackRight)
                    {
                        BoardSquare right = board.Rows[col].Squares[row];

                        if (
                            (right.Piece != null && right.Piece.Color != this.Color)
                            || (right.EnPassantColor != "" && right.EnPassantColor != this.Color)
                        )
                        {
                            if (check)
                            {
                                if (right.CheckBlockingColor == this.Color)
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
            }

            return moves;
        }

        public List<int[]> GetPressure(Board board, int[] coords)
        {
            List<int[]> moves = new();
            int dir = (this.Color == "black") ? 1 : -1;

            int i = coords[0] + dir;
            int j = coords[1] - 1;

            if (i > -1 && i < board.Rows.Count)
            {
                //Attacking
                if (j > -1)
                {
                    moves.Add(board.Rows[i].Squares[j].Coords);
                }

                j += 2;

                if (j < board.Rows[i].Squares.Count)
                {
                    moves.Add(board.Rows[i].Squares[j].Coords);
                }
            }

            return moves;
        }

        public IPiece Copy()
        {
            Pawn newPiece =
                new(this.Color) { HasMoved = this.HasMoved, PinnedDir = this.PinnedDir };
            return newPiece;
        }

        public override string ToString()
        {
            return Color + "Pawn";
        }
    }
}
