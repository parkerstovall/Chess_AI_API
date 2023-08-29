using api.models;
using Microsoft.Extensions.Caching.Memory;

namespace api.repository
{
    public class GameRepository
    {
        private readonly IMemoryCache _cache;

        public GameRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<BoardDisplay> GetBoard(int gameID)
        {
            if (_cache.TryGetValue($"Board:{gameID}", out BoardDisplay? board) && board != null)
            {
                return board;
            }

            return await _cache.GetOrCreateAsync<BoardDisplay>(
                    $"Board:{gameID}",
                    entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                        return Task.FromResult(GetNewBoard());
                    }
                ) ?? GetNewBoard();
        }

        private BoardDisplay GetNewBoard()
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
            BoardDisplay board = new();

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

                    board.Rows[i].Squares.Add(new());
                    BoardDisplaySquare square = board.Rows[i].Squares[j];

                    string color = whiteSquare ? "white" : "black";
                    square.Col = i;
                    square.Row = j;
                    square.BackColor = color;
                    square.CssClass += $"{piece} {color} boardBtn".Trim();

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
    }
}
