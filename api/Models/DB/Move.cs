using ChessApi.Models.API;
using MongoDB.Bson;

namespace ChessApi.Models.DB
{
    public class Move
    {
        public required ObjectId GameID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public required int[] From { get; set; }
        public required int[] To { get; set; }
        public required string PieceType { get; set; }
        public required byte PieceColor { get; set; }
    }
}
