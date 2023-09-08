namespace api.pieces.interfaces
{
    public interface IPieceHasMoved : IPiece
    {
        public bool HasMoved { get; set; }
    }
}
