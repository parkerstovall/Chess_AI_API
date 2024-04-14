using api.models.api;

namespace api.models.db
{
    public class Move
    {
        public int OldColumn { get; set; }
        public int NewColumn { get; set; }
        public int OldRow { get; set; }
        public int NewRow { get; set; }
        public string? PieceType { get; set; }
        public string? PieceColor { get; set; }
    }
}
