using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerInfoGetService : IGetService<GameWagerInfoQuery, GameWagerInfoResult>
{
    private readonly IBrandGameProvider _brandGameProvider;
    private readonly IGame _game;
    private readonly IGameCurrency _gameCurrency;
    private readonly IGameProvider _gameProvider;
    private readonly IPlayer _player;

    public GameWagerInfoGetService(
        IBrandGameProvider brandGameProvider,
        IGame game,
        IGameProvider gameProvider,
        IPlayer player,
        IGameCurrency gameCurrency)
    {
        _brandGameProvider = brandGameProvider ?? throw new ArgumentNullException(nameof(brandGameProvider));
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _gameCurrency = gameCurrency ?? throw new ArgumentNullException(nameof(gameCurrency));
    }

    public async ValueTask<GameWagerInfoResult> GetAsync(GameWagerInfoQuery query, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetIdAsync(
            code: query.GameProviderCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var player = await _player.GetAsync(
            playerCode: query.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderConfigurationId = await _brandGameProvider.GetGameProviderConfigurationIdAsync(
            brandId: player.BrandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var game = await _game.GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: query.ExternalGameId,
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

        var gameCurrencyResult = await _gameCurrency.GetAsync(
            gameProviderCode: game.GameProviderCode,
            currencyCode: player.DefaultWalletCurrencyCode,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: winLoss,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameWagerInfoResult(
            GameProviderId: game.GameProviderId,
            GameProviderCode: game.GameProviderCode,
            GameProviderConfigurationId: gameProviderConfigurationId,
            SubGameProviderCode: game.SubGameProviderCode,
            GameId: game.GameId,
            GameCategory: game.GameCategory,
            GameType: game.GameType,
            ResellerId: player.ResellerId,
            ResellerCode: player.ResellerCode,
            LicenseeId: player.LicenseeId,
            LicenseeCode: player.LicenseeCode,
            BrandId: player.BrandId,
            BrandCode: player.BrandCode,
            PlayerCode: player.PlayerCode,
            Username: player.Username,
            CurrencyCode: player.DefaultWalletCurrencyCode,
            WalletId: player.DefaultWalletId,
            CurrencyRatio: gameCurrencyResult.CurrencyRatio,
            Bet: gameCurrencyResult.Bet,
            ValidBet: gameCurrencyResult.ValidBet,
            Settle: gameCurrencyResult.Settle,
            Commission: gameCurrencyResult.Commission,
            Turnover: gameCurrencyResult.Turnover,
            WinLoss: gameCurrencyResult.PlayerWinLoss);
    }
}
