using Microsoft.Extensions.Caching.Memory;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class BrandGameProviderCacheRepository(
	IBrandGameProviderRepository repository,
	IMemoryCache memoryCache) : IBrandGameProviderRepository
{
	public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default)
	{
		string key = string.Concat(typeof(BrandGameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(BrandGameProviderCacheRepository).Name!.ToLower(), ":", brandId, ":", gameProviderId);
		if (!memoryCache.TryGetValue(key, out Guid gameProviderConfigurationId))
		{
			gameProviderConfigurationId = await repository.GetGameProviderConfigurationIdAsync(
				brandId: brandId,
				gameProviderId: gameProviderId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache.Set(
				key: key,
				value: gameProviderConfigurationId,
				absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(1));
		}

		return gameProviderConfigurationId;
	}
}
