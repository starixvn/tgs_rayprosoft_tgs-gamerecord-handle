using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameBetsRapidTestWorkerPublishCommandService(
	ILogger<GameBetsRapidTestWorkerPublishCommandService> logger,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IGameBetsRapidTestWorkerClient client) : ICommandService<GameBetsRapidTestWorkerPublishCommand>
{
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

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

		var source = new GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel
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
			ExternalCreatedOn = command.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			ExternalPayoffOn = command.ExternalPayoffOn,
			ExternalResettleOn = command.ExternalResettleOn,
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

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "{logWarning}",
					new
					{
						LogOn = DateTime.Now.ToString("u"),
						RetryTime = retryTime,
						RetryCount = retryCount,
						Data = new
						{
							source.GameProviderCode,
							source.SubGameProviderCode,
							source.GameId,
							source.ResellerCode,
							source.LicenseeCode,
							source.BrandCode,
							source.PlayerCode,
							source.Username,
							source.CurrencyCode,
							source.WalletId,
							source.ExternalRoundId,
							source.ExternalBetId,
							source.ExternalGameId
						},
						exception.Message
					}));

		return client.HandleAsync(
			source: source,
			cancellationToken: cancellationToken);
	}
}
