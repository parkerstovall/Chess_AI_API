using api.models.api;
using api.pieces;
using api.pieces.interfaces;

namespace api.helperclasses
{
    public class CheckTracker
    {
        public King? BlackKing { get; set; }
        public King? WhiteKing { get; set; }
        public int[] WhiteKingCoords { get; set; } = new int[2];
        public int[] BlackKingCoords { get; set; } = new int[2];
        public List<IPiece> BlackCheckers { get; set; } = new List<IPiece>();
        public List<IPiece> WhiteCheckers { get; set; } = new List<IPiece>();
        public List<BoardSquare> PinPieces { get; set; } = new List<BoardSquare>();

        public void SetKing(King king, int[] loc)
        {
            if (king.Color == "black")
            {
                BlackKing = king;
                BlackKingCoords = loc;
            }
            else
            {
                WhiteKing = king;
                WhiteKingCoords = loc;
            }
        }

        public void AddChecker(IPiece checker)
        {
            if (checker.Color == "black")
            {
                BlackCheckers.Add(checker);
            }
            else
            {
                WhiteCheckers.Add(checker);
            }
        }
    }
}
