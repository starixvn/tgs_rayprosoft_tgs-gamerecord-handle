using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameProviderRepository(
	ILogger<GameProviderRepository> logger,
	IOptions<PolicySettingsOptions> options,
	TgsContext context) : IGameProviderRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<Guid> GetIdAsync(string code, CancellationToken cancellationToken = default)
	{
		code = code.ToUpper();

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<NpgsqlException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn}  RetryTime:{retryTime} RetryCount:{retryCount}",
					DateTime.Now.ToString("o"),
					retryTime,
					retryCount));

		return await retryPolicy.ExecuteAsync(() => context.GameProviders
			.Where(gp => gp.Code == code)
			.Select(gp => (Guid?)gp.Id)
			.FirstOrDefaultAsync(
			cancellationToken: cancellationToken))
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(code, $"This game provider does not exist.({code})");
	}

	public IAsyncEnumerable<string> QueryByCodeAsync(CancellationToken cancellationToken = default)
		=> context.GameProviders
		.Select(selector: gp => gp.Code)
		.ToAsyncEnumerable();
}
