using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class IPAddressController(
	IOptions<IPAddressOptions> options) : ControllerBase
{
	/// <summary>
	/// Gets this instance.
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public string Get()
		=> options.Value.IPAddress;
}
