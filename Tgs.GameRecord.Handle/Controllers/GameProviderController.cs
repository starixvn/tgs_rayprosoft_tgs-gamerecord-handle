using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class GameProviderController : ControllerBase
{
    [HttpGet("{code}")]
    public ValueTask<Guid> GetIdAsync(
        [FromServices] IGameProviderRepository repository,
        string code)
        => repository.GetIdAsync(code, HttpContext.RequestAborted);

    [HttpGet("by-code")]
    public IAsyncEnumerable<string> QueryByCodeAsync(
        [FromServices] IGameProviderRepository repository)
        => repository.QueryByCodeAsync(HttpContext.RequestAborted);
}
