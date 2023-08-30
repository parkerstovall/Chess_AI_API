using System.Text;
using api.models.api;
using api.models.client;
using api.pieces;

namespace api.helperclasses
{
    internal static class BoardHelper
    {
        internal static Board GetNewBoard()
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
        internal static BoardDisplay GetBoardForDisplay(Board board, List<int[]>? moves)
        {
            BoardDisplay boardDisplay = new();
            bool whiteSquare = true;

            foreach (BoardRow row in board.Rows)
            {
                BoardDisplayRow displayRow = new();

                foreach (BoardSquare square in row.Squares)
                {
                    displayRow.Squares.Add(CreateBoardSquare(whiteSquare, square, moves));
                    whiteSquare = !whiteSquare;
                }

                whiteSquare = !whiteSquare;
                boardDisplay.Rows.Add(displayRow);
            }

            return boardDisplay;
        }

        internal static BoardDisplaySquare CreateBoardSquare(
            bool whiteSquare,
            BoardSquare square,
            List<int[]>? moves
        )
        {
            string backColor = whiteSquare ? "white" : "black";
            string cssClass = $"boardBtn {backColor}";

            if (square.Piece != null)
            {
                cssClass += " " + square.Piece.ToString(false);
            }

            if (moves != null && CheckSquareInMoves(square, moves))
            {
                cssClass += " highlighted";
            }

            return new()
            {
                Col = square.Coords[0],
                Row = square.Coords[1],
                CssClass = cssClass
            };
        }

        internal static bool CheckSquareInMoves(BoardSquare square, List<int[]> moves)
        {
            foreach (int[] move in moves)
            {
                if (move[0] == square.Coords[0] && move[1] == square.Coords[1])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
