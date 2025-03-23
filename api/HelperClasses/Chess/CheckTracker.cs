using ChessApi.Models.API;
using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;

namespace ChessApi.HelperClasses.Chess
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
            if (square.Piece is not null && square.Piece is King king)
            {
                if (king.Color == 1)
                {
                    BlackKing = square;
                }
                else
                {
                    WhiteKing = square;
                }
            }
        }

        public BoardSquare? GetKing(byte color)
        {
            return color == 0 ? WhiteKing : BlackKing;
        }

        public List<BoardSquare> GetKingAttackers(byte color)
        {
            return [.. Attackers.Where((a) => a.Piece?.Color != color)];
        }

        public void AddAttacker(BoardSquare attacker)
        {
            Attackers.Add(attacker);
        }

        public void SetHasSavingSquares(byte color, bool hasSavingSquares)
        {
            if (color == 0)
            {
                HasWhiteSavingSquares = hasSavingSquares;
            }
            else
            {
                HasBlackSavingSquares = hasSavingSquares;
            }
        }

        public byte? CheckColor()
        {
            if (IsKingInCheck(0))
            {
                return 0;
            }
            else if (IsKingInCheck(1))
            {
                return 1;
            }

            return null;
        }

        private bool IsKingInCheck(byte color)
        {
            BoardSquare? square = GetKing(color);

            if (square is null || square.Piece is null || square.Piece is not King king)
            {
                return false;
            }

            return king.InCheck;
        }
    }
}
