using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameProviderRepository(
    IMemoryCache memoryCache,
    IGameCommandClient client,
    TimeSpan absoluteExpirationRelativeToNow) : IGameProvider
{
    public async ValueTask<Guid> GetIdAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:gameprovider:{code}";

        if (!memoryCache.TryGetValue(key, out Guid id))
        {
            id = await client.GameProviderGetGameProviderIdAsync(
                gameProviderCode: code,
                cancellationToken: cancellationToken)
                ?? throw new NullReferenceException($"This game provider does not exist.({code})");

            return memoryCache.Set(
                key: key,
                value: id,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
        }

        return id;
    }
}
