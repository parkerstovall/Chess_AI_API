using api.pieces.interfaces;

namespace api.models.api
{
    public class BoardSquare
    {
        public IPiece? Piece = null;
        public int[] coords { get; set; } = { -1, -1 };
        public string CheckBlockingColor { get; set; } = "";
        public Direction PinnedDirection = Direction.None;
        public int BlackPressure { get; set; }
        public int WhitePressure { get; set; }
        public string EnPassantColor = "";
        /*
        
        public string blackPressureSource = "";
        public string whitePressureSource = "";
        public bool inCheck = false;
        public bool checkmate = false;
        
        */
    }
}
