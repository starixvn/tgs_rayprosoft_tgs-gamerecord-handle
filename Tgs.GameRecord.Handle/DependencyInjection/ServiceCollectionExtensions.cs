using Tgs.GameCommand.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandleService(
        this IServiceCollection services,
        Action<IPAddressOptions, IServiceProvider> ipAddressOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions)
        => services
        .AddMemoryCache()
        .AddHandleCore(
            ipAddressOptions)
        .AddHandleRepository(
            gameCommandClientOptions,
            gameCurrencyClientOptions);
}
