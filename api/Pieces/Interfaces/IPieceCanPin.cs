using ChessApi.HelperClasses.Chess;
using ChessApi.Models.API;
using ChessApi.Models.DB;

namespace ChessApi.Pieces.Interfaces
{
    public interface IPieceCanPin : IPiece
    {
        public bool CanPin(Board board, int[] start, int[] dest);
        public void CheckPins(int[] start, int[] dest, ref Game game);
        public bool HasSavingSquares(int[] start, int[] dest, ref Game game);
        public bool GoodDir(Direction dir);
    }
}
