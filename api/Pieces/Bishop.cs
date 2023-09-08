using api.helperclasses;
using api.models.api;
using api.pieces.interfaces;

namespace api.pieces
{
    public class Bishop : IPieceCanPin
    {
        public string Color { get; set; }
        public Direction PinnedDir { get; set; } = Direction.None;

        public Bishop(string Color)
        {
            this.Color = Color;
        }

        public List<int[]> GetPaths(Board board, int[] coords, bool check)
        {
            List<int[]> moves = new();

            int col = coords[0];
            int row = coords[1];

            Tuple<int[], int[]> increments = PieceHelper.GetIncrements(
                PinnedDir,
                diag: true,
                straight: false
            );
            int[] colInc = increments.Item1;
            int[] rowInc = increments.Item2;

            for (int i = 0; i < colInc.Length; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (PieceHelper.IsInBoard(col, row))
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

        public List<int[]> GetPressure(Board board, int[] coords)
        {
            List<int[]> moves = new();

            int col = coords[0];
            int row = coords[1];
            int[] colInc = { -1, -1, 1, 1 };
            int[] rowInc = { 1, -1, -1, 1 };

            for (int i = 0; i < 4; i++, col = coords[0], row = coords[1])
            {
                col += colInc[i];
                row += rowInc[i];

                while (PieceHelper.IsInBoard(col, row))
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

        public bool CanPin(Board board, int[] start, int[] dest)
        {
            Direction dir = PieceHelper.GetDirection(start, dest);

            return dir == Direction.FromTopLeftToBottomRight
                || dir == Direction.FromBottomRightToTopLeft
                || dir == Direction.FromTopRightToBottomLeft
                || dir == Direction.FromBottomLeftToTopRight;
        }

        public void CheckSavingSquares(int[] start, int[] dest, ref Board board)
        {
            Direction dir = PieceHelper.GetDirection(start, dest);
            int[] inc = new int[2];

            switch (dir)
            {
                case Direction.FromTopLeftToBottomRight:
                    inc[0] = 1;
                    inc[1] = 1;
                    break;
                case Direction.FromTopRightToBottomLeft:
                    inc[0] = 1;
                    inc[1] = -1;
                    break;
                case Direction.FromBottomRightToTopLeft:
                    inc[0] = -1;
                    inc[1] = -1;
                    break;
                case Direction.FromBottomLeftToTopRight:
                    inc[0] = -1;
                    inc[1] = 1;
                    break;
                default:
                    return;
            }

            start[0] += inc[0];
            start[1] += inc[1];
            List<IPiece> pieces = new();
            while (PieceHelper.IsInBoard(start[0], start[1]))
            {
                BoardSquare square = board.Rows[start[0]].Squares[start[1]];
                if (square.Piece != null)
                {
                    if (square.Piece is King)
                    {
                        break;
                    }

                    pieces.Add(square.Piece);

                    if (pieces.Count > 1)
                    {
                        break;
                    }
                }

                start[0] += inc[0];
                start[1] += inc[1];
            }

            if (pieces.Count == 1)
            {
                pieces[0].PinnedDir = dir;
            }
        }

        public override string ToString()
        {
            return Color + "Bishop";
        }
    }
}
