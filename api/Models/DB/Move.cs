using api.models.api;
using MongoDB.Bson;

namespace api.models.db
{
    public class Move
    {
        public ObjectId GameID { get; set; }
        public DateTime CreateDate { get; set; }
        public required int[] From { get; set; }
        public required int[] To { get; set; }
        public required string PieceType { get; set; }
        public required string PieceColor { get; set; }
    }
}
