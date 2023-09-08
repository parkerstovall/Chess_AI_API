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
    }
}
