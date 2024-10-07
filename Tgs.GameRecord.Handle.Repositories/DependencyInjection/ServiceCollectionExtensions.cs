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
		.AddTransient<IGameCurrencyRepository, GameCurrencyRepository>()
		.AddTransient<IBrandGameProviderRepository, BrandGameProviderRepository>()
		.Decorate<IBrandGameProviderRepository, BrandGameProviderCacheRepository>()
		.AddTransient<IGameProviderRepository, GameProviderRepository>()
		.Decorate<IGameProviderRepository, GameProviderCacheRepository>()
		.AddTransient<IGameRepository, GameRepository>()
		.Decorate<IGameRepository, GameCacheRepository>()
		.AddTransient<IPlayerRepository, PlayerRepository>()
		.Decorate<IPlayerRepository, PlayerCacheRepository>()
		.AddTransient<IGameBetsRapidTestWorkerPublishRepository, GameBetsRapidTestWorkerPublishRepository>();
}
