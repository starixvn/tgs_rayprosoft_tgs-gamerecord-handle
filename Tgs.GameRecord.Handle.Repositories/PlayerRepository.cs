using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class PlayerRepository(
    IMemoryCache memoryCache,
    IGameCommandClient client,
    TimeSpan absoluteExpirationRelativeToNow) : IPlayer
{
    public async ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:player:{playerCode}";

        if (!memoryCache.TryGetValue(key, out PlayerInfo? info))
        {
            info = await client
                .PlayerGetAsync(
               playerCode: playerCode,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? throw new NullReferenceException($"This player does not exist.({playerCode})");

            return memoryCache
                .Set(
                key: key,
                value: info,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
        }

        return info ?? throw new NullReferenceException($"This player does not exist.({playerCode})");
    }
}
