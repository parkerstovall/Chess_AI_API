using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Pawn : IPiece, IPieceHasMoved
    {
        public bool HasMoved { get; set; } = false;
        public string Color { get; set; }

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

            if (col > -1 && col < board.Rows.Count)
            {
                //Moving
                if (
                    board.Rows[col].Squares[row].Piece == null
                    && (
                        board.Rows[coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[coords[0]].Squares[row].PinnedDirection
                            == Direction.FromTopToBottom
                    )
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
                    if (
                        board.Rows[coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[coords[0]].Squares[row].PinnedDirection
                            == Direction.FromTopLeftToBottomRight
                    )
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
                    if (
                        board.Rows[coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[coords[0]].Squares[row].PinnedDirection
                            == Direction.FromTopLeftToBottomRight
                    )
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
                    moves.Add(board.Rows[i].Squares[j].coords);
                }

                j += 2;

                if (j < board.Rows[i].Squares.Count)
                {
                    moves.Add(board.Rows[i].Squares[j].coords);
                }
            }

            return moves;
        }

        public override string ToString()
        {
            return Color + "Pawn";
        }
    }
}
