using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordHandleDataPublishCommandService : ICommandService<GameRecordHandleDataPublishCommand>
{
    private readonly ILogger<GameRecordHandleDataPublishCommandService> _logger;
    private readonly IMessageSender _sender;
    private readonly ISnowflakeWork _work;

    public GameRecordHandleDataPublishCommandService(
        ILogger<GameRecordHandleDataPublishCommandService> logger,
        ISnowflakeWork work,
        IMessageSender sender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(GameRecordHandleDataPublishCommand command, CancellationToken cancellationToken = default)
    {
        var wagerId = _work.NextId();
        var wagerState = WagerStates.Waiting;
        var wagerType = WagerTypes.Normal;
        var closedState = ClosedStates.None;
        var voidState = VoidStates.None;
        var cancelState = CancelStates.None;
        var freeGameState = FreeGameStates.None;
        var gambleState = GambleStates.None;
        var cashoutState = CashoutStates.None;
        var resettleState = ResettleStates.None;

        if (command.Status == GameRecordStatus.Open)
        {
            wagerState = WagerStates.Running;
        }
        if (command.Status == GameRecordStatus.Closed)
        {
            wagerState = command.ClientStatus switch
            {
                GameRecordClientStatus.New => WagerStates.Running,
                GameRecordClientStatus.Open => WagerStates.Running,
                GameRecordClientStatus.Void => WagerStates.Void,
                GameRecordClientStatus.Win => WagerStates.Win,
                GameRecordClientStatus.Loss => WagerStates.Lose,
                GameRecordClientStatus.Draw => WagerStates.Draw,
                _ => WagerStates.Waiting
            };
            closedState = ClosedStates.Closed;

            if (wagerState == WagerStates.Void)
            {
                voidState = VoidStates.Void;
            }

            if (wagerState == WagerStates.Reject)
            {
                cancelState = CancelStates.Cancel;
            }
        }

        var publish = new GameWagerPublish
        {
            WagerId = wagerId,
            GameProviderId = command.GameProviderId,
            GameProviderCode = command.GameProviderCode,
            GameProviderConfigurationId = command.GameProviderConfigurationId,
            SubGameProviderCode = command.SubGameProviderCode,
            GameId = command.GameId,
            GameCategory = command.GameCategory,
            GameType = command.GameType,
            ResellerId = command.ResellerId,
            ResellerCode = command.ResellerCode,
            LicenseeId = command.LicenseeId,
            LicenseeCode = command.LicenseeCode,
            BrandId = command.BrandId,
            BrandCode = command.BrandCode,
            PlayerCode = command.PlayerCode,
            Username = command.Username,
            CurrencyCode = command.CurrencyCode,
            WalletId = command.WalletId,
            WagerState = wagerState,
            WagerType = wagerType,
            ClosedState = closedState,
            VoidState = voidState,
            CancelState = cancelState,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = resettleState,
            ExternalRoundId = command.ExternalRoundId,
            ExternalWagerId = command.ExternalRoundId,
            ExternalGameId = command.ExternalGameId,
            ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
            ExternalPayoffOn = command.ExternalClosedOn.ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ExchangeRate = command.ExchangeRate,
            Bet = command.Bet,
            ValidBet = command.ValidBet,
            Settle = command.Settle,
            Commission = command.Commission,
            Turnover = command.Turnover,
            WinLoss = command.PlayerWinLoss,
            AdditionalData = command.AdditionalData,
            HandleOn = DateTimeOffset.UtcNow.ToTimestamp(),
            RapidTestOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            CreatedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            UpdatedOn = DateTimeOffset.UtcNow.ToTimestamp()
        };

        _logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This published message.({publish})",
            DateTime.Now.ToString("o"),
            NatsConst.GameRecordHandleDataPublish,
            publish);

        return _sender
            .PublishAsync(
            subject: NatsConst.GameRecordHandleDataPublish,
            data: publish.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
