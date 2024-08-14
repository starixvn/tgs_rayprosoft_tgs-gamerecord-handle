using Tgs.GameRecord.Handle.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordHandleClient(this IServiceCollection services, Action<IHttpClientBuilder> httpClientConfigure)
	{
		httpClientConfigure(services.AddHttpClient<IGameRecordHandleClient, GameRecordHandleClient>());

		return services;
	}
}
