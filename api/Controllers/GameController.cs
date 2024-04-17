using api.models.client;
using api.repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[ApiController]
[Route("api/v1/game")]
public class BoardController(GameRepository repo) : ControllerBase
{
    private readonly GameRepository _repo = repo;

    [HttpPost("tryGetSavedGame")]
    [SwaggerOperation(
        Summary = "Try Get Game From Cookie",
        Description = "Try to get a saved game from cookie"
    )]
    public async Task<SavedGameResult> TryGetSavedGame()
    {
        return await _repo.TryGetSavedGame();
    }

    [HttpPost("startGame")]
    [SwaggerOperation(
        Summary = "Start new game",
        Description = "Builds new board and returns game ID"
    )]
    public async Task<BoardDisplay> StartGame(bool isWhite, bool isTwoPlayer = false)
    {
        return await _repo.StartGame(isWhite, isTwoPlayer);
    }

    [HttpPost("compMove")]
    [SwaggerOperation(
        Summary = "Gets computer move",
        Description = "Runs a MinMax algorithm on the board and returns the best move"
    )]
    public async Task<BoardDisplay> CompMove()
    {
        return await _repo.CompMove();
    }

    [HttpPost("click")]
    [SwaggerOperation(
        Summary = "HandlesClick",
        Description = "Handles board click and moves piece if applicable, returns board."
    )]
    public async Task<ClickReturn> HandleClick(int row, int col)
    {
        return await _repo.HandleClick(row, col);
    }
}
