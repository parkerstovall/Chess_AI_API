using System.Data;
using api.models;
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

    [HttpGet("{gameID}/getBoard")]
    [SwaggerOperation(Summary = "Get new board", Description = "Get new board")]
    public async Task<BoardDisplay> GetBoard(int gameID)
    {
        return await _repo.GetBoard(gameID);
    }
}
