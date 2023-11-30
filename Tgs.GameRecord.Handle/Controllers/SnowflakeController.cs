using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class SnowflakeController : ControllerBase
{
    /// <summary>
    /// Gets the datacenter identifier.
    /// </summary>
    /// <param name="work">The work.</param>
    /// <returns></returns>
    [HttpGet("DatacenterId")]
    public long GetDatacenterId(
        [FromServices] ISnowflakeWork work)
        => work.GetDatacenterId();

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="work">The work.</param>
    /// <returns></returns>
    [HttpGet("Id")]
    public long GetId(
        [FromServices] ISnowflakeWork work)
        => work.GetId();

    /// <summary>
    /// Gets the worker identifier.
    /// </summary>
    /// <param name="work">The work.</param>
    /// <returns></returns>
    [HttpGet("WorkerId")]
    public long GetWorkerId(
        [FromServices] ISnowflakeWork work)
        => work.GetWorkerId();
}
