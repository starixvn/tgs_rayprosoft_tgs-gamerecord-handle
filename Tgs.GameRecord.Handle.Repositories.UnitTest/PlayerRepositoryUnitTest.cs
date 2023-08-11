using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class PlayerRepositoryUnitTest
{
    [Fact]
    public async Task No_Data()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

        var playerCode = "4ucmqplxb029";

        var key = $"gamerecord:handle:player:{playerCode}";

        _ = fakeMemoryCache
            .TryGetValue(
                key: Arg.Is(key),
                value: out _)
             .Returns(false);

        _ = fakeClient
            .PlayerGetAsync(
            playerCode: Arg.Is(playerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<PlayerInfo?>(null));

        var repository = new PlayerRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        _ = await Assert.ThrowsAsync<NullReferenceException>(
             () => repository
             .GetAsync(
                 playerCode: playerCode)
             .AsTask())
            .ConfigureAwait(false);
    }

    [Fact]
    public async Task Normal()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

        var playerCode = "4ucmqplxb029";

        var key = $"gamerecord:handle:player:{playerCode}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeClient
            .PlayerGetAsync(
            playerCode: Arg.Is(playerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new PlayerRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            playerCode: playerCode)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(info.PlayerCode, actual.PlayerCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }
}
