using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordHandleRepository(
        this IServiceCollection services)
        => services
        .AddScoped<IBrandGameProvider>(sp => ActivatorUtilities.CreateInstance<BrandGameProviderRepository>(sp, TimeSpan.FromMinutes(5)))
        .AddScoped<IGameCurrency, GameCurrencyRepository>()
        .AddScoped<IGameProvider>(sp => ActivatorUtilities.CreateInstance<GameProviderRepository>(sp, TimeSpan.FromDays(1)))
        .AddScoped<IGame>(sp => ActivatorUtilities.CreateInstance<GameRepository>(sp, TimeSpan.FromDays(1)))
        .AddScoped<IPlayer>(sp => ActivatorUtilities.CreateInstance<PlayerRepository>(sp, TimeSpan.FromHours(1)));
}
