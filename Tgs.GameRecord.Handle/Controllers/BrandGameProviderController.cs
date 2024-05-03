using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class BrandGameProviderController : ControllerBase
{
	[HttpGet("{brandId:guid}/{gameProviderId:guid}")]
	public ValueTask<Guid> GetIdAsync(
	[FromServices] IBrandGameProviderRepository repository,
	Guid brandId,
	Guid gameProviderId)
	=> repository.GetGameProviderConfigurationIdAsync(brandId, gameProviderId, HttpContext.RequestAborted);
}
