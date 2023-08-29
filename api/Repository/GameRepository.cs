using api.models.client;
using api.models.api;
using Microsoft.Extensions.Caching.Memory;
using System.Xml;

namespace api.repository
{
    public class GameRepository
    {
        private readonly IMemoryCache _cache;

        public GameRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public int StartGame()
        {
            int gameID = 0;

            if (_cache.TryGetValue("OpenGames", out Dictionary<int, Board>? openGames))
            {
                gameID = openGames == null ? 0 : openGames.Count;
            }
            else
            {
                openGames = new();
            }

            openGames?.Add(gameID, GetNewBoard());

            _cache.Set("OpenGames", openGames);

            return gameID;
        }

        public BoardDisplay GetBoard(int gameID)
        {
            Board board = _cache.Get<Dictionary<int, Board>>("OpenGames")?[gameID] ?? GetNewBoard();

            return GetBoardForDisplay(board);
        }

        public BoardDisplay GetMoves(int gameID, int row, int col)
        {
            return new();
        }

        private Board GetNewBoard()
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

            bool whiteSquare = true;
            bool whitePiece = false;
            Board board = new();

            for (int i = 0; i < 8; i++)
            {
                board.Rows.Add(new());
                for (var j = 0; j < 8; j++)
                {
                    string piece = "";

                    if (i == 0 || i == 7)
                    {
                        piece = (whitePiece ? "white" : "black") + pieceOrder[j];
                    }
                    else if (i == 1 || i == 6)
                    {
                        piece += whitePiece ? "whitePawn" : "blackPawn";
                    }

                    string color = whiteSquare ? "white" : "black";

                    board.Rows[i].Squares.Add(
                        new()
                        {
                            Col = i,
                            Row = j,
                            BackColor = color,
                            CssClass = $"{piece} {color} boardBtn".Trim()
                        }
                    );

                    whiteSquare = !whiteSquare;
                }

                if (i == 1)
                {
                    whitePiece = !whitePiece;
                }

                whiteSquare = !whiteSquare;
            }

            return board;
        }

        //Don't want to expose the full board class to the client
        private BoardDisplay GetBoardForDisplay(Board board)
        {
            BoardDisplay boardDisplay = new();

            foreach (BoardRow row in board.Rows)
            {
                BoardDisplayRow displayRow = new();
                foreach (BoardSquare square in row.Squares)
                {
                    displayRow.Squares.Add(
                        new()
                        {
                            Col = square.Col,
                            Row = square.Row,
                            BackColor = square.BackColor,
                            CssClass = square.CssClass
                        }
                    );
                }

                boardDisplay.Rows.Add(displayRow);
            }

            return boardDisplay;
        }
    }
}
