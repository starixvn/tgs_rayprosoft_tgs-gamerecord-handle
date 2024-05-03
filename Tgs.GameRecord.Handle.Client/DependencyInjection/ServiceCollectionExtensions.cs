using Tgs.GameRecord.Handle.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordHandleClient(
		this IServiceCollection services)
		=> services
		.AddTransient<IGameRecordHandleClient, GameRecordHandleClient>();
}
