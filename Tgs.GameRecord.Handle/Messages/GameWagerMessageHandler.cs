using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameWagerMessageHandler(
	ILogger<GameWagerMessageHandler> logger,
	ICommandService<GameWagerHandleCommand> service) : IMessageHandler<Common.Protos.GameWagerRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameWagerRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				data.GameProviderCode,
				data.PlayerCode,
				data.ExternalRoundId,
				data.ExternalWagerId,
				data.ExternalGameId,
				ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset(),
				data.WagerState
			});

		try
		{
			return service.ExecuteAsync(
				command: new GameWagerHandleCommand(
					GameProviderCode: data.GameProviderCode,
					PlayerCode: data.PlayerCode,
					ExternalRoundId: data.ExternalRoundId,
					ExternalWagerId: data.ExternalWagerId,
					ExternalGameId: data.ExternalGameId,
					ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset(),
					ExternalClosedOn: data.ExternalClosedOn.ToDateTimeOffset(),
					ExternalPayoffOn: data.ExternalPayoffOn.ToDateTimeOffset(),
					ExternalResettleOn: data.ExternalResettleOn.ToDateTimeOffset(),
					WagerState: data.WagerState,
					WagerType: data.WagerType,
					FreeGameState: data.FreeGameState,
					GambleState: data.GambleState,
					CashoutState: data.CashoutState,
					ResettleState: data.ResettleState,
					Bet: data.Bet,
					ValidBet: data.ValidBet,
					Settle: data.Settle,
					Commission: data.Commission,
					Turnover: data.Turnover,
					WinLoss: data.WinLoss,
					Jackpot: 0.0m,
					AdditionalData: data.AdditionalData),
				cancellationToken: cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
				DateTime.Now.ToString("o"),
				subject,
				new
				{
					data.GameProviderCode,
					data.PlayerCode,
					data.ExternalRoundId,
					data.ExternalWagerId,
					data.ExternalGameId,
					ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
					ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset(),
					data.WagerState
				});

			throw;
		}
	}
}
