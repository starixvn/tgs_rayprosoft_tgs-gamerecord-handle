using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class PlayerRepository : IPlayer
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly IGameCommandClient _client;
    private readonly IMemoryCache _memoryCache;

    public PlayerRepository(
        IMemoryCache memoryCache,
        IGameCommandClient client,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:player:{playerCode}";

        if (!_memoryCache.TryGetValue(key, out PlayerInfo? info))
        {
            info = await _client
                .PlayerGetAsync(
               playerCode: playerCode,
               cancellationToken: cancellationToken)
               .ConfigureAwait(false)
               ?? throw new NullReferenceException($"This player does not exist.({playerCode})");

            return _memoryCache
                .Set(
                key: key,
                value: info,
                absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }

        return info ?? throw new NullReferenceException($"This player does not exist.({playerCode})");
    }
}
