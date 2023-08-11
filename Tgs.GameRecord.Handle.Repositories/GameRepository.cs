using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameRepository : IGame
{
    private readonly TimeSpan _absoluteExpirationRelativeToNow;
    private readonly IGameCommandClient _client;
    private readonly IMemoryCache _memoryCache;

    public GameRepository(
        IMemoryCache memoryCache,
        IGameCommandClient client,
        TimeSpan absoluteExpirationRelativeToNow)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }

    public async ValueTask<GameModel> GetAsync(
        Guid gameProviderId,
        string externaGamelId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        if (!_memoryCache.TryGetValue(key, out GameModel? game))
        {
            var info = await _client
               .GameGetAsync(
               gameProviderId: gameProviderId,
               gameExternalId: externaGamelId,
               cancellationToken: cancellationToken)
              .ConfigureAwait(false)
              ?? throw new NullReferenceException($"This game does not exist.({externaGamelId})");

            var gameType = info.Type switch
            {
                GameType.Sportsbook => GameRecordGameType.Sportsbook,
                GameType.VirtualSports => GameRecordGameType.VirtualSports,
                GameType.VirtualSportsLobby => GameRecordGameType.VirtualSports,
                GameType.ESports => GameRecordGameType.ESports,
                GameType.LiveDealer => GameRecordGameType.LiveDealer,
                GameType.LiveDealerLobby => GameRecordGameType.LiveDealer,
                GameType.CardGame => GameRecordGameType.CardGame,
                GameType.CardGameLobby => GameRecordGameType.CardGame,
                GameType.Fishing => GameRecordGameType.Fishing,
                GameType.FishingLobby => GameRecordGameType.Fishing,
                GameType.Arcade => GameRecordGameType.Arcade,
                GameType.ArcadeLobby => GameRecordGameType.Arcade,
                GameType.Lottery => GameRecordGameType.Lottery,
                GameType.LotteryLobby => GameRecordGameType.Lottery,
                GameType.Slot => GameRecordGameType.Slot,
                GameType.TableGame => GameRecordGameType.TableGame,
                _ => GameRecordGameType.Other
            };

            game = new GameModel(
               GameId: info.GameId,
               GameCode: info.GameCode,
               GameProviderId: info.GameProviderId,
               GameProviderCode: info.GameProviderCode,
               ExternalGameId: info.GameExternalId,
               GameType: gameType,
               SubGameProviderCode: info.SubGameProviderCode ?? info.GameProviderCode);

            return _memoryCache.Set(
                key: key,
                value: game,
                absoluteExpirationRelativeToNow: _absoluteExpirationRelativeToNow);
        }

        return game ?? throw new NullReferenceException($"This game does not exist.({externaGamelId})");
    }
}
