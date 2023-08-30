using api.models.client;
using api.models.api;
using Microsoft.Extensions.Caching.Memory;
using api.helperclasses;
using Microsoft.AspNetCore.Components.Web;

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

            openGames?.Add(gameID, BoardHelper.GetNewBoard());

            _cache.Set("OpenGames", openGames);

            return gameID;
        }

        public BoardDisplay GetBoard(int gameID)
        {
            Board board =
                _cache.Get<Dictionary<int, Board>>("OpenGames")?[gameID]
                ?? BoardHelper.GetNewBoard();

            return BoardHelper.GetBoardForDisplay(board);
        }

        public List<int[]> GetMoves(int gameID, int row, int col)
        {
            Board board =
                _cache.Get<Dictionary<int, Board>>("OpenGames")?[gameID]
                ?? BoardHelper.GetNewBoard();

            List<int[]> moves = board.Rows[row].Squares[col].Piece?.GetPaths(board, false) ?? new();

            _cache.Set($"Moves:{gameID}", moves);
            return moves;
        }

        public string MovePiece(int gameID, int row, int col)
        {
            List<int[]> moves = _cache.Get<List<int[]>>($"Moves:{gameID}") ?? new();

            foreach (int[] move in moves)
            {
                if (move[0] == row && move[1] == col)
                {
                    return "{\"Response\": \"Valid\"}";
                }
            }

            return "{\"Response\": \"Invalid\"}";
        }
    }
}
