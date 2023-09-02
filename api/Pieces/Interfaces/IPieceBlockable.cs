using api.models.api;

namespace api.pieces.interfaces
{
    public interface IPieceCanPin
    {
        public bool IsPinning(Board board, int[] coords, int[] kingCoords, out IPiece pinnedPiece);
    }
}
