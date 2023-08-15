using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;

namespace Tgs.GameRecord.Handle.Client;

internal class GameRecordHandleClient : IGameRecordHandleClient
{
    private readonly IMessageSender _messageSender;

    public GameRecordHandleClient(IMessageSender messageSender)
    {
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public ValueTask MessageAsync(
        string gameProviderCode,
        string externalRoundId,
        string externalGameId,
        string playerCode,
        GameRecordStatus status,
        GameRecordClientStatus clientStatus,
        DateTimeOffset externalCreatedOn,
        DateTimeOffset? externalClosedOn,
        decimal bet,
        decimal validBet,
        decimal settle,
        decimal commission,
        decimal turnover,
        decimal playerWinLoss,
        string additionalData,
        CancellationToken cancellationToken)

    {
        var request = new GameRecordHandleRequest
        {
            GameProviderCode = gameProviderCode,
            ExternalRoundId = externalRoundId,
            ExternalGameId = externalGameId,
            PlayerCode = playerCode,
            Status = status,
            ClientStatus = clientStatus,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            PlayerWinLoss = playerWinLoss,
            AdditionalData = additionalData
        };
        return _messageSender
            .PublishAsync(
                subject: NatsConst.GameRecordHandlePublish,
                data: request.ToByteArray(),
                cancellationToken: cancellationToken);
    }
}
