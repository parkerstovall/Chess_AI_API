using api.helperclasses;
using api.models.api;
using api.models.client;
using api.models.db;
using api.pieces.interfaces;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;

namespace api.repository
{
    public class GameRepository(ConnectionRepository connRepo, IHttpContextAccessor context)
    {
        private readonly ConnectionRepository _connRepo = connRepo;
        private readonly IHttpContextAccessor _context = context;

        public async Task<BoardDisplay> StartGame(bool isWhite, bool isTwoPlayer)
        {
            //Create new Game
            var game = new Game()
            {
                Board = BoardHelper.GetNewBoard(),
                IsPlayerWhite = isWhite,
                IsTwoPlayer = isTwoPlayer
            };

            // Add cookie to response, save game to DB
            await GetsertGame(game, true);

            return BoardHelper.GetBoardForDisplay(game);
        }

        public async Task<ClickReturn> HandleClick(int row, int col)
        {
            Game game = await GetsertGame();
            // if (!game.IsTwoPlayer && game.IsPlayerWhite != game.IsWhiteTurn)
            // {
            //     return new() { Moved = false, Board = BoardHelper.GetBoardForDisplay(game) };
            // }

            bool moved = false;
            int[]? clickedSquare = [row, col];
            BoardSquare square = game.Board.Rows[row].Squares[col];
            var currentTurnColor = game.IsWhiteTurn ? "white" : "black";
            var playerColor = game.IsPlayerWhite ? "white" : "black";

            if (
                game.AvailableMoves.Count != 0
                && game.SelectedSquare != null
                && MoveHelper.TryMovePiece(
                    clickedSquare,
                    game.SelectedSquare,
                    game.AvailableMoves,
                    ref game
                )
            )
            {
                IPiece? piece = game.Board.Rows[row].Squares[col].Piece;
                if (piece != null && game.SelectedSquare != null)
                {
                    var moveHistory = _connRepo.GetCollection<Move>("MoveHistory");
                    await moveHistory.InsertOneAsync(
                        new Move
                        {
                            From = game.SelectedSquare,
                            To = clickedSquare,
                            PieceColor = piece.Color,
                            PieceType = piece.GetType().Name
                        }
                    );
                }

                game.AvailableMoves.Clear();
                game.IsWhiteTurn = !game.IsWhiteTurn;
                game.SelectedSquare = null;
                moved = true;
            }
            else if (square.Piece == null
            // || square.Piece.Color != currentTurnColor
            // || (!game.IsTwoPlayer && square.Piece.Color != playerColor)
            )
            {
                game.AvailableMoves.Clear();
                game.SelectedSquare = null;
                return new() { Moved = false, Board = BoardHelper.GetBoardForDisplay(game) };
            }
            else
            {
                game.AvailableMoves = MoveHelper.GetMovesFromPiece(
                    game.Board,
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

            ChessAI ai = new(game.IsPlayerWhite);
            ai.GetMove(ref game, out Move? foundMove);

            if (foundMove != null)
            {
                await _connRepo.GetCollection<Move>("MoveHistory").InsertOneAsync(foundMove);
            }

            game.IsWhiteTurn = !game.IsWhiteTurn;
            game = await GetsertGame(game);

            return BoardHelper.GetBoardForDisplay(game);
        }

        private async Task<Game> GetsertGame(Game? updateGame = null, bool forceInsert = false)
        {
            var collection = _connRepo.GetCollection<Game>("Games");
            var gameID = _context?.HttpContext?.Request.Cookies["GameID"];
            if (gameID != null && ObjectId.TryParse(gameID, out ObjectId oGameID))
            {
                var filter = Builders<Game>.Filter.Eq("_id", oGameID);
                if (forceInsert)
                {
                    //End the current game, then start a new one after the if block
                    var update = Builders<Game>.Update.Set(
                        game => game.Status,
                        GameStatus.UserEnded.ToString()
                    );

                    await collection.UpdateOneAsync(filter, update);
                }
                else
                {
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
