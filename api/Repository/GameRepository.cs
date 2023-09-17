using api.models.client;
using api.models.api;
using Microsoft.Extensions.Caching.Memory;
using api.helperclasses;

namespace api.repository
{
    public class GameRepository
    {
        private readonly IMemoryCache _cache;

        public GameRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        public GameStart StartGame()
        {
            Guid gameID = Guid.NewGuid();
            Board board = BoardHelper.GetNewBoard();

            List<OpenGame> openGames = _cache.Get<List<OpenGame>>("OpenGames") ?? new();
            openGames.Add(new OpenGame { GameID = gameID, LastPing = DateTime.Now });
            _cache.Set("OpenGames", openGames);

            _cache.Set($"Board:{gameID}", board);
            _cache.Set($"Turn:{gameID}", "white");

            return new GameStart
            {
                Board = BoardHelper.GetBoardForDisplay(board, null, null),
                GameID = gameID
            };
        }

        public BoardDisplay HandleClick(Guid gameID, int row, int col)
        {
            Board board = _cache.Get<Board>($"Board:{gameID}") ?? BoardHelper.GetNewBoard();
            string? color = _cache.Get<string>($"Turn:{gameID}");

            int[]? clickedSquare = new[] { row, col };

            BoardSquare square = board.Rows[row].Squares[col];

            List<int[]> moves = _cache.Get<List<int[]>>($"Moves:{gameID}") ?? new();
            int[] start = _cache.Get<int[]>($"SelectedSquare:{gameID}") ?? Array.Empty<int>();

            if (
                moves.Any()
                && start.Any()
                && MoveHelper.TryMovePiece(
                    clickedSquare,
                    start,
                    ref moves,
                    ref board,
                    out bool check
                )
            )
            {
                _cache.Set($"Turn:{gameID}", color == "white" ? "black" : "white");
                _cache.Remove($"Moves:{gameID}");
                _cache.Remove($"SelectedSquare:{gameID}");
                _cache.Set($"Check:{gameID}", check);
                clickedSquare = null;
            }
            else if (square.Piece == null || square.Piece.Color != color)
            {
                return BoardHelper.GetBoardForDisplay(board, null, null);
            }
            else
            {
                check = _cache.Get<bool>($"Check:{gameID}");
                moves = MoveHelper.GetMovesFromPiece(board, clickedSquare, check);
                _cache.Set($"Moves:{gameID}", moves);
                _cache.Set($"SelectedSquare:{gameID}", clickedSquare);
            }

            return BoardHelper.GetBoardForDisplay(board, moves, clickedSquare);
        }

        public bool Ping(Guid gameID)
        {
            List<OpenGame> openGames = _cache.Get<List<OpenGame>>("OpenGames") ?? new();
            OpenGame? game = openGames.FirstOrDefault(g => g.GameID == gameID);

            if (game is not null)
            {
                game.LastPing = DateTime.Now;
                _cache.Set("OpenGames", openGames);
                return true;
            }

            return false;
        }
    }
}
