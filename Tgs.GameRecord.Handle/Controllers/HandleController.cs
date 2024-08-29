using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.Handle.Core;

using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Controllers;

[Route("[controller]")]
[ApiController]
public class HandleController : ControllerBase
{
	/// <summary>
	/// Handles the asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"gameProviderCode": "LY",
	///				"playerCode": "4un4b9l34304",
	///				"externalRoundId": "888-1722937124-92240583-21",
	///				"externalBetId": "888-1722937124-92240583-21",
	///				"externalGameId": "910",
	///				"externalCreatedOn": "2024-08-06T17:38:44+08:00",
	///				"externalClosedOn": "2024-08-06T17:39:03+08:00",
	///				"externalPayoffOn": "2024-08-06T17:39:03+08:00",
	///				"externalResettleOn": "1970-01-01T00:00:00+00:00",
	///				"betState": "Win",
	///				"betType": "Normal",
	///				"freeGameState": "None",
	///				"gambleState": "None",
	///				"cashoutState": "None",
	///				"resettleState": "None",
	///				"jackpotState": "None",
	///				"currencyRatio": 1,
	///				"bet": 2,
	///				"validBet": 2,
	///				"settle": 4,
	///				"commission": 0,
	///				"turnover": 2,
	///				"winLoss": 2,
	///				"jackpot": 0,
	///				"additionalData": "{\"AllBet\": 2, \"GameID\": \"888-1722937124-92240583-21\", \"KindID\": 910, \"Profit\": 2, \"ChairID\": 0, \"Revenue\": 0, \"TableID\": 182020001, \"Accounts\": \"200995_4un4b9l34304\", \"LineCode\": \"200995_GPI\", \"ServerID\": 9101, \"CardValue\": \"0c32050b06001\", \"CellScore\": 2, \"ChannelID\": 200995, \"UserCount\": 1, \"GameEndTime\": \"2024-08-06T17:39:03+08:00\", \"GameStartTime\": \"2024-08-06T17:38:44+08:00\"}"
	///			}
	/// </remarks>
	[HttpPost]
	public async ValueTask HandleAsync(
		[FromServices] IMessageSender messageSender,
		[FromBody] Common.ViewModels.GameRecordHandleViewModel source)
	{
		var tasks = new Task[]{
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameRecordHandleNatsConst.GameRecordHandleRequest,
				data: new Common.Protos.GameRecordHandleRequest
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
					AdditionalData = source.AdditionalData
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
			Task.Run( ()=>messageSender.PublishAsync(
				subject: GameRecordHandleNatsConst.GameRecordHandleExpiryRequest,
				data: new Common.Protos.GameRecordHandleExpiryRequest
				{
					GameProviderCode = source.GameProviderCode,
					ExternalGameId = source.ExternalGameId
				},
				cancellationToken: HttpContext.RequestAborted).AsTask()),
		};

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}
}
