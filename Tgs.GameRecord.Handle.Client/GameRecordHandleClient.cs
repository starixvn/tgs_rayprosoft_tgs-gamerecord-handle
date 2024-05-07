using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Client.Models;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Client;

internal class GameRecordHandleClient(
	IMessageSender messageSender) : IGameRecordHandleClient
{
	public ValueTask RequestPublishAsyncAsync(GameRecordRequest source, JsonNode additionalData, CancellationToken cancellationToken = default)
	{
		var request = new Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = source.GameProviderCode,
			PlayerCode = source.PlayerCode,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalGameId = source.ExternalGameId,
			ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
			ExternalClosedOn = source.ExternalClosedOn.ToTimestamp(),
			ExternalPayoffOn = source.ExternalPayoffOn.ToTimestamp(),
			ExternalResettleOn = source.ExternalResettleOn.ToTimestamp(),
			BetState = source.BetState,
			BetType = source.BetType,
			FreeGameState = source.FreeGameState,
			GambleState = source.GambleState,
			CashoutState = source.CashoutState,
			ResettleState = source.ResettleState,
			JackpotState = source.JackpotState,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			Jackpot = source.Jackpot,
			AdditionalData = additionalData.ToJsonString()
		};

		return messageSender.PublishAsync(
			subject: Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken);
	}

	public ValueTask RequestPublishAsyncAsync(GameRecordRunningRequest source, JsonNode additionalData, CancellationToken cancellationToken = default)
	{
		var request = new Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = source.GameProviderCode,
			PlayerCode = source.PlayerCode,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalGameId = source.ExternalGameId,
			ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
			ExternalClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			ExternalPayoffOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = Common.Protos.BetState.Running,
			BetType = source.BetType,
			FreeGameState = Common.Protos.FreeGameState.None,
			GambleState = Common.Protos.GambleState.None,
			CashoutState = Common.Protos.CashoutState.None,
			ResettleState = Common.Protos.ResettleState.None,
			JackpotState = Common.Protos.JackpotState.None,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = 0.0m,
			Commission = 0.0m,
			Turnover = 0.0m,
			WinLoss = 0.0m,
			Jackpot = 0.0m,
			AdditionalData = additionalData.ToJsonString()
		};

		return messageSender.PublishAsync(
			subject: Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken);
	}

	public ValueTask RequestPublishAsyncAsync(GameRecordClosedRequest source, JsonNode additionalData, CancellationToken cancellationToken = default)
	{
		var request = new Common.Protos.GameRecordHandleRequest
		{
			GameProviderCode = source.GameProviderCode,
			PlayerCode = source.PlayerCode,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalGameId = source.ExternalGameId,
			ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
			ExternalClosedOn = source.ExternalClosedOn.ToTimestamp(),
			ExternalPayoffOn = source.ExternalClosedOn.ToTimestamp(),
			ExternalResettleOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
			BetState = source.BetState,
			BetType = source.BetType,
			FreeGameState = source.FreeGameState,
			GambleState = source.GambleState,
			CashoutState = source.CashoutState,
			ResettleState = Common.Protos.ResettleState.None,
			JackpotState = source.JackpotState,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			Commission = source.Commission,
			Turnover = source.Turnover,
			WinLoss = source.WinLoss,
			Jackpot = source.Jackpot,
			AdditionalData = additionalData.ToJsonString()
		};

		return messageSender.PublishAsync(
			subject: Common.GameRecordHandleNatsConst.GameRecordHandleRequest,
			data: request,
			cancellationToken: cancellationToken);
	}
}
