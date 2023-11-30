using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameRepository(
    IMemoryCache memoryCache,
    IGameCommandClient client,
    TimeSpan absoluteExpirationRelativeToNow) : IGame
{
    public async ValueTask<GameModel?> GetAsync(
        Guid gameProviderId,
        string externaGamelId,
        CancellationToken cancellationToken = default)
    {
        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        if (!memoryCache.TryGetValue(key, out GameModel? game))
        {
            var info = await client.GameGetAsync(
               gameProviderId: gameProviderId,
               gameExternalId: externaGamelId,
               cancellationToken: cancellationToken)
              .ConfigureAwait(false);

            if (info == null)
            {
                return null;
            }

            var gameType = info.Type switch
            {
                GameType.Sportsbook => GameTypes.Sportsbook,
                GameType.VirtualSports => GameTypes.VirtualSports,
                GameType.VirtualSportsLobby => GameTypes.VirtualSports,
                GameType.ESports => GameTypes.ESports,
                GameType.LiveDealer => GameTypes.LiveDealer,
                GameType.LiveDealerLobby => GameTypes.LiveDealer,
                GameType.CardGame => GameTypes.CardGame,
                GameType.CardGameLobby => GameTypes.CardGame,
                GameType.Fishing => GameTypes.Fishing,
                GameType.FishingLobby => GameTypes.Fishing,
                GameType.Arcade => GameTypes.Arcade,
                GameType.ArcadeLobby => GameTypes.Arcade,
                GameType.Lottery => GameTypes.Lottery,
                GameType.LotteryLobby => GameTypes.Lottery,
                GameType.Slot => GameTypes.Slot,
                GameType.TableGame => GameTypes.TableGame,
                _ => GameTypes.None
            };

            var gameCategory = gameType switch
            {
                GameTypes.Sportsbook => GameCategories.Sportsbook,
                GameTypes.VirtualSports => GameCategories.VirtualSports,
                GameTypes.ESports => GameCategories.Sportsbook,
                GameTypes.LiveDealer => GameCategories.LiveDealer,
                GameTypes.CardGame => GameCategories.CardGame,
                GameTypes.Fishing => GameCategories.Slot,
                GameTypes.Arcade => GameCategories.Slot,
                GameTypes.Lottery => GameCategories.Lottery,
                GameTypes.Slot => GameCategories.Slot,
                GameTypes.TableGame => GameCategories.Slot,
                _ => GameCategories.None
            };

            game = new GameModel(
               GameId: info.GameId,
               GameCode: info.GameCode,
               GameProviderId: info.GameProviderId,
               GameProviderCode: info.GameProviderCode,
               ExternalGameId: info.GameExternalId,
               GameCategory: gameCategory,
               GameType: gameType,
               SubGameProviderCode: info.SubGameProviderCode ?? info.GameProviderCode);

            return memoryCache.Set(
                key: key,
                value: game,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
        }

        return game;
    }
}
