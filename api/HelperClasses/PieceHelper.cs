using api.models.api;
using api.models.db;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    internal static class PieceHelper
    {
        internal static bool IsInBoard(int col, int row)
        {
            return col > -1 && col < 8 && row > -1 && row < 8;
        }

        internal static Direction GetDirection(int[] start, int[] dest)
        {
            if (start[0] == dest[0])
            {
                if (start[1] < dest[1])
                {
                    return Direction.FromLeftToRight;
                }
                else
                {
                    return Direction.FromRightToLeft;
                }
            }

            if (start[1] == dest[1])
            {
                if (start[0] < dest[0])
                {
                    return Direction.FromTopToBottom;
                }
                else
                {
                    return Direction.FromBottomToTop;
                }
            }

            int colDiff = start[0] - dest[0];
            int rowDiff = start[1] - dest[1];

            if (colDiff == rowDiff)
            {
                if (colDiff > 0)
                {
                    return Direction.FromBottomRightToTopLeft;
                }
                else
                {
                    return Direction.FromTopLeftToBottomRight;
                }
            }

            if (Math.Abs(colDiff) == Math.Abs(rowDiff))
            {
                if (colDiff > 0)
                {
                    return Direction.FromBottomLeftToTopRight;
                }
                else
                {
                    return Direction.FromTopRightToBottomLeft;
                }
            }

            return Direction.None;
        }

        internal static Tuple<int[], int[]> GetIncrements(
            Direction PinnedDir,
            bool diag,
            bool straight
        )
        {
            int[] colInc = Array.Empty<int>();
            int[] rowInc = Array.Empty<int>();

            if (
                diag && PinnedDir == Direction.FromTopLeftToBottomRight
                || PinnedDir == Direction.FromBottomRightToTopLeft
            )
            {
                colInc = new int[] { 1, -1 };
                rowInc = new int[] { 1, -1 };
            }
            else if (
                diag && PinnedDir == Direction.FromBottomLeftToTopRight
                || PinnedDir == Direction.FromTopRightToBottomLeft
            )
            {
                colInc = new int[] { -1, 1 };
                rowInc = new int[] { 1, -1 };
            }
            else if (
                straight && PinnedDir == Direction.FromTopToBottom
                || PinnedDir == Direction.FromBottomToTop
            )
            {
                colInc = new int[] { -1, 1 };
                rowInc = new int[] { 0, 0 };
            }
            else if (
                straight && PinnedDir == Direction.FromLeftToRight
                || PinnedDir == Direction.FromRightToLeft
            )
            {
                colInc = new int[] { 0, 0 };
                rowInc = new int[] { -1, 1 };
            }
            else if (PinnedDir == Direction.None)
            {
                if (diag && straight)
                {
                    colInc = new int[] { -1, -1, 1, 1, -1, 1, 0, 0 };
                    rowInc = new int[] { 1, -1, -1, 1, 0, 0, -1, 1 };
                }
                else if (diag)
                {
                    colInc = new int[] { -1, -1, 1, 1 };
                    rowInc = new int[] { 1, -1, -1, 1 };
                }
                else if (straight)
                {
                    colInc = new int[] { -1, 1, 0, 0 };
                    rowInc = new int[] { 0, 0, -1, 1 };
                }
            }

            return Tuple.Create(colInc, rowInc);
        }

        internal static int[] GetSingleIncrement(Direction dir)
        {
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
                case Direction.FromLeftToRight:
                    inc[0] = 0;
                    inc[1] = 1;
                    break;
                case Direction.FromRightToLeft:
                    inc[0] = 0;
                    inc[1] = -1;
                    break;
                case Direction.FromTopToBottom:
                    inc[0] = 1;
                    inc[1] = 0;
                    break;
                case Direction.FromBottomToTop:
                    inc[0] = -1;
                    inc[1] = 0;
                    break;
            }

            return inc;
        }

        public static void SetPins(int[] start, int[] inc, Direction dir, ref Game game)
        {
            start[0] += inc[0];
            start[1] += inc[1];
            List<IPiece> pieces = new();
            while (IsInBoard(start[0], start[1]))
            {
                BoardSquare square = game.Board.Rows[start[0]].Squares[start[1]];
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

        public static bool SetSavingSquares(int[] start, int[] inc, string color, ref Game game)
        {
            bool canSave = false;

            while (IsInBoard(start[0], start[1]))
            {
                BoardSquare square = game.Board.Rows[start[0]].Squares[start[1]];

                if (square.Piece != null && square.Piece is King)
                {
                    break;
                }

                int pawnInc = color == "white" ? -1 : 1;

                if (IsInBoard(start[0] + pawnInc, start[1]))
                {
                    BoardSquare pawnSquare = game.Board.Rows[start[0] + pawnInc].Squares[start[1]];
                    if (
                        pawnSquare.Piece != null
                        && pawnSquare.Piece is Pawn pawn
                        && pawn.Color != color
                    )
                    {
                        canSave = true;
                        square.CheckBlockingColor = color == "white" ? "black" : "white";
                    }
                }

                if (IsInBoard(start[0] + (2 * pawnInc), start[1]))
                {
                    BoardSquare pawnSquare = game.Board.Rows[start[0] + (2 * pawnInc)].Squares[
                        start[1]
                    ];
                    if (
                        pawnSquare.Piece != null
                        && pawnSquare.Piece is Pawn pawn
                        && !pawn.HasMoved
                        && pawn.Color != color
                    )
                    {
                        canSave = true;
                        square.CheckBlockingColor = color == "white" ? "black" : "white";
                    }
                }

                if (GetEnemyPressure(color, square) > 1)
                {
                    canSave = true;
                    square.CheckBlockingColor = color == "white" ? "black" : "white";
                }
                else if (GetEnemyPressure(color, square) == 1 && !GetKingPressure(color, square))
                {
                    canSave = true;
                    square.CheckBlockingColor = color == "white" ? "black" : "white";
                }

                start[0] += inc[0];
                start[1] += inc[1];
            }

            return canSave;
        }

        private static int GetEnemyPressure(string color, BoardSquare square)
        {
            if (color == "white")
            {
                return square.BlackPressure;
            }
            else
            {
                return square.WhitePressure;
            }
        }

        private static bool GetKingPressure(string color, BoardSquare square)
        {
            if (color == "white")
            {
                return square.BlackKingPressure;
            }
            else
            {
                return square.WhiteKingPressure;
            }
        }
    }
}
