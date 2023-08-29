using api.models.client;
using api.repository;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[ApiController]
[Route("api/v1/game")]
public class BoardController : ControllerBase
{
    private readonly ILogger<BoardController> _logger;
    private readonly GameRepository _repo;

    public BoardController(ILogger<BoardController> logger, GameRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet("startGame")]
    [SwaggerOperation(
        Summary = "Start new game",
        Description = "Builds new board and returns game ID"
    )]
    public int StartGame()
    {
        return _repo.StartGame();
    }

    [HttpGet("{gameID}/getBoard")]
    [SwaggerOperation(Summary = "Get new board", Description = "Get new board")]
    public BoardDisplay GetBoard(int gameID)
    {
        return _repo.GetBoard(gameID);
    }

    [HttpGet("{gameID}/moves")]
    [SwaggerOperation(
        Summary = "Gets moves for click",
        Description = "Uses gameID and supplied col / row"
    )]
    public BoardDisplay GetMoves(int gameID, [FromQuery] int row, [FromQuery] int col)
    {
        return _repo.GetMoves(gameID, row, col);
    }
}
