using ChessApi.Models.API;
using MongoDB.Bson;

namespace ChessApi.Models.DB
{
    public class PreviousAlpha
    {
        public required ObjectId GameID { get; set; }
        public int? Alpha { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
