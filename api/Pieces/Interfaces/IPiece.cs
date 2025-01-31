using ChessApi.HelperClasses.Chess;
using ChessApi.Models.API;

namespace ChessApi.Pieces.Interfaces
{
    public interface IPiece
    {
        public string Color { get; set; }
        public Direction PinnedDir { get; set; }
        public int[,] WhiteValues { get; }
        public int[,] BlackValues { get; }
        public int Value { get; }

        public List<PossibleMove> GetPaths(Board board, int[] coords, bool check);
        public List<int[]> GetPressure(Board board, int[] coords);
        public IPiece Copy();
    }
}
