using Microsoft.EntityFrameworkCore;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordHandleRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
        => services
        .AddDbContext<TgsContext>(dbContextOptions)
        .AddScoped<IGameCurrency, GameCurrencyRepository>()
        .AddScoped<IBrandGameProviderRepository, BrandGameProviderRepository>()
        .Decorate<IBrandGameProviderRepository, BrandGameProviderCacheRepository>()
        .AddScoped<IGameProviderRepository, GameProviderRepository>()
        .Decorate<IGameProviderRepository, GameProviderCacheRepository>()
        .AddScoped<IGameRepository, GameRepository>()
        .Decorate<IGameRepository, GameCacheRepository>()
        .AddScoped<IPlayerRepository, PlayerRepository>()
        .Decorate<IPlayerRepository, PlayerCacheRepository>();
}
