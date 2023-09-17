using System.ComponentModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using api.models.api;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    internal static class MoveHelper
    {
        internal static List<int[]> GetMovesFromPiece(Board board, int[] clickedSquare, bool check)
        {
            List<int[]> moves =
                board.Rows[clickedSquare[0]].Squares[clickedSquare[1]].Piece?.GetPaths(
                    board,
                    clickedSquare,
                    check
                ) ?? new();
            return moves;
        }

        internal static bool TryMovePiece(
            int[] coords,
            int[] start,
            ref List<int[]> moves,
            ref Board board,
            out bool check
        )
        {
            check = false;

            foreach (int[] move in moves)
            {
                if (move[0] == coords[0] && move[1] == coords[1])
                {
                    check = MovePiece(start, move, ref board);
                    moves.Clear();
                    return true;
                }
            }

            return false;
        }

        private static bool MovePiece(int[] start, int[] dest, ref Board board)
        {
            BoardSquare from = board.Rows[start[0]].Squares[start[1]];
            BoardSquare to = board.Rows[dest[0]].Squares[dest[1]];

            if (from.Piece == null)
            {
                return new();
            }

            //Attempts EnPassant if valid
            CheckEnPassantCapture(from.Piece, to, start, dest, ref board);

            //Set all values to empty
            ResetBoard(ref board);

            //Check if EnPassant is available for next turn
            CheckEnPassantMove(from.Piece, start, dest, ref board);

            //Actual Piece move
            MovePieceInner(to, from, ref board);

            //Refresh board stats
            CheckTracker tracker = RefreshBoard(ref board);

            //Checks for Pins
            CheckPins(tracker, ref board);

            //Check for Checkmate
            CheckCheckmate(tracker, ref board);

            return tracker.IsInCheck();
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

        private static void ResetBoard(ref Board board)
        {
            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    square.CheckBlockingColor = "";
                    square.EnPassantColor = "";
                    square.BlackPressure = 0;
                    square.WhitePressure = 0;

                    if (square.Piece != null)
                    {
                        square.Piece.PinnedDir = Direction.None;

                        if (square.Piece is King king)
                        {
                            king.InCheck = false;
                        }
                    }
                }
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

        private static void MovePieceInner(BoardSquare to, BoardSquare from, ref Board board)
        {
            to.Piece = from.Piece;
            from.Piece = null;

            if (to.Piece is IPieceHasMoved pieceHasMoved)
            {
                pieceHasMoved.HasMoved = true;
            }

            if (Math.Abs(from.Coords[1] - to.Coords[1]) > 1 && to.Piece is King)
            {
                CastleKing(from.Coords, to.Coords, ref board);
            }
        }

        private static void CastleKing(int[] start, int[] dest, ref Board board)
        {
            int[] rookStart = { start[0], start[1] > dest[1] ? 0 : 7 };
            int[] rookDest = { start[0], start[1] > dest[1] ? 3 : 5 };
            MovePiece(rookStart, rookDest, ref board);
        }

        private static CheckTracker RefreshBoard(ref Board board)
        {
            CheckTracker tracker = new();
            BoardSquare? checkSquare = null;
            int[] checkSavingSquareLoc = new int[] { -1, -1 };

            foreach (BoardRow row in board.Rows)
            {
                foreach (BoardSquare square in row.Squares)
                {
                    AddBoardPressure(
                        square,
                        ref board,
                        ref tracker,
                        ref checkSquare,
                        ref checkSavingSquareLoc
                    );
                }
            }

            if (checkSquare != null && checkSquare.Piece is IPieceCanPin checkSavingSquaresPiece)
            {
                tracker.SetHasSavingSquares(
                    checkSavingSquaresPiece.Color == "white" ? "black" : "white",
                    checkSavingSquaresPiece.HasSavingSquares(
                        new int[] { checkSquare.Coords[0], checkSquare.Coords[1] },
                        checkSavingSquareLoc,
                        ref board
                    )
                );
            }

            return tracker;
        }

        private static void AddBoardPressure(
            BoardSquare square,
            ref Board board,
            ref CheckTracker tracker,
            ref BoardSquare? checkSquare,
            ref int[] kingLoc
        )
        {
            if (square.Piece == null)
            {
                return;
            }

            if (square.Piece is IPieceCanPin)
            {
                tracker.PinPieces.Add(square);
            }

            foreach (int[] pMove in square.Piece.GetPressure(board, square.Coords))
            {
                BoardSquare pSquare = board.Rows[pMove[0]].Squares[pMove[1]];

                if (pSquare.Piece is King king)
                {
                    if (square.Piece.Color != king.Color)
                    {
                        if (
                            square.Piece is IPieceCanPin pinPiece
                            && tracker.GetKing(pinPiece.Color) == null
                        )
                        {
                            checkSquare = square;
                            kingLoc = pMove;
                        }
                        else
                        {
                            checkSquare = null;
                            kingLoc = new int[] { -1, -1 };
                            tracker.SetHasSavingSquares(king.Color, false);
                        }

                        tracker.SetKing(pSquare);
                        king.InCheck = true;
                    }
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

        private static void CheckPins(CheckTracker tracker, ref Board board)
        {
            foreach (BoardSquare square in tracker.PinPieces)
            {
                if (square.Piece == null || square.Piece is not IPieceCanPin piece)
                {
                    continue;
                }

                int[] lStart = new int[] { square.Coords[0], square.Coords[1] };
                if (piece.Color == "white" && tracker.BlackKing != null)
                {
                    piece.CheckPins(lStart, tracker.BlackKing.Coords, ref board);
                }
                else if (tracker.WhiteKing != null)
                {
                    piece.CheckPins(lStart, tracker.WhiteKing.Coords, ref board);
                }
            }
        }

        private static void CheckCheckmate(CheckTracker tracker, ref Board board)
        {
            if (tracker.WhiteKing != null)
            {
                CheckKingInCheckMate(tracker.WhiteKing, tracker, ref board);
            }
            else if (tracker.BlackKing != null)
            {
                CheckKingInCheckMate(tracker.BlackKing, tracker, ref board);
            }
        }

        private static void CheckKingInCheckMate(
            BoardSquare square,
            CheckTracker tracker,
            ref Board board
        )
        {
            if (square.Piece == null || square.Piece is not King king || !king.InCheck)
            {
                return;
            }

            if (king.GetPaths(board, square.Coords, true).Count > 0)
            {
                return;
            }

            if (
                king.Color == "white"
                    ? tracker.HasWhiteSavingSquares
                    : tracker.HasBlackSavingSquares
            )
            {
                return;
            }

            king.InCheckMate = true;
        }
    }
}
