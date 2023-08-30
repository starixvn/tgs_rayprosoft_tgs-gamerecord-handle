using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerPublishCommandService : ICommandService<GameWagerPublishCommand>
{
    private readonly ILogger<GameWagerPublishCommandService> _logger;
    private readonly IMessageSender _sender;
    private readonly ISnowflakeWork _work;

    public GameWagerPublishCommandService(
        ILogger<GameWagerPublishCommandService> logger,
        ISnowflakeWork work,
        IMessageSender sender)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(GameWagerPublishCommand command, CancellationToken cancellationToken = default)
    {
        var wagerId = _work.NextId();

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
            WagerState = command.WagerState,
            WagerType = command.WagerType,
            ClosedState = closedState,
            VoidState = voidState,
            CancelState = cancelState,
            FreeGameState = command.FreeGameState,
            GambleState = command.GambleState,
            CashoutState = command.CashoutState,
            ResettleState = command.ResettleState,
            ExternalRoundId = command.ExternalRoundId,
            ExternalWagerId = command.ExternalWagerId,
            ExternalGameId = command.ExternalGameId,
            ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
            ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
            ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
            ExchangeRate = command.ExchangeRate,
            Bet = command.Bet,
            ValidBet = command.ValidBet,
            Settle = command.Settle,
            Commission = command.Commission,
            Turnover = command.Turnover,
            WinLoss = command.WinLoss,
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
