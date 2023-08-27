using Microsoft.AspNetCore.Mvc;

namespace ChessAPI.Controllers;

[ApiController]
[Route("api/v1/moves")]
public class MovesController : ControllerBase
{
    private readonly ILogger<MovesController> _logger;

    public MovesController(ILogger<MovesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string GetMoveSet()
    {
        return "{test: 'test'}";
    }
}
