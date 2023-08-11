using Tgs.GameCommand.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandleRepository(
        this IServiceCollection services,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions)
        => services
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameCurrencyClient(gameCurrencyClientOptions)
        .AddScoped<IBrandGameProvider>(sp => ActivatorUtilities.CreateInstance<BrandGameProviderRepository>(sp, TimeSpan.FromMinutes(5)))
        .AddScoped<IGameCurrency, GameCurrencyRepository>()
        .AddScoped<IGameProvider>(sp => ActivatorUtilities.CreateInstance<GameProviderRepository>(sp, TimeSpan.FromHours(1)))
        .AddScoped<IGame>(sp => ActivatorUtilities.CreateInstance<GameRepository>(sp, TimeSpan.FromHours(1)))
        .AddScoped<IPlayer>(sp => ActivatorUtilities.CreateInstance<PlayerRepository>(sp, TimeSpan.FromHours(1)));
}
