using Microsoft.Extensions.Caching.Memory;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameCacheRepository(
    IGameRepository repository,
    IMemoryCache memoryCache) : IGameRepository
{
    public async ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId, CancellationToken cancellationToken = default)
    {
        string key = string.Concat(typeof(GameCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(GameCacheRepository).Name!.ToLower(), ":", gameProviderId, ":", externalGameId);
        if (!memoryCache.TryGetValue(key, out GameInfo? info))
        {
            info = await repository.GetAsync(
                gameProviderId: gameProviderId,
                externalGameId: externalGameId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (info is not null)
            {
                return memoryCache.Set(
                    key: key,
                    value: info,
                    absoluteExpirationRelativeToNow: TimeSpan.FromDays(1));
            }
        }

        return info;
    }

    public IAsyncEnumerable<GameExternaldInfo> QueryByExternaldAsync(string gameProviderCode, CancellationToken cancellationToken = default)
        => repository.QueryByExternaldAsync(gameProviderCode, cancellationToken);
}
