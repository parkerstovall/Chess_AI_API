using api.helperclasses.chess;
using api.models.api;
using api.models.db;

namespace api.pieces.interfaces
{
    public interface IPieceCanPin : IPiece
    {
        public bool CanPin(Board board, int[] start, int[] dest);
        public void CheckPins(int[] start, int[] dest, ref Game game);
        public bool HasSavingSquares(int[] start, int[] dest, ref Game game);
        public bool GoodDir(Direction dir);
    }
}
