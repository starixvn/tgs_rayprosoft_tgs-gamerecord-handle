using System.Text.Json.Nodes;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Client;

internal class GameRecordHandleClient(IMessageSender messageSender) : IGameRecordHandleClient
{
    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, DateTimeOffset? externalPayoffOn, DateTimeOffset? externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, JsonNode additionalData, CancellationToken cancellationToken = default)
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
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset externalClosedOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, JsonNode additionalData, CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = WagerTypes.Normal,
            FreeGameState = FreeGameStates.None,
            GambleState = GambleStates.None,
            CashoutState = CashoutStates.None,
            ResettleState = ResettleStates.None,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = externalClosedOn.ToTimestamp(),
            ExternalPayoffOn = externalClosedOn.ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset externalClosedOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, JsonNode additionalData, CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = WagerTypes.Normal,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = ResettleStates.None,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = externalClosedOn.ToTimestamp(),
            ExternalPayoffOn = externalClosedOn.ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset externalClosedOn, DateTimeOffset externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, JsonNode additionalData, CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = WagerTypes.Normal,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = resettleState,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = externalClosedOn.ToTimestamp(),
            ExternalPayoffOn = externalClosedOn.ToTimestamp(),
            ExternalResettleOn = externalResettleOn.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset externalClosedOn, DateTimeOffset externalPayoffOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, JsonNode additionalData, CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = wagerState,
            WagerType = WagerTypes.Normal,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = cashoutState,
            ResettleState = ResettleStates.None,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = externalClosedOn.ToTimestamp(),
            ExternalPayoffOn = externalPayoffOn.ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = settle,
            Commission = commission,
            Turnover = turnover,
            WinLoss = winLoss,
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }

    public ValueTask MessageRunningAsync(string gameProviderCode, string playerCode, FreeGameStates freeGameState, GambleStates gambleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, decimal bet, decimal validBet, JsonNode additionalData, CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            GameProviderCode = gameProviderCode,
            PlayerCode = playerCode,
            WagerState = WagerStates.Running,
            WagerType = WagerTypes.Normal,
            FreeGameState = freeGameState,
            GambleState = gambleState,
            CashoutState = CashoutStates.None,
            ResettleState = ResettleStates.None,
            ExternalRoundId = externalRoundId,
            ExternalWagerId = externalWagerId,
            ExternalGameId = externalGameId,
            ExternalCreatedOn = externalCreatedOn.ToTimestamp(),
            ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            Bet = bet,
            ValidBet = validBet,
            Settle = 0.0m,
            Commission = 0.0m,
            Turnover = 0.0m,
            WinLoss = 0.0m,
            AdditionalData = additionalData.ToJsonString()
        };

        return messageSender.PublishAsync<ReadOnlyMemory<byte>>(
            subject: NatsConst.GameRecordWagerPublish,
            data: request.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
