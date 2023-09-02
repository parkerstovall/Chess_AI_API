using api.pieces.interfaces;

namespace api.pieces
{
    public static class PieceFactory
    {
        public static IPiece? GetPiece(string color, string type)
        {
            return type switch
            {
                "Pawn" => new Pawn(color),
                "King" => new King(color),
                "Bishop" => new Bishop(color),
                "Queen" => new Queen(color),
                "Rook" => new Rook(color),
                "Knight" => new Knight(color),
                _ => null,
            };
        }
    }
}
