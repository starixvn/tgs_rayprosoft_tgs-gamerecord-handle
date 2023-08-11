using Tgs.GameCommand.Client;
using Tgs.GameCurrency.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandleService(
        this IServiceCollection services,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions)
        => services
        .AddHandleCore()
        .AddHandleRepository(
            gameCommandClientOptions,
            gameCurrencyClientOptions);
}
