using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameRepository(
	ILogger<GameRepository> logger,
	IOptions<PolicySettingsOptions> options,
	TgsContext context) : IGameRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId, CancellationToken cancellationToken = default)
	{
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

		return await retryPolicy.ExecuteAsync(() => context.Games
			.Where(predicate: g => g.GameProviderId == gameProviderId && g.ExternalId == externalGameId)
			.Join(context.GameProviders,
			game => game.GameProviderId,
			gameProvider => gameProvider.Id,
			(game, gameProvider) => new GameInfo(
				game.Id,
				game.Code,
				game.GameProviderId,
				gameProvider.Code,
				game.ExternalId,
				game.SubGameProviderCode ?? gameProvider.Code))
			.FirstOrDefaultAsync(
			cancellationToken: cancellationToken))
			.ConfigureAwait(false);
	}

	public IAsyncEnumerable<GameExternaldInfo> QueryByExternaldAsync(string gameProviderCode, CancellationToken cancellationToken = default)
	{
		gameProviderCode = gameProviderCode.ToUpper();

		return context.GameProviders
			.Where(predicate: g => g.Code == gameProviderCode)
			.Join(context.Games,
			gameProvider => gameProvider.Id,
			game => game.GameProviderId,
			(gameProvider, game) => new GameExternaldInfo(
				gameProvider.Id,
				game.ExternalId))
			.ToAsyncEnumerable();
	}
}
