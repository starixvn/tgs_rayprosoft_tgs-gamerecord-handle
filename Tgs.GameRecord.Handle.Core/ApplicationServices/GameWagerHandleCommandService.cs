using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerHandleCommandService : ICommandService<GameWagerHandleCommand>
{
    private readonly IGetService<GameWagerInfoQuery, GameWagerInfoResult> _infoService;
    private readonly ICommandService<GameWagerPublishCommand> _publishService;
    private readonly ISnowflakeWork _work;

    public GameWagerHandleCommandService(
        IGetService<GameWagerInfoQuery, GameWagerInfoResult> infoService,
        ISnowflakeWork work,
        ICommandService<GameWagerPublishCommand> publishService)
    {
        _infoService = infoService ?? throw new ArgumentNullException(nameof(infoService));
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
    }

    public async ValueTask ExecuteAsync(
        GameWagerHandleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.ExternalRoundId is null || string.IsNullOrWhiteSpace(command.ExternalRoundId))
        {
            throw new ArgumentException($"This ExternalRoundId is empty/null .({new
            {
                command.GameProviderCode,
                command.ExternalRoundId,
                command.ExternalWagerId,
                command.PlayerCode,
                command.ExternalGameId
            }})");
        }

        if (command.ExternalWagerId is null || string.IsNullOrWhiteSpace(command.ExternalWagerId))
        {
            throw new ArgumentException($"This ExternalWagerId is empty/null .({new
            {
                command.GameProviderCode,
                command.ExternalRoundId,
                command.ExternalWagerId,
                command.PlayerCode,
                command.ExternalGameId
            }})");
        }
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

        var result = await _infoService.GetAsync(
            query: query,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var wagerId = _work.GetWagerId();

        var closedState = ClosedStates.None;
        var voidState = VoidStates.None;
        var cancelState = CancelStates.None;
        if (command.WagerState is not WagerStates.Waiting and not WagerStates.Running)
        {
            closedState = ClosedStates.Closed;

            if (command.WagerState == WagerStates.Void)
            {
                voidState = VoidStates.Void;
            }

            if (command.WagerState == WagerStates.Reject)
            {
                cancelState = CancelStates.Cancel;
            }
        }

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
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
            ClosedState: closedState,
            VoidState: voidState,
            CancelState: cancelState,
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

        await _publishService.ExecuteAsync(
            command: publishCommand,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
