using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerPublishRoundWorkCommandService : ICommandService<GameWagerPublishRoundWorkCommand>
{
    private readonly IMessageSender _sender;

    public GameWagerPublishRoundWorkCommandService(IMessageSender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(GameWagerPublishRoundWorkCommand command, CancellationToken cancellationToken = default)
    {
        var status = GameRecordStatus.Open;
        var clientStatus = GameRecordClientStatus.Open;
        if (command.WagerState is not WagerStates.Waiting and not WagerStates.Running)
        {
            clientStatus = command.WagerState switch
            {
                WagerStates.Win => GameRecordClientStatus.Win,
                WagerStates.Lose => GameRecordClientStatus.Loss,
                WagerStates.HalfWin => GameRecordClientStatus.Win,
                WagerStates.HalfLose => GameRecordClientStatus.Loss,
                _ => GameRecordClientStatus.Void,
            };
        }

        var data = new GameRecordPublish
        {
            GameProviderId = command.GameProviderId,
            GameProviderCode = command.GameProviderCode,
            GameProviderConfigurationId = command.GameProviderConfigurationId,
            SubGameProviderCode = command.SubGameProviderCode,
            GameId = command.GameId,
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
            Status = status,
            ClientStatus = clientStatus,
            WagerType = command.WagerType,
            ExternalRoundId = command.ExternalRoundId,
            ExternalWagerId = command.ExternalRoundId,
            ExternalGameId = command.ExternalGameId,
            ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
            CurrencyRatio = command.ExchangeRate,
            Bet = command.Bet,
            ValidBet = command.ValidBet,
            Settle = command.Settle,
            Commission = command.Commission,
            Turnover = command.Turnover,
            PlayerWinLoss = command.WinLoss,
            AdditionalData = command.AdditionalData,
            HandleOn = DateTimeOffset.UtcNow.ToTimestamp(),
            RapidTestOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            CreatedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            UpdatedOn = DateTimeOffset.UtcNow.ToTimestamp()
        };
        return _sender
            .PublishAsync(
            subject: NatsConst.GameRoundWorkerPublish,
            data: data.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
