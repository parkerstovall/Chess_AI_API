using api.pieces.interfaces;

namespace api.models.api
{
    public class BoardSquare
    {
        public IPiece? Piece = null;
        public int[] Coords { get; set; } = { -1, -1 };
        public string CheckBlockingColor { get; set; } = "";
        public int BlackPressure { get; set; }
        public int WhitePressure { get; set; }
        public string EnPassantColor = "";
    }
}
