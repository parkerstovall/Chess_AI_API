using ChessApi.Models.DB;

namespace ChessApi.Models.API
{
    public class PossibleMove
    {
        public int[] MoveTo { get; set; } = [-1, -1];
        public int[] MoveFrom { get; set; } = [-1, -1];
        public bool HasMoved
        {
            get
            {
                return (this.PieceValue == 100 && (MoveTo[0] == 1 || MoveTo[0] == 6))
                    || (MoveTo[0] == 0 || MoveTo[0] == 7);
            }
        }
        public int PieceValue { get; set; } = -1;
        public int? CaptureValue { get; set; }
    }
}
