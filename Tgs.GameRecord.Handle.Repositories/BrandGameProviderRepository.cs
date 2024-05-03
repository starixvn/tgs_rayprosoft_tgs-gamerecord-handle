using Microsoft.EntityFrameworkCore;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class BrandGameProviderRepository(
	TgsContext context) : IBrandGameProviderRepository
{
	public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
	{
		var brandGameProvider = await context.BrandGameProviders
			.FirstOrDefaultAsync(
			predicate: bgp => bgp.BrandId == brandId && bgp.GameProviderId == gameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"The game provider configuration ID does not exist.({brandId}|{gameProviderId})");

		return brandGameProvider.GameProviderConfigurationId;
	}
}
