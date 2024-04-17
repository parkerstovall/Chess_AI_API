using api.models.api;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses.chess
{
    public class CheckTracker
    {
        public BoardSquare? WhiteKing { get; set; } = null;
        public BoardSquare? BlackKing { get; set; } = null;
        public List<BoardSquare> PinPieces { get; set; } = [];
        public bool HasWhiteSavingSquares { get; set; } = false;
        public bool HasBlackSavingSquares { get; set; } = false;
        public List<BoardSquare> Attackers { get; set; } = [];

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

        public List<BoardSquare> GetKingAttackers(string color)
        {
            return Attackers.Where((a) => a.Piece?.Color != color).ToList();
        }

        public void AddAttacker(BoardSquare attacker)
        {
            Attackers.Add(attacker);
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

        public string? CheckColor()
        {
            if (IsKingInCheck("white"))
            {
                return "white";
            }
            else if (IsKingInCheck("black"))
            {
                return "black";
            }

            return null;
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
