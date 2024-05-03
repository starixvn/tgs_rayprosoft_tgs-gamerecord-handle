using System.Reflection;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordHandleCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton(TimeProvider.System)
		.AddSingleton<IGameRecordDispatcher, GameRecordDispatcher>();
}
