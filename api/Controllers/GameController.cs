using api.models.client;
using api.repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[ApiController]
[Route("api/v1/game")]
public class BoardController : ControllerBase
{
    private readonly GameRepository _repo;

    public BoardController(GameRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("startGame")]
    [SwaggerOperation(
        Summary = "Start new game",
        Description = "Builds new board and returns game ID"
    )]
    public GameStart StartGame()
    {
        return _repo.StartGame();
    }

    [HttpPost("{gameID}/click")]
    [SwaggerOperation(
        Summary = "HandlesClick",
        Description = "Handles board click and moves piece if applicable, returns board."
    )]
    public BoardDisplay HandleClick(int gameID, [FromQuery] int row, [FromQuery] int col)
    {
        return _repo.HandleClick(gameID, row, col);
    }

    [HttpGet("test")]
    [SwaggerOperation(Summary = "test", Description = "Returns 'test'.")]
    public string Test()
    {
        return "test";
    }
}
