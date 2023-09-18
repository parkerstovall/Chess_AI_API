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

        public GameStart StartGame(bool isWhite)
        {
            Guid gameID = Guid.NewGuid();
            Board board = BoardHelper.GetNewBoard();

            List<OpenGame> openGames = _cache.Get<List<OpenGame>>("OpenGames") ?? new();
            openGames.Add(new OpenGame { GameID = gameID, LastPing = DateTime.Now });
            _cache.Set("OpenGames", openGames);

            _cache.Set($"Board:{gameID}", board);
            _cache.Set($"Turn:{gameID}", "white");
            _cache.Set($"PlayerColor:{gameID}", isWhite ? "white" : "black");
            return new GameStart
            {
                Board = BoardHelper.GetBoardForDisplay(board, null, null),
                GameID = gameID
            };
        }

        public ClickReturn HandleClick(Guid gameID, int row, int col)
        {
            Board board = _cache.Get<Board>($"Board:{gameID}") ?? BoardHelper.GetNewBoard();
            bool moved = false;
            if (_cache.TryGetValue($"Board:{gameID}:compTurn", out bool isCompTurn) && isCompTurn)
            {
                return new()
                {
                    Moved = moved,
                    Board = BoardHelper.GetBoardForDisplay(board, null, null)
                };
            }

            string? color = _cache.Get<string>($"Turn:{gameID}");
            string? playerColor = _cache.Get<string>($"PlayerColor:{gameID}");

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
                    out string checkColor
                )
            )
            {
                _cache.Set($"Turn:{gameID}", color == "white" ? "black" : "white");
                _cache.Remove($"Moves:{gameID}");
                _cache.Remove($"SelectedSquare:{gameID}");
                _cache.Set($"Check:{gameID}", checkColor);
                clickedSquare = null;
                moved = true;
            }
            else if (
                square.Piece == null
                || square.Piece.Color != color
                || square.Piece.Color != playerColor
            )
            {
                _cache.Remove($"Moves:{gameID}");
                _cache.Remove($"SelectedSquare:{gameID}");
                return new()
                {
                    Moved = moved,
                    Board = BoardHelper.GetBoardForDisplay(board, null, null)
                };
            }
            else
            {
                checkColor = _cache.Get<string>($"Check:{gameID}") ?? "";
                moves = MoveHelper.GetMovesFromPiece(board, clickedSquare, checkColor);
                _cache.Set($"Moves:{gameID}", moves);
                _cache.Set($"SelectedSquare:{gameID}", clickedSquare);
            }

            return new()
            {
                Moved = moved,
                Board = BoardHelper.GetBoardForDisplay(board, moves, clickedSquare)
            };
        }

        public BoardDisplay CompMove(Guid gameID)
        {
            _cache.Set($"Board:{gameID}:compTurn", true);
            Board board = _cache.Get<Board>($"Board:{gameID}") ?? BoardHelper.GetNewBoard();
            string? color = _cache.Get<string>($"Turn:{gameID}");
            string checkColor = _cache.Get<string>($"Check:{gameID}") ?? "";

            ChessAI ai = new(checkColor, color == "black");

            Board newBoard = ai.GetMove(board, out checkColor);

            _cache.Set($"Board:{gameID}", newBoard);
            _cache.Set($"Turn:{gameID}", color == "white" ? "black" : "white");
            _cache.Set($"Board:{gameID}:compTurn", false);
            _cache.Set($"Check:{gameID}", checkColor);
            return BoardHelper.GetBoardForDisplay(newBoard, null, null);
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
