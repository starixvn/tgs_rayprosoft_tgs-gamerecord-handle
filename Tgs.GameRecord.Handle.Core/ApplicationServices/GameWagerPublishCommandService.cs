using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerPublishCommandService(
    ILogger<GameWagerPublishCommandService> logger,
    IMessageSender messageSender) : ICommandService<GameWagerPublishCommand>
{
    public ValueTask ExecuteAsync(GameWagerPublishCommand command, CancellationToken cancellationToken = default)
    {
        var publish = new GameWagerPublish
        {
            WagerId = command.WagerId,
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
            ClosedState = command.ClosedState,
            VoidState = command.VoidState,
            CancelState = command.CancelState,
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
            CurrencyRatio = command.CurrencyRatio,
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
            UpdatedOn = DateTimeOffset.UtcNow.ToTimestamp(),
            ResetState = ResetStates.None,
            ResetOn = DateTimeOffset.UnixEpoch.ToTimestamp()
        };

        logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This published message.({publish})",
            DateTime.Now.ToString("o"),
            NatsConst.GameRecordHandleDataPublish,
            publish);

        return messageSender.PublishAsync(
            subject: NatsConst.GameRecordHandleDataPublish,
            data: publish.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
