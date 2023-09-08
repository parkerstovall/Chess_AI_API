using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPiece
    {
        public string Color { get; set; }

        public List<int[]> GetPaths(Board board, int[] coords, bool check);
        public List<int[]> GetPressure(Board board, int[] coords);
    }
}
