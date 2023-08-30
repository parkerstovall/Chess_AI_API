using System.Text;
using api.models.api;
using api.models.client;
using api.pieces;

namespace api.helperclasses
{
    public static class BoardHelper
    {
        public static Board GetNewBoard()
        {
            string[] pieceOrder =
            {
                "Rook",
                "Knight",
                "Bishop",
                "Queen",
                "King",
                "Bishop",
                "Knight",
                "Rook"
            };

            bool whitePiece = false;
            Board board = new();

            for (int i = 0; i < 8; i++)
            {
                board.Rows.Add(new());
                for (var j = 0; j < 8; j++)
                {
                    IPiece? piece = null;
                    if (i == 0 || i == 7)
                    {
                        piece = PieceFactory.GetPiece(
                            whitePiece ? "white" : "black",
                            pieceOrder[j],
                            new int[] { i, j }
                        );
                    }
                    else if (i == 1 || i == 6)
                    {
                        piece = PieceFactory.GetPiece(
                            whitePiece ? "white" : "black",
                            "Pawn",
                            new int[] { i, j }
                        );
                    }

                    board.Rows[i].Squares.Add(new() { Coords = new int[] { i, j }, Piece = piece });
                }

                if (i == 1)
                {
                    whitePiece = !whitePiece;
                }
            }

            return board;
        }

        //Don't want to expose the full board class to the client
        public static BoardDisplay GetBoardForDisplay(Board board)
        {
            BoardDisplay boardDisplay = new();
            bool whiteSquare = true;

            foreach (BoardRow row in board.Rows)
            {
                BoardDisplayRow displayRow = new();
                foreach (BoardSquare square in row.Squares)
                {
                    string backColor = whiteSquare ? "white" : "black";
                    string cssClass = $"boardBtn {backColor}";

                    if (square.Piece != null)
                    {
                        cssClass += " " + square.Piece.ToString(false);
                    }

                    displayRow.Squares.Add(
                        new()
                        {
                            Col = square.Coords[0],
                            Row = square.Coords[1],
                            CssClass = cssClass
                        }
                    );

                    whiteSquare = !whiteSquare;
                }

                whiteSquare = !whiteSquare;
                boardDisplay.Rows.Add(displayRow);
            }

            return boardDisplay;
        }
    }
}
