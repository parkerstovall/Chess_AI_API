using api.models.api;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.models.db
{
    public class Game
    {
        [BsonId]
        public ObjectId GameID { get; set; }
        public required Board Board { get; set; }
        public bool IsPlayerWhite { get; set; }
        public bool IsWhiteTurn { get; set; } = true;
        public bool IsComplete { get; set; } = false;
        public List<Move> MoveHistory { get; set; } = new();
        public List<int[]> AvailableMoves { get; set; } = new();
        public int[]? SelectedSquare { get; set; } = null;
        public string? CheckedColor { get; set; } = null;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}
