using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    [HttpGet("{gameProviderId:guid}/{externalGameId}")]
    public ValueTask<GameInfo?> GetIdAsync(
        [FromServices] IGameRepository repository,
        Guid gameProviderId,
        string externalGameId)
        => repository.GetAsync(gameProviderId, externalGameId, HttpContext.RequestAborted);

    [HttpGet("by-externa-id")]
    public IAsyncEnumerable<GameExternaldInfo> QueryByExternaldAsync(
        [FromServices] IGameRepository repository,
        string gameProviderCode)
        => repository.QueryByExternaldAsync(gameProviderCode, HttpContext.RequestAborted);
}
