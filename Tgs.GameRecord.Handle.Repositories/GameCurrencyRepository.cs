using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameCurrency.Client;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameCurrencyRepository(
	ILogger<GameCurrencyRepository> logger,
	IOptions<PolicySettingsOptions> options,
	IGameCurrencyClient client) : IGameCurrencyRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<GameCurrencyInfo> GetAsync(string gameProviderCode, string currencyCode, decimal bet, decimal validBet, decimal settle, decimal winLoss, decimal turnover, decimal commission, decimal contribute, decimal jackpot, CancellationToken cancellationToken = default)
	{
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
							GameProviderCode = gameProviderCode,
							CurrencyCode = currencyCode
						},
						exception.Message
					}));

		var result = await retryPolicy.ExecuteAsync(() => client.ConvertPlatformRoundsAsync(
			gameProviderCode: gameProviderCode,
			currencyCode: currencyCode,
			bet: bet,
			validBet: validBet,
			settle: settle,
			winLoss: winLoss,
			turnover: turnover,
			commission: commission,
			contribute: contribute,
			jackpot: jackpot,
			cancellationToken).AsTask())
			.ConfigureAwait(false);

		return new GameCurrencyInfo(
			CurrencyRatio: result.ExchangeRate,
			Bet: result.Bet,
			ValidBet: result.ValidBet,
			Settle: result.Settle,
			WinLoss: result.WinLoss,
			Turnover: result.Turnover,
			Commission: result.Commission,
			Contribute: result.Contribute,
			Jackpot: result.Jackpot);
	}
}
