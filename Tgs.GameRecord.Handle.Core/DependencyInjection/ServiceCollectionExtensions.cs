using System.Reflection;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandleCore(
        this IServiceCollection services,
        Action<IPAddressOptions, IServiceProvider> ipAddressOptions)
        => services
            .AddOptions<IPAddressOptions>()
            .Configure(ipAddressOptions)
            .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddSingleton<ISnowflakeWork, SnowflakeWork>()
        .AddSerialNumberGenerator();
}
