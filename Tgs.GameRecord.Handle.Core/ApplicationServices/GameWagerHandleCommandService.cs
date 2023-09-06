using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerHandleCommandService : ICommandService<GameWagerHandleCommand>
{
    private readonly IGetService<GameWagerInfoQuery, GameWagerInfoResult> _infoService;
    private readonly ICommandService<GameWagerPublishCommand> _publishService;

    public GameWagerHandleCommandService(
        IGetService<GameWagerInfoQuery, GameWagerInfoResult> infoService,
        ICommandService<GameWagerPublishCommand> publishService)
    {
        _infoService = infoService ?? throw new ArgumentNullException(nameof(infoService));
        _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
    }

    public async ValueTask ExecuteAsync(
        GameWagerHandleCommand command,
        CancellationToken cancellationToken = default)
    {
        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);

        var result = await _infoService
            .GetAsync(
            query: query,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var publishCommand = new GameWagerPublishCommand(
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        await _publishService
            .ExecuteAsync(
            publishCommand,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
