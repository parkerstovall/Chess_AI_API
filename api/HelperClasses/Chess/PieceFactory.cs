using ChessApi.Pieces;
using ChessApi.Pieces.Interfaces;

namespace ChessApi.HelperClasses.Chess
{
    internal static class PieceFactory
    {
        internal static IPiece? GetPiece(bool color, string type)
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
