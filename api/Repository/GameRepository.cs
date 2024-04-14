using api.helperclasses;
using api.models.api;
using api.models.client;
using api.models.db;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.repository
{
    public class GameRepository(ConnectionRepository connRepo, IHttpContextAccessor context)
    {
        private readonly ConnectionRepository _connRepo = connRepo;
        private readonly IHttpContextAccessor _context = context;

        public async Task<BoardDisplay> StartGame(bool isWhite)
        {
            //Create new Game
            var game = new Game() { Board = BoardHelper.GetNewBoard(), IsPlayerWhite = isWhite };

            // Add cookie to response, save game to DB
            await GetsertGame(game);

            return BoardHelper.GetBoardForDisplay(game);
        }

        public async Task<ClickReturn> HandleClick(int row, int col)
        {
            Game game = await GetsertGame();
            if (game.IsPlayerWhite != game.IsWhiteTurn)
            {
                return new() { Moved = false, Board = BoardHelper.GetBoardForDisplay(game) };
            }

            bool moved = false;
            int[]? clickedSquare = [row, col];
            var board = game.Board;
            BoardSquare square = board.Rows[row].Squares[col];
            var currentTurnColor = game.IsWhiteTurn ? "white" : "black";
            var playerColor = game.IsPlayerWhite ? "white" : "black";

            if (
                game.AvailableMoves.Count != 0
                && game.SelectedSquare != null
                && MoveHelper.TryMovePiece(
                    clickedSquare,
                    game.SelectedSquare,
                    game.AvailableMoves,
                    ref board,
                    out string? checkColor
                )
            )
            {
                game.Board = board;
                game.AvailableMoves.Clear();
                game.IsWhiteTurn = !game.IsWhiteTurn;
                game.SelectedSquare = null;
                game.CheckedColor = checkColor;
                moved = true;
            }
            else if (
                square.Piece == null
                || square.Piece.Color != currentTurnColor
                || square.Piece.Color != playerColor
            )
            {
                game.AvailableMoves.Clear();
                game.SelectedSquare = null;
                return new() { Moved = false, Board = BoardHelper.GetBoardForDisplay(game) };
            }
            else
            {
                game.AvailableMoves = MoveHelper.GetMovesFromPiece(
                    board,
                    clickedSquare,
                    game.CheckedColor
                );
                game.SelectedSquare = clickedSquare;
            }

            game = await GetsertGame(game);

            return new() { Moved = moved, Board = BoardHelper.GetBoardForDisplay(game) };
        }

        public async Task<BoardDisplay> CompMove()
        {
            var game = await GetsertGame();
            if (game.IsWhiteTurn == game.IsPlayerWhite)
            {
                return BoardHelper.GetBoardForDisplay(game);
            }

            ChessAI ai = new(game.CheckedColor, game.IsPlayerWhite);

            game.Board = ai.GetMove(game.Board, out string? checkColor);
            game.CheckedColor = checkColor;
            game.IsWhiteTurn = !game.IsWhiteTurn;
            game = await GetsertGame(game);

            return BoardHelper.GetBoardForDisplay(game);
        }

        private async Task<Game> GetsertGame(Game? updateGame = null)
        {
            var collection = _connRepo.GetCollection<Game>("Games");
            var gameID = _context?.HttpContext?.Request.Cookies["GameID"];
            if (gameID != null && ObjectId.TryParse(gameID, out ObjectId oGameID))
            {
                var filter = Builders<Game>.Filter.Eq("_id", oGameID);

                //Update and return if game is provided
                if (updateGame != null)
                {
                    updateGame.GameID = oGameID;
                    var replaceOptions = new ReplaceOptions() { IsUpsert = true };
                    await collection.ReplaceOneAsync(filter, updateGame, replaceOptions);
                    return updateGame;
                }

                // Get if no game is provided
                var dbGame = await collection.Find(filter).FirstOrDefaultAsync();
                if (dbGame != null)
                {
                    return dbGame;
                }
            }

            //Insert if no game ID in cookie and game is provided
            if (updateGame != null)
            {
                await collection.InsertOneAsync(updateGame);
                _context?.HttpContext?.Response.Cookies.Append(
                    "GameID",
                    updateGame.GameID.ToString()
                );
                return updateGame;
            }

            // Otherwise, bad request
            throw new InvalidOperationException("GameID or UpdateGame must be present");
        }
    }
}
