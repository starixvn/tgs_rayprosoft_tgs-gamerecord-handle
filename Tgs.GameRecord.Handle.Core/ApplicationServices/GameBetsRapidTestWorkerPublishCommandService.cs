using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameBetsRapidTestWorkerPublishCommandService(
	ILogger<GameBetsRapidTestWorkerPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<GameBetsRapidTestWorkerPublishCommand>
{
	public ValueTask ExecuteAsync(GameBetsRapidTestWorkerPublishCommand command, CancellationToken cancellationToken = default)
	{
		var betState = command.BetState switch
		{
			GameBets.Common.Enums.BetState.Running => GameBets.RapidTest.Worker.Common.Protos.BetState.Running,
			GameBets.Common.Enums.BetState.Void => GameBets.RapidTest.Worker.Common.Protos.BetState.Void,
			GameBets.Common.Enums.BetState.Refund => GameBets.RapidTest.Worker.Common.Protos.BetState.Refund,
			GameBets.Common.Enums.BetState.Cancel => GameBets.RapidTest.Worker.Common.Protos.BetState.Cancel,
			GameBets.Common.Enums.BetState.Loss => GameBets.RapidTest.Worker.Common.Protos.BetState.Loss,
			GameBets.Common.Enums.BetState.Win => GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
			GameBets.Common.Enums.BetState.Draw => GameBets.RapidTest.Worker.Common.Protos.BetState.Draw,
			GameBets.Common.Enums.BetState.HalfWin => GameBets.RapidTest.Worker.Common.Protos.BetState.HalfWin,
			GameBets.Common.Enums.BetState.HalfLoss => GameBets.RapidTest.Worker.Common.Protos.BetState.HalfLoss,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetState.Waiting
		};

		var betType = command.BetType switch
		{
			GameBets.Common.Enums.BetType.Normal => GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetType.Normal
		};

		var freeGameState = command.FreeGameState switch
		{
			GameBets.Common.Enums.FreeGameState.FreeGame => GameBets.RapidTest.Worker.Common.Protos.FreeGameState.FreeGame,
			_ => GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None
		};

		var gambleState = command.GambleState switch
		{
			GameBets.Common.Enums.GambleState.Gamble => GameBets.RapidTest.Worker.Common.Protos.GambleState.Gamble,
			_ => GameBets.RapidTest.Worker.Common.Protos.GambleState.None
		};

		var cashoutState = command.CashoutState switch
		{
			GameBets.Common.Enums.CashoutState.Cashout => GameBets.RapidTest.Worker.Common.Protos.CashoutState.Cashout,
			_ => GameBets.RapidTest.Worker.Common.Protos.CashoutState.None
		};

		var resettleState = command.ResettleState switch
		{
			GameBets.Common.Enums.ResettleState.Resettle => GameBets.RapidTest.Worker.Common.Protos.ResettleState.Resettle,
			_ => GameBets.RapidTest.Worker.Common.Protos.ResettleState.None
		};

		var jackpotState = command.JackpotState switch
		{
			GameBets.Common.Enums.JackpotState.Jackpot => GameBets.RapidTest.Worker.Common.Protos.JackpotState.Jackpot,
			_ => GameBets.RapidTest.Worker.Common.Protos.JackpotState.None
		};

		var request = new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
		{
			GameProviderId = command.GameProviderId,
			GameProviderCode = command.GameProviderCode,
			GameProviderConfigurationId = command.GameProviderConfigurationId,
			SubGameProviderCode = command.SubGameProviderCode,
			GameId = command.GameId,
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
			ExternalRoundId = command.ExternalRoundId,
			ExternalBetId = command.ExternalBetId,
			ExternalGameId = command.ExternalGameId,
			ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
			ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
			ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
			ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
			BetState = betState,
			BetType = betType,
			FreeGameState = freeGameState,
			GambleState = gambleState,
			CashoutState = cashoutState,
			ResettleState = resettleState,
			JackpotState = jackpotState,
			CurrencyRatio = command.CurrencyRatio,
			Bet = command.Bet,
			ValidBet = command.ValidBet,
			Settle = command.Settle,
			Commission = command.Commission,
			Turnover = command.Turnover,
			WinLoss = command.WinLoss,
			Jackpot = command.Jackpot,
			AdditionalData = command.AdditionalData
		};

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message publishes request parameters.({request})",
			DateTime.Now.ToString("o"),
			GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest,
			new
			{
				request.GameProviderCode,
				request.SubGameProviderCode,
				request.GameId,
				request.ResellerCode,
				request.LicenseeCode,
				request.BrandCode,
				request.PlayerCode,
				request.Username,
				WalletId = request.WalletId.ToGuid(),
				request.ExternalRoundId,
				request.ExternalBetId,
				ExternalCreatedOn = request.ExternalCreatedOn.ToDateTimeOffset()
			});

		return messageSender.PublishAsync(
			subject: GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest,
			data: request,
			cancellationToken: cancellationToken);
	}
}
