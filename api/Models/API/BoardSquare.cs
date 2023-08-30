using api.pieces;

namespace api.models.api
{
    public class BoardSquare
    {
        public IPiece? Piece = null;
        public int[] Coords { get; set; } = { -1, -1 };
        public string BackColor { get; set; } = "";
        public string CssClass { get; set; } = "";
        public string CheckBlockingColor { get; set; } = "";
        public Vector PinnedDirection = Vector.None;
        public int BlackPressure { get; set; }
        public int WhitePressure { get; set; }
        public string EnPassantColor = "";
        /*
        public int[]? enPassantVictim;
        
        public string blackPressureSource = "";
        public string whitePressureSource = "";
        public string pinDir = "";
        
        public bool isSelected = false;
        public bool isHighlighted = false;
        public bool hasMoved = false;
        public bool moved = false;
        public bool inCheck = false;
        public bool checkmate = false;
        */
    }
}
