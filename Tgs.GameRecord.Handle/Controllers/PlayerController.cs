using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core.Models;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    [HttpGet("{playerCode}")]
    public ValueTask<PlayerInfo> GetIdAsync(
    [FromServices] IPlayerRepository repository,
    string playerCode)
    => repository.GetAsync(playerCode, HttpContext.RequestAborted);
}
