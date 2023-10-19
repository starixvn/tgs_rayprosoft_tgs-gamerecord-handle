using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class SnowflakeController : ControllerBase
{
    [HttpGet("DatacenterId")]
    public long GetDatacenterId(
        [FromServices] ISnowflakeWork work)
        => work.GetDatacenterId();

    [HttpGet("Id")]
    public long GetId(
        [FromServices] ISnowflakeWork work)
        => work.GetWagerId();

    [HttpGet("IPAddress")]
    public string GetIPAddress(
        [FromServices] ISnowflakeWork work)
        => work.GetIPAddress().ToString();

    [HttpGet("WorkerId")]
    public long GetWorkerId(
        [FromServices] ISnowflakeWork work)
        => work.GetWorkerId();
}
