using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerInfoGetService(
    IBrandGameProviderRepository brandGameProvider,
    IGameRepository game,
    IGameProviderRepository gameProvider,
    IPlayerRepository player,
    IGameCurrency gameCurrency) : IGetService<GameWagerInfoQuery, GameWagerInfoResult>
{
    public async ValueTask<GameWagerInfoResult> GetAsync(GameWagerInfoQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await gameProvider.GetIdAsync(
            code: query.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerInfo = await player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await brandGameProvider.GetGameProviderConfigurationIdAsync(
            brandId: playerInfo.BrandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameModel = await game.GetAsync(
            gameProviderId: gameProviderId,
            externalGameId: query.ExternalGameId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException($"This game does not exist.({new
            {
                query.GameProviderCode,
                query.PlayerCode,
                query.ExternalGameId
            }})");

        var bet = query.Bet;
        var validBet = query.ValidBet;
        var settle = 0.0m;
        var commission = 0.0m;
        var turnover = 0.0m;
        var winLoss = 0.0m;

        if (query.WagerState is not WagerStates.Waiting and not WagerStates.Running)
        {
            settle = query.Settle;
            commission = query.Commission;
            turnover = query.Turnover;
            winLoss = query.WinLoss;
        }

        var gameCurrencyResult = await gameCurrency.GetAsync(
            gameProviderCode: gameModel.GameProviderCode,
            currencyCode: playerInfo.DefaultWalletCurrencyCode,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: winLoss,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameWagerInfoResult(
            GameProviderId: gameModel.GameProviderId,
            GameProviderCode: gameModel.GameProviderCode,
            GameProviderConfigurationId: gameProviderConfigurationId,
            SubGameProviderCode: gameModel.SubGameProviderCode,
            GameId: gameModel.GameId,
            GameCategory: gameModel.GameCategory,
            GameType: gameModel.GameType,
            ResellerId: playerInfo.ResellerId,
            ResellerCode: playerInfo.ResellerCode,
            LicenseeId: playerInfo.LicenseeId,
            LicenseeCode: playerInfo.LicenseeCode,
            BrandId: playerInfo.BrandId,
            BrandCode: playerInfo.BrandCode,
            PlayerCode: playerInfo.PlayerCode,
            Username: playerInfo.Username,
            CurrencyCode: playerInfo.DefaultWalletCurrencyCode,
            WalletId: playerInfo.DefaultWalletId,
            CurrencyRatio: gameCurrencyResult.CurrencyRatio,
            Bet: gameCurrencyResult.Bet,
            ValidBet: gameCurrencyResult.ValidBet,
            Settle: gameCurrencyResult.Settle,
            Commission: gameCurrencyResult.Commission,
            Turnover: gameCurrencyResult.Turnover,
            WinLoss: gameCurrencyResult.PlayerWinLoss);
    }
}
