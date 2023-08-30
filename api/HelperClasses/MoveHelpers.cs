using api.models.api;

namespace api.helperclasses
{
    internal static class MoveHelpers
    {
        internal static List<int[]> GetMovesFromPiece(Board board, int row, int col)
        {
            List<int[]> moves = board.Rows[row].Squares[col].Piece?.GetPaths(board, false) ?? new();
            return moves;
        }

        internal static bool TryMovePiece(
            int row,
            int col,
            int[] start,
            ref List<int[]> moves,
            ref Board board
        )
        {
            foreach (int[] move in moves)
            {
                if (move[0] == row && move[1] == col)
                {
                    MovePiece(start, move, ref board);
                    moves.Clear();
                    return true;
                }
            }

            return false;
        }

        internal static void MovePiece(int[] start, int[] dest, ref Board board)
        {
            board.Rows[dest[0]].Squares[dest[1]].Piece = board.Rows[start[0]].Squares[
                start[1]
            ].Piece;
            board.Rows[start[0]].Squares[start[1]].Piece = null;
        }
    }
}
