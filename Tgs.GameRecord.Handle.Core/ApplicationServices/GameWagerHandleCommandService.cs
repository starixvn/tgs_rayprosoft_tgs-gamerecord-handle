using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerHandleCommandService(
    ISystemClock systemClock,
    IGetService<GameWagerInfoQuery, GameWagerInfoResult> infoService,
    ISnowflakeWork snowflakeWork,
    ICommandService<GameWagerPublishCommand> publishService) : ICommandService<GameWagerHandleCommand>
{
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

        
        if (command.WagerState is not WagerStates.Waiting and not WagerStates.Running)
        {
            var currentTime = systemClock.CurrentTime;
            var externalClosedOn = command.ExternalClosedOn;
            var diff = currentTime.Subtract(externalClosedOn);
            if (diff.TotalDays > 100)
            {
                throw new Exception($"This game bet will take more than 100 days to settle.({new
                {
                    command.GameProviderCode,
                    command.ExternalRoundId,
                    command.ExternalWagerId,
                    command.PlayerCode,
                    command.ExternalGameId,
                    command.ExternalCreatedOn,
                    command.ExternalClosedOn,
                    systemClock.CurrentTime,
                }})");
            }
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

        var result = await infoService.GetAsync(
            query: query,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var wagerId = snowflakeWork.GetId();

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

        await publishService.ExecuteAsync(
           command: publishCommand,
           cancellationToken: cancellationToken)
           .ConfigureAwait(false);
    }
}
