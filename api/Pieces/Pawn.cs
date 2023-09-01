using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Pawn : IPiece, IPieceHasMoved
    {
        public bool HasMoved { get; set; } = false;
        public string Color { get; set; }
        public int[] Coords { get; set; }

        public Pawn(string Color, int[] Coords)
        {
            this.Color = Color;
            this.Coords = Coords;
        }

        public List<int[]> GetPaths(Board board, bool check)
        {
            List<int[]> moves = new();
            int dir = (this.Color == "black") ? 1 : -1;

            int col = Coords[0] + dir;
            int row = Coords[1];

            if (col > -1 && col < board.Rows.Count)
            {
                //Moving
                if (
                    board.Rows[col].Squares[row].Piece == null
                    && (
                        board.Rows[Coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[Coords[0]].Squares[row].PinnedDirection
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

                col = Coords[0] + dir;
                row -= 1;

                //Attacking
                if (row > -1)
                {
                    if (
                        board.Rows[Coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[Coords[0]].Squares[row].PinnedDirection
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
                        board.Rows[Coords[0]].Squares[row].PinnedDirection == Direction.None
                        || board.Rows[Coords[0]].Squares[row].PinnedDirection
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

        public List<int[]> GetPressure(Board board)
        {
            List<int[]> moves = new();
            int dir = (this.Color == "black") ? 1 : -1;

            int i = Coords[0] + dir;
            int j = Coords[1] - 1;

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

        public string ToString(bool pipeSeparated)
        {
            if (pipeSeparated)
            {
                return Color + "|Pawn";
            }

            return Color + "Pawn";
        }
    }
}
