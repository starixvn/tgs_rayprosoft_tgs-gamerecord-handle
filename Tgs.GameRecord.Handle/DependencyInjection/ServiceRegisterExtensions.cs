using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegisterExtensions
{
    public static async Task<WebApplication> UseRegisterAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var client = scope.ServiceProvider.GetRequiredService<IGameCommandClient>();
            var gameProvider = scope.ServiceProvider.GetRequiredService<IGameProvider>();
            var game = scope.ServiceProvider.GetRequiredService<IGame>();
            var gameProviderInfos = await client
                .GameProviderQueryAsync()
                .ToArrayAsync()
                .ConfigureAwait(false)
                ?? Enumerable.Empty<GameProviderInfo>();

            foreach (var gameProviderInfo in gameProviderInfos)
            {
                try
                {
                    var gameProviderId = await gameProvider
                        .GetIdAsync(
                        code: gameProviderInfo.GameProviderCode)
                        .ConfigureAwait(false);

                    var gameInfos = await client
                        .GameQueryAsync(
                        gameProviderCode: gameProviderInfo.GameProviderCode)
                        .ToArrayAsync()
                        .ConfigureAwait(false)
                        ?? Enumerable.Empty<GameInfo>();

                    foreach (var gameInfo in gameInfos)
                    {
                        try
                        {
                            _ = await game
                                .GetAsync(
                                gameProviderId: gameProviderId,
                                externaGamelId: gameInfo.GameExternalId)
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
