using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameRepository(
    TgsContext context) : IGameRepository
{
    public async ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId, CancellationToken cancellationToken = default)
    {
        var gameInfo = await context.Games
            .Where(predicate: g => g.GameProviderId == gameProviderId && g.ExternalId == externalGameId)
            .Join(context.GameProviders,
            game => game.GameProviderId,
            gameProvider => gameProvider.Id,
            (game, gameProvider) => new
            {
                GameProviderId = gameProvider.Id,
                GameProviderCode = gameProvider.Code,
                game.SubGameProviderCode,
                GameId = game.Id,
                GameCode = game.Code,
                GameExternalId = game.ExternalId,
                GameType = game.Type
            })
            .FirstOrDefaultAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (gameInfo is null)
        {
            return null;
        }

        var gameType = gameInfo.GameType switch
        {
            1 => GameTypes.Sportsbook,
            2 => GameTypes.VirtualSports,
            3 => GameTypes.VirtualSports,
            4 => GameTypes.ESports,
            5 => GameTypes.LiveDealer,
            6 => GameTypes.LiveDealer,
            7 => GameTypes.CardGame,
            8 => GameTypes.CardGame,
            9 => GameTypes.Fishing,
            10 => GameTypes.Fishing,
            11 => GameTypes.Arcade,
            12 => GameTypes.Arcade,
            13 => GameTypes.Lottery,
            14 => GameTypes.Lottery,
            15 => GameTypes.Slot,
            16 => GameTypes.TableGame,
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

        return new GameInfo(
            GameId: gameInfo.GameId,
            GameCode: gameInfo.GameCode,
            GameProviderId: gameInfo.GameProviderId,
            GameProviderCode: gameInfo.GameProviderCode,
            ExternalGameId: gameInfo.GameExternalId,
            GameCategory: gameCategory,
            GameType: gameType,
            SubGameProviderCode: gameInfo.SubGameProviderCode ?? gameInfo.GameProviderCode);
    }

    public IAsyncEnumerable<GameExternaldInfo> QueryByExternaldAsync(string gameProviderCode, CancellationToken cancellationToken = default)
    {
        gameProviderCode = gameProviderCode.ToUpper();

        return context.GameProviders
            .Where(predicate: g => g.Code == gameProviderCode)
            .Join(context.Games,
            gameProvider => gameProvider.Id,
            game => game.GameProviderId,
            (gameProvider, game) => new GameExternaldInfo(
                gameProvider.Id,
                game.ExternalId))
            .ToAsyncEnumerable();
    }
}
