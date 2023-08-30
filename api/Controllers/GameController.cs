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

    [HttpGet("{gameID}/getMoves")]
    [SwaggerOperation(
        Summary = "Gets moves for click",
        Description = "Uses gameID and supplied col / row"
    )]
    public List<int[]> GetMoves(int gameID, [FromQuery] int row, [FromQuery] int col)
    {
        return _repo.GetMoves(gameID, row, col);
    }

    [HttpPost("{gameID}/movePiece")]
    [SwaggerOperation(
        Summary = "Moves Piece",
        Description = "Moves Piece using gameID and supplied col / row and Cached moves"
    )]
    public string MovePiece(int gameID, [FromQuery] int row, [FromQuery] int col)
    {
        return _repo.MovePiece(gameID, row, col);
    }
}
