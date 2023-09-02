using api.models.api;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    internal static class MoveHelper
    {
        internal static List<int[]> GetMovesFromPiece(Board board, int[] clickedSquare)
        {
            List<int[]> moves =
                board.Rows[clickedSquare[0]].Squares[clickedSquare[1]].Piece?.GetPaths(board, clickedSquare, false)
                ?? new();
            return moves;
        }

        internal static bool TryMovePiece(
            int[] coords,
            int[] start,
            ref List<int[]> moves,
            ref Board board
        )
        {
            foreach (int[] move in moves)
            {
                if (move[0] == coords[0] && move[1] == coords[1])
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
            BoardSquare from = board.Rows[start[0]].Squares[start[1]];
            BoardSquare to = board.Rows[dest[0]].Squares[dest[1]];

            if (from.Piece == null)
            {
                return;
            }

            CheckEnPassantCapture(from.Piece, to, start, dest, ref board);
            ResetBoard(ref board);
            CheckEnPassantMove(from.Piece, start, dest, ref board);

            to.Piece = from.Piece;
            from.Piece = null;

            if (to.Piece is IPieceHasMoved pieceHasMoved)
            {
                pieceHasMoved.HasMoved = true;
            }

            if (Math.Abs(start[1] - dest[1]) > 1 && to.Piece is King)
            {
                CastleKing(start, dest, ref board);
            }

            RefreshBoard(ref board);
        }

        private static void CastleKing(int[] start, int[] dest, ref Board board)
        {
            int[] rookStart = { start[0], start[1] > dest[1] ? 0 : 7 };
            int[] rookDest = { start[0], start[1] > dest[1] ? 3 : 5 };
            MovePiece(rookStart, rookDest, ref board);
        }

        private static void CheckEnPassantCapture(
            IPiece from,
            BoardSquare to,
            int[] start,
            int[] dest,
            ref Board board
        )
        {
            //Checking for en passant capture
            if (
                from is Pawn pawn
                && to.Piece == null
                && to.EnPassantColor != ""
                && to.EnPassantColor != pawn.Color
            )
            {
                int[] enPassantSquare = { dest[0], dest[1] };
                enPassantSquare[0] += (start[0] > dest[0]) ? 1 : -1;
                board.Rows[enPassantSquare[0]].Squares[enPassantSquare[1]].Piece = null;
            }
        }

        private static void CheckEnPassantMove(
            IPiece piece,
            int[] start,
            int[] dest,
            ref Board board
        )
        {
            //Checking for en passant opportunity
            if (Math.Abs(start[0] - dest[0]) == 2 && piece is Pawn pawn)
            {
                int[] enPassantSquare = { start[0], start[1] };
                enPassantSquare[0] += (start[0] > dest[0]) ? -1 : 1;
                board.Rows[enPassantSquare[0]].Squares[enPassantSquare[1]].EnPassantColor =
                    pawn.Color;
            }
        }

        private static void ResetBoard(ref Board board)
        {
            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    square.CheckBlockingColor = "";
                    square.EnPassantColor = "";
                    square.PinnedDirection = Direction.None;
                    square.BlackPressure = 0;
                    square.WhitePressure = 0;

                    if (square.Piece is King king)
                    {
                        king.InCheck = false;
                    }
                }
            }
        }

        private static void RefreshBoard(ref Board board)
        {
            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    AddBoardPressure(square, ref board);
                }
            }
        }

        private static void AddBoardPressure(BoardSquare square, ref Board board)
        {
            if (square.Piece == null)
            {
                return;
            }

            foreach (int[] pMove in square.Piece.GetPressure(board, square.coords))
            {
                BoardSquare pSquare = board.Rows[pMove[0]].Squares[pMove[1]];

                if (pSquare.Piece is King king && square.Piece.Color != king.Color)
                {
                    king.InCheck = true;
                }

                if (square.Piece.Color == "white")
                {
                    pSquare.WhitePressure++;
                }
                else
                {
                    pSquare.BlackPressure++;
                }
            }
        }
    }
}
