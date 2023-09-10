using api.helperclasses;
using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPieceCanPin : IPiece
    {
        public bool CanPin(Board board, int[] start, int[] dest);
        public void CheckPins(int[] start, int[] dest, ref Board board);
        public bool HasSavingSquares(int[] start, int[] dest, ref Board board);
        public bool GoodDir(Direction dir);
    }
}
