using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Client;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class BrandGameProviderRepositoryUnitTest
{
    [Fact]
    public async Task No_Data()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        var key = $"gamerecord:handle:brand:gameprovider:{brandId}:{gameProviderId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        _ = fakeClient
            .BrandGetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(brandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var repository = new BrandGameProviderRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        _ = await Assert.ThrowsAsync<NullReferenceException>(
             () => repository
             .GetGameProviderConfigurationIdAsync(
                 brandId: brandId,
                 gameProviderId: gameProviderId)
             .AsTask());
    }

    [Fact]
    public async Task Normal()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        var key = $"gamerecord:handle:brand:gameprovider:{brandId}:{gameProviderId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeClient
            .BrandGetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(brandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var repository = new BrandGameProviderRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetGameProviderConfigurationIdAsync(
            brandId: brandId,
            gameProviderId: gameProviderId);

        Assert.Equal(gameProviderConfigurationId, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: gameProviderConfigurationId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }
}
