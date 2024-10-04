using System.Reflection;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordHandleCore(
		this IServiceCollection services)
		=> services
		.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
		.AddSingleton<IGameRecordDispatcher, GameRecordDispatcher>();
}
