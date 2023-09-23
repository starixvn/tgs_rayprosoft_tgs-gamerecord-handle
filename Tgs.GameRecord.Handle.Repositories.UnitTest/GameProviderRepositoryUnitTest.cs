using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Client;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
    [Fact]
    public async Task Get_No_Data()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var code = "KY";

        var key = $"gamerecord:handle:gameprovider:{code}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        _ = fakeClient
            .GameProviderGetGameProviderIdAsync(
            gameProviderCode: Arg.Is(code),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var repository = new GameProviderRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        _ = await Assert.ThrowsAsync<NullReferenceException>(
             () => repository
             .GetIdAsync(
                 code: code)
             .AsTask());
    }

    [Fact]
    public async Task Get_Normal()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var code = "KY";

        var key = $"gamerecord:handle:gameprovider:{code}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var id = Guid.NewGuid();
        _ = fakeClient
            .GameProviderGetGameProviderIdAsync(
            gameProviderCode: Arg.Is(code),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(id);

        var repository = new GameProviderRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository.GetIdAsync(code: code);

        Assert.Equal(id, actual);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
            key: key,
            value: id,
            absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }
}
