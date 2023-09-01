using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPiece
    {
        public string Color { get; set; }
        public int[] Coords { get; set; }

        public string ToString(bool pipeSeparated);
        public List<int[]> GetPaths(Board board, bool check);
        public List<int[]> GetPressure(Board board);
    }
}
