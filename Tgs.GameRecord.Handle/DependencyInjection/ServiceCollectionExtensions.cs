using System.Net;

using Microsoft.Extensions.Options;
using Snowflake.Core;
using Tgs.GameCommand.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameRecord.Handle.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordHandle(
        this IServiceCollection services,
        Action<IPAddressOptions, IServiceProvider> ipAddressOptions,
        Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
        Action<GameCurrencyClientOptions, IServiceProvider> gameCurrencyClientOptions)
        => services
        .AddOptions<IPAddressOptions>()
        .Configure(ipAddressOptions)
        .Services
        .AddSingleton((sp) =>
        {
            var options = sp.CreateScope().ServiceProvider.GetService<IOptions<IPAddressOptions>>()!;
            var ipAddress = IPAddress.Parse(options.Value.IPAddress);
            var address = ipAddress.IsIPv6Teredo
            ? (long)BitConverter.ToUInt64(ipAddress.GetAddressBytes())
            : BitConverter.ToUInt32(ipAddress.GetAddressBytes());
            var workerId = (address % 255 & 30) + 1;
            var datacenterId = ((address >> 5) % 255 & 30) + 1;
            return new IdWorker(workerId, datacenterId);
        })
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameCurrencyClient(gameCurrencyClientOptions)
        .AddMemoryCache()
        .AddGameRecordHandleCore()
        .AddGameRecordHandleRepository();
}
