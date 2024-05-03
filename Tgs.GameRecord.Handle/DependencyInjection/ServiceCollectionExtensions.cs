using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordHandle(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddMemoryCache()
		.AddGameRecordHandleCore()
		.AddGameRecordHandleRepositories(
			dbContextOptions: dbContextOptions);
}
