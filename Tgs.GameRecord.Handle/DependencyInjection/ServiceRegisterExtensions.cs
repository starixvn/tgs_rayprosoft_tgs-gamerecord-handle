using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
	public static async Task<WebApplication> UseRegisterAsync(this WebApplication app)
	{
		using (var scope = app.Services.CreateScope())
		{
			var gameProvider = scope.ServiceProvider.GetRequiredService<IGameProviderRepository>();
			var game = scope.ServiceProvider.GetRequiredService<IGameRepository>();

			var gameProviderCodes = await gameProvider.QueryByCodeAsync()
				.ToArrayAsync()
				.ConfigureAwait(false)
				?? Enumerable.Empty<string>();

			foreach (var gameProviderCode in gameProviderCodes)
			{
				try
				{
					var gameProviderId = await gameProvider
						.GetIdAsync(
						code: gameProviderCode)
						.ConfigureAwait(false);

					var externalGameIdInfos = await game
						.QueryByExternaldAsync(
						gameProviderCode: gameProviderCode)
						.ToArrayAsync()
						.ConfigureAwait(false);

					if (externalGameIdInfos.Length == 0)
					{
						continue;
					}

					foreach (var (GameProviderId, ExternalGameId) in externalGameIdInfos)
					{
						try
						{
							_ = await game.GetAsync(
								gameProviderId: GameProviderId,
								externalGameId: ExternalGameId)
								.ConfigureAwait(false);
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
		}

		return app;
	}
}
