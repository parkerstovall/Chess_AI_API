using api.pieces;

namespace api.models.api
{
    public class BoardSquare
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public string BackColor { get; set; } = "";
        public string CssClass { get; set; } = "";

        //public Piece piece = null;
        public int[]? enPassantVictim;
        public string enPassantColor = "";
        public string blockCheckColor = "";
        public string blackPressureSource = "";
        public string whitePressureSource = "";
        public string pinDir = "";
        public int blackPressure;
        public int whitePressure;
        public bool isSelected = false;
        public bool isHighlighted = false;
        public bool hasMoved = false;
        public bool moved = false;
        public bool inCheck = false;
        public bool checkmate = false;
    }
}
