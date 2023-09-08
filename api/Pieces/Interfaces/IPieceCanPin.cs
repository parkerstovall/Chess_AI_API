using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPieceCanPin : IPiece
    {
        public bool CanPin(Board board, int[] start, int[] dest);
        public void CheckSavingSquares(int[] start, int[] dest, ref Board board);
    }
}
