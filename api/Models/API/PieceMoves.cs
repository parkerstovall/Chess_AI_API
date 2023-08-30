using api.pieces;

namespace api.models.api
{
    public class PieceMove
    {
        public int[]? From { get; set; }
        public int[]? To { get; set; }
        public IPiece? Piece { get; set; }
        public IPiece? CapturedPiece { get; set; }
    }
}
