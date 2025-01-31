namespace ChessApi.Pieces.Interfaces
{
    public interface IPieceHasMoved : IPiece
    {
        public bool HasMoved { get; set; }
    }
}
