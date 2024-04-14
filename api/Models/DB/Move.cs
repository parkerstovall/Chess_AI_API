using api.models.api;

namespace api.models.db
{
    public class Move
    {
        public required int[] From { get; set; }
        public required int[] To { get; set; }
        public required string PieceType { get; set; }
        public required string PieceColor { get; set; }
    }
}
