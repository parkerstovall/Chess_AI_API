using ChessApi.Pieces.Interfaces;

namespace ChessApi.Models.API
{
    public class BoardSquare
    {
        public IPiece? Piece = null;
        public int[] Coords { get; set; } = { -1, -1 };
        public string CheckBlockingColor { get; set; } = "";
        public int BlackPressure { get; set; }
        public int WhitePressure { get; set; }
        public bool WhiteKingPressure { get; set; }
        public bool BlackKingPressure { get; set; }
        public string EnPassantColor = "";

        public BoardSquare Copy()
        {
            BoardSquare copy =
                new()
                {
                    Piece = Piece?.Copy(),
                    Coords = new int[] { Coords[0], Coords[1] },
                    CheckBlockingColor = CheckBlockingColor,
                    BlackPressure = BlackPressure,
                    WhitePressure = WhitePressure,
                    EnPassantColor = EnPassantColor
                };
            return copy;
        }
    }
}
