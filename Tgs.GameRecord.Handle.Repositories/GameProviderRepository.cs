using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameProviderRepository : IGameProvider
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly IGameCommandClient _client;
    private readonly IMemoryCache _memoryCache;

    public GameProviderRepository(
        IMemoryCache memoryCache,
        IGameCommandClient client,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<Guid> GetIdAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:gameprovider:{code}";

        if (!_memoryCache.TryGetValue(key, out Guid id))
        {
            id = await _client
               .GameProviderGetGameProviderIdAsync(
               gameProviderCode: code,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? throw new NullReferenceException($"This game provider does not exist.({code})");

            return _memoryCache
                .Set(
                key: key,
                value: id,
                absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }

        return id;
    }
}
