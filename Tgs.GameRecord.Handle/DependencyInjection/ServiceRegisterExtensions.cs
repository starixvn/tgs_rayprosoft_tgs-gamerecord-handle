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
            var gameProviderIdInfos = await client
                .GameProviderGetGameProviderIdsAsync()
                .ToArrayAsync()
                .ConfigureAwait(false)
                ?? Enumerable.Empty<GameProviderIdInfo>();

            foreach (var gameProviderIdInfo in gameProviderIdInfos)
            {
                try
                {
                    var gameProviderId = await gameProvider
                        .GetIdAsync(
                        code: gameProviderIdInfo.GameProviderCode)
                        .ConfigureAwait(false);

                    var gameInfos = await client
                        .GameQueryAsync(
                        gameProviderCode: gameProviderIdInfo.GameProviderCode)
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
