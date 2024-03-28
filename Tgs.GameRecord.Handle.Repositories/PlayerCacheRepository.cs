using Microsoft.Extensions.Caching.Memory;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class PlayerCacheRepository(
    IPlayerRepository repository,
    IMemoryCache memoryCache) : IPlayerRepository
{
    public async ValueTask<PlayerInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default)
    {
        string key = string.Concat(typeof(PlayerCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerCacheRepository).Name!.ToLower(), ":", playerCode);
        if (!memoryCache.TryGetValue(key, out PlayerInfo? playerInfo))
        {
            playerInfo = await repository.GetAsync(
                playerCode: playerCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (playerInfo is not null)
            {
                return memoryCache.Set(
                    key: key,
                    value: playerInfo,
                    absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
            }
        }

        return playerInfo;
    }
}
