using api.models.api;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    public class CheckTracker
    {
        public BoardSquare? WhiteKing { get; set; } = null;
        public BoardSquare? BlackKing { get; set; } = null;
        public List<BoardSquare> PinPieces { get; set; } = new List<BoardSquare>();
        public bool HasWhiteSavingSquares { get; set; } = false;
        public bool HasBlackSavingSquares { get; set; } = false;

        public void SetKing(BoardSquare square)
        {
            if (square.Piece != null && square.Piece is King king)
            {
                if (king.Color == "black")
                {
                    BlackKing = square;
                }
                else
                {
                    WhiteKing = square;
                }
            }
        }

        public BoardSquare? GetKing(string color)
        {
            return color == "white" ? WhiteKing : BlackKing;
        }

        public void SetHasSavingSquares(string color, bool hasSavingSquares)
        {
            if (color == "white")
            {
                HasWhiteSavingSquares = hasSavingSquares;
            }
            else
            {
                HasBlackSavingSquares = hasSavingSquares;
            }
        }

        public bool IsInCheck()
        {
            return IsKingInCheck("white") || IsKingInCheck("black");
        }

        private bool IsKingInCheck(string color)
        {
            BoardSquare? square = GetKing(color);

            if (square == null || square.Piece == null || square.Piece is not King king)
            {
                return false;
            }

            return king.InCheck;
        }
    }
}
