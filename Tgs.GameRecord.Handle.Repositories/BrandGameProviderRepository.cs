using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class BrandGameProviderRepository(
    IMemoryCache memoryCache,
    IGameCommandClient client,
    TimeSpan absoluteExpirationRelativeToNow) : IBrandGameProvider
{
    public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:brand:gameprovider:{brandId}:{gameProviderId}";

        if (!memoryCache.TryGetValue(key, out Guid gameProviderConfigurationId))
        {
            gameProviderConfigurationId = await client
                .BrandGetGameProviderConfigurationIdAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"The game provider configuration ID does not exist.({brandId}|{gameProviderId})");

            return memoryCache.Set(
                key: key,
                value: gameProviderConfigurationId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
        }

        return gameProviderConfigurationId;
    }
}
