using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordHandleCommandService : ICommandService<GameRecordHandleCommand>
{
    private readonly ICommandService<GameRecordDataPublishCommand> _dataPublishCommandService;
    private readonly IGetService<GameRecordInfoQuery, GameRecordInfoResult> _gameRecordService;
    private readonly ICommandService<GameRoundsWorkPublishCommand> _workPublishCommandService;

    public GameRecordHandleCommandService(
        IGetService<GameRecordInfoQuery, GameRecordInfoResult> gameRecordService,
        ICommandService<GameRoundsWorkPublishCommand> workPublishCommandService,
        ICommandService<GameRecordDataPublishCommand> dataPublishCommandService)
    {
        _gameRecordService = gameRecordService ?? throw new ArgumentNullException(nameof(gameRecordService));
        _workPublishCommandService = workPublishCommandService ?? throw new ArgumentNullException(nameof(workPublishCommandService));
        _dataPublishCommandService = dataPublishCommandService ?? throw new ArgumentNullException(nameof(dataPublishCommandService));
    }

    public async ValueTask ExecuteAsync(
        GameRecordHandleCommand command,
        CancellationToken cancellationToken = default)
    {
        var gameRecordInfoQuery = new GameRecordInfoQuery(
            GameProviderCode: command.GameProviderCode,
            ExternalRoundId: command.ExternalRoundId,
            ExternalGameId: command.ExternalGameId,
            PlayerCode: command.PlayerCode,
            Status: command.Status,
            ClientStatus: command.ClientStatus,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss);

        var gameRecordInfoResult = await _gameRecordService
            .GetAsync(
            query: gameRecordInfoQuery,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var workPublishCommand = new GameRoundsWorkPublishCommand(
            GameProviderId: gameRecordInfoResult.GameProviderId,
            GameProviderCode: gameRecordInfoResult.GameProviderCode,
            GameProviderConfigurationId: gameRecordInfoResult.GameProviderConfigurationId,
            SubGameProviderCode: gameRecordInfoResult.SubGameProviderCode,
            GameId: gameRecordInfoResult.GameId,
            GameType: gameRecordInfoResult.GameType,
            ResellerId: gameRecordInfoResult.ResellerId,
            ResellerCode: gameRecordInfoResult.ResellerCode,
            LicenseeId: gameRecordInfoResult.LicenseeId,
            LicenseeCode: gameRecordInfoResult.LicenseeCode,
            BrandId: gameRecordInfoResult.BrandId,
            BrandCode: gameRecordInfoResult.BrandCode,
            PlayerCode: gameRecordInfoResult.PlayerCode,
            Username: gameRecordInfoResult.Username,
            CurrencyCode: gameRecordInfoResult.CurrencyCode,
            WalletId: gameRecordInfoResult.WalletId,
            Status: gameRecordInfoResult.Status,
            ClientStatus: gameRecordInfoResult.ClientStatus,
            ExternalRoundId: gameRecordInfoResult.ExternalRoundId,
            ExternalGameId: gameRecordInfoResult.ExternalGameId,
            ExternalCreatedOn: gameRecordInfoResult.ExternalCreatedOn,
            ExternalClosedOn: gameRecordInfoResult.ExternalClosedOn,
            ExchangeRate: gameRecordInfoResult.ExchangeRate,
            Bet: gameRecordInfoResult.Bet,
            ValidBet: gameRecordInfoResult.ValidBet,
            Settle: gameRecordInfoResult.Settle,
            Commission: gameRecordInfoResult.Commission,
            Turnover: gameRecordInfoResult.Turnover,
            PlayerWinLoss: gameRecordInfoResult.PlayerWinLoss,
            AdditionalData: command.AdditionalData);

        await _workPublishCommandService
            .ExecuteAsync(
            workPublishCommand,
            cancellationToken)
            .ConfigureAwait(false);

        var dataPublishCommand = new GameRecordDataPublishCommand(
            GameProviderId: gameRecordInfoResult.GameProviderId,
            GameProviderCode: gameRecordInfoResult.GameProviderCode,
            GameProviderConfigurationId: gameRecordInfoResult.GameProviderConfigurationId,
            SubGameProviderCode: gameRecordInfoResult.SubGameProviderCode,
            GameId: gameRecordInfoResult.GameId,
            GameType: gameRecordInfoResult.GameType,
            ResellerId: gameRecordInfoResult.ResellerId,
            ResellerCode: gameRecordInfoResult.ResellerCode,
            LicenseeId: gameRecordInfoResult.LicenseeId,
            LicenseeCode: gameRecordInfoResult.LicenseeCode,
            BrandId: gameRecordInfoResult.BrandId,
            BrandCode: gameRecordInfoResult.BrandCode,
            PlayerCode: gameRecordInfoResult.PlayerCode,
            Username: gameRecordInfoResult.Username,
            CurrencyCode: gameRecordInfoResult.CurrencyCode,
            WalletId: gameRecordInfoResult.WalletId,
            Status: gameRecordInfoResult.Status,
            ClientStatus: gameRecordInfoResult.ClientStatus,
            ExternalRoundId: gameRecordInfoResult.ExternalRoundId,
            ExternalGameId: gameRecordInfoResult.ExternalGameId,
            ExternalCreatedOn: gameRecordInfoResult.ExternalCreatedOn,
            ExternalClosedOn: gameRecordInfoResult.ExternalClosedOn,
            ExchangeRate: gameRecordInfoResult.ExchangeRate,
            Bet: gameRecordInfoResult.Bet,
            ValidBet: gameRecordInfoResult.ValidBet,
            Settle: gameRecordInfoResult.Settle,
            Commission: gameRecordInfoResult.Commission,
            Turnover: gameRecordInfoResult.Turnover,
            PlayerWinLoss: gameRecordInfoResult.PlayerWinLoss,
            AdditionalData: command.AdditionalData);

        await _dataPublishCommandService
            .ExecuteAsync(
            dataPublishCommand,
            cancellationToken)
            .ConfigureAwait(false);
    }
}
