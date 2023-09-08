using api.pieces.interfaces;
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
                            pieceOrder[j]
                        );
                    }
                    else if (i == 1 || i == 6)
                    {
                        piece = PieceFactory.GetPiece(whitePiece ? "white" : "black", "Pawn");
                    }

                    board.Rows[i].Squares.Add(new() { coords = new int[] { i, j }, Piece = piece });
                }

                if (i == 1)
                {
                    whitePiece = !whitePiece;
                }
            }

            return board;
        }

        //Don't want to expose the full board class to the client
        internal static BoardDisplay GetBoardForDisplay(
            Board board,
            List<int[]>? moves,
            int[]? selectedSquare
        )
        {
            BoardDisplay boardDisplay = new();
            bool whiteSquare = true;

            foreach (BoardRow row in board.Rows)
            {
                BoardDisplayRow displayRow = new();

                foreach (BoardSquare square in row.Squares)
                {
                    displayRow.Squares.Add(
                        CreateBoardSquare(whiteSquare, square, moves, selectedSquare)
                    );
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
            List<int[]>? moves,
            int[]? selectedSquare
        )
        {
            string backColor = whiteSquare ? "white" : "black";
            string cssClass = $"boardBtn {backColor}";

            if (square.Piece != null)
            {
                cssClass += " " + square.Piece.ToString();

                if (
                    selectedSquare != null
                    && selectedSquare[0] == square.coords[0]
                    && selectedSquare[1] == square.coords[1]
                )
                {
                    cssClass += " selected";
                }

                if (square.Piece is King king && king.InCheck)
                {
                    cssClass += " inCheck";
                }
            }

            if (moves != null && CheckSquareInMoves(square, moves))
            {
                cssClass += " highlighted";
            }

            return new()
            {
                Col = square.coords[0],
                Row = square.coords[1],
                CssClass = cssClass
            };
        }

        internal static bool CheckSquareInMoves(BoardSquare square, List<int[]> moves)
        {
            foreach (int[] move in moves)
            {
                if (move[0] == square.coords[0] && move[1] == square.coords[1])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
