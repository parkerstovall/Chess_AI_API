namespace api.pieces
{
    public static class PieceFactory
    {
        public static IPiece? GetPiece(string color, string type, int[] coords)
        {
            return type switch
            {
                "Pawn" => new Pawn(color, coords),
                "King" => new King(color, coords),
                "Bishop" => new Bishop(color, coords),
                "Queen" => new Queen(color, coords),
                "Rook" => new Rook(color, coords),
                "Knight" => new Knight(color, coords),
                _ => null,
            };
        }
    }
}
