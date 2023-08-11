using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class BrandGameProviderRepository : IBrandGameProvider
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly IGameCommandClient _client;
    private readonly IMemoryCache _memoryCache;

    public BrandGameProviderRepository(
        IMemoryCache memoryCache,
        IGameCommandClient client,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:brand:gameprovider:{brandId}:{gameProviderId}";

        if (!_memoryCache.TryGetValue(key, out Guid gameProviderConfigurationId))
        {
            gameProviderConfigurationId = await _client
                .BrandGetGameProviderConfigurationIdAsync(
                brandId: brandId,
                gameProviderId: gameProviderId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new NullReferenceException($"The game provider configuration ID does not exist.({brandId}|{gameProviderId})");

            return _memoryCache.Set(
                key: key,
                value: gameProviderConfigurationId,
                absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }

        return gameProviderConfigurationId;
    }
}
