using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Client;

internal class GameRecordHandleClient : IGameRecordHandleClient
{
    private readonly IMessageSender _messageSender;

    public GameRecordHandleClient(IMessageSender messageSender)
    {
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, DateTimeOffset? externalPayoffOn, DateTimeOffset? externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = wagerType,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = resettleState,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalPayoffOn = (externalPayoffOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalResettleOn = (externalResettleOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData
        };

        return _messageSender.PublishAsync(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, DateTimeOffset? externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = wagerType,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = resettleState,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalPayoffOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalResettleOn = (externalResettleOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData
        };

        return _messageSender.PublishAsync(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = wagerType,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = ResettleStates.None,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalPayoffOn = (externalClosedOn ?? DateTimeOffset.UnixEpoch).ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData
        };

        return _messageSender.PublishAsync(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
