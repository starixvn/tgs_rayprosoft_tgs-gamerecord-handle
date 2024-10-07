using System.Text.Json;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameRecordHandleMessageHandler(
	ILogger<GameRecordHandleMessageHandler> logger,
	IGameRecordDispatcher dispatcher) : IMessageHandler<Common.Protos.GameRecordHandleRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameRecordHandleRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.GameProviderCode,
					data.PlayerCode,
					data.ExternalRoundId,
					data.ExternalBetId,
					data.ExternalGameId,
					ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset().ToString("u"),
					ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset().ToString("u"),
					data.BetState
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushCommandAsync(
		   command: new GameRecordCommandRequest(
			   GameProviderCode: data.GameProviderCode,
			   PlayerCode: data.PlayerCode,
			   ExternalRoundId: data.ExternalRoundId,
			   ExternalBetId: data.ExternalBetId,
			   ExternalGameId: data.ExternalGameId,
			   ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset(),
			   ExternalClosedOn: data.ExternalClosedOn.ToDateTimeOffset(),
			   ExternalPayoffOn: data.ExternalPayoffOn.ToDateTimeOffset(),
			   ExternalResettleOn: data.ExternalResettleOn.ToDateTimeOffset(),
			   BetState: data.BetState,
			   BetType: data.BetType,
			   FreeGameState: data.FreeGameState,
			   GambleState: data.GambleState,
			   CashoutState: data.CashoutState,
			   ResettleState: data.ResettleState,
			   JackpotState: data.JackpotState,
			   Bet: data.Bet,
			   ValidBet: data.ValidBet,
			   Settle: data.Settle,
			   WinLoss: data.WinLoss,
			   Turnover: data.Turnover,
			   Commission: data.Commission,
			   Contribute: data.Contribute,
			   Jackpot: data.Jackpot,
			   AdditionalData: data.AdditionalData),
		   cancellationToken: cancellationToken);
	}
}
