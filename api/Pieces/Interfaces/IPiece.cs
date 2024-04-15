using api.helperclasses.chess;
using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPiece
    {
        public string Color { get; set; }
        public Direction PinnedDir { get; set; }
        public int[,] WhiteValues { get; }
        public int[,] BlackValues { get; }
        public int Value { get; }

        public List<int[]> GetPaths(Board board, int[] coords, bool check);
        public List<int[]> GetPressure(Board board, int[] coords);
        public IPiece Copy();
    }
}
