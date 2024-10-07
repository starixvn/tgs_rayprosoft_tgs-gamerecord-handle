using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameBetsRapidTestWorkerPublishRepository(
	ILogger<GameBetsRapidTestWorkerPublishRepository> logger,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IGameBetsRapidTestWorkerClient client) : IGameBetsRapidTestWorkerPublishRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

	public async ValueTask PublishAsync(Core.Models.GameBetsRapidTestWorkerPublish source, CancellationToken cancellationToken = default)
	{
		var model = new GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel
		{
			GameProviderId = source.GameProviderId,
			GameProviderCode = source.GameProviderCode,
			GameProviderConfigurationId = source.GameProviderConfigurationId,
			SubGameProviderCode = source.SubGameProviderCode,
			GameId = source.GameId,
			ResellerId = source.ResellerId,
			ResellerCode = source.ResellerCode,
			LicenseeId = source.LicenseeId,
			LicenseeCode = source.LicenseeCode,
			BrandId = source.BrandId,
			BrandCode = source.BrandCode,
			PlayerCode = source.PlayerCode,
			Username = source.Username,
			CurrencyCode = source.CurrencyCode,
			WalletId = source.WalletId,
			ExternalRoundId = source.ExternalRoundId,
			ExternalBetId = source.ExternalBetId,
			ExternalGameId = source.ExternalGameId,
			ExternalCreatedOn = source.ExternalCreatedOn,
			ExternalClosedOn = source.ExternalClosedOn,
			ExternalPayoffOn = source.ExternalPayoffOn,
			ExternalResettleOn = source.ExternalResettleOn,
			BetState = source.BetState,
			BetType = source.BetType,
			FreeGameState = source.FreeGameState,
			GambleState = source.GambleState,
			CashoutState = source.CashoutState,
			ResettleState = source.ResettleState,
			JackpotState = source.JackpotState,
			CurrencyRatio = source.CurrencyRatio,
			Bet = source.Bet,
			ValidBet = source.ValidBet,
			Settle = source.Settle,
			WinLoss = source.WinLoss,
			Turnover = source.Turnover,
			Commission = source.Commission,
			Contribute = source.Contribute,
			Jackpot = source.Jackpot,
			AdditionalData = source.AdditionalData
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
							model.GameProviderCode,
							model.SubGameProviderCode,
							model.GameId,
							model.ResellerCode,
							model.LicenseeCode,
							model.BrandCode,
							model.PlayerCode,
							model.Username,
							model.CurrencyCode,
							model.WalletId,
							model.ExternalRoundId,
							model.ExternalBetId,
							model.ExternalGameId
						},
						exception.Message
					}));

		await retryPolicy.ExecuteAsync(() => client.HandleAsync(
			source: model,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}
}
