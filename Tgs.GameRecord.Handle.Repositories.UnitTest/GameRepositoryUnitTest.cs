using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class GameRepositoryUnitTest
{
    [Fact]
    public async Task CardGame()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "910";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 1000017,
            GameCode: "Baccarat",
            GameProviderId: gameProviderId,
            GameProviderCode: "KY",
            GameExternalId: "910",
            Type: GameType.CardGame,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Baccarat",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "KY");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.CardGame, actual.GameCategory);
        Assert.Equal((int)info.Type, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task CardGameLobby()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "0";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 1000001,
            GameCode: "Lobby",
            GameProviderId: gameProviderId,
            GameProviderCode: "KY",
            GameExternalId: "0",
            Type: GameType.CardGameLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "KY");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.CardGame, actual.GameCategory);
        Assert.Equal((int)GameType.CardGame, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task ESports()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "ESPORTSBULL";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 5500001,
            GameCode: "ESPORTSBULL",
            GameProviderId: gameProviderId,
            GameProviderCode: "IM_ESP",
            GameExternalId: "ESPORTSBULL",
            Type: GameType.ESports,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "ESPORTSBULL",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "IM_ESP");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Sportsbook, actual.GameCategory);
        Assert.Equal((int)info.Type, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task Fishing()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "520";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 1000044,
            GameCode: "LK_Fishing",
            GameProviderId: gameProviderId,
            GameProviderCode: "KY",
            GameExternalId: "520",
            Type: GameType.Fishing,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "LK Fishing",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "KY");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Slot, actual.GameCategory);
        Assert.Equal((int)GameType.Fishing, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task LiveDealer()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "BonsaiBacc000001";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 4300685,
            GameCode: "bonsai_speed_baccarat_a",
            GameProviderId: gameProviderId,
            GameProviderCode: "EVOTF",
            GameExternalId: "BonsaiBacc000001",
            Type: GameType.LiveDealer,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Bonsai Speed Baccarat A",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "EVOTF");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.LiveDealer, actual.GameCategory);
        Assert.Equal((int)info.Type, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task LiveDealerLobby()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "EVOLobby";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 4300178,
            GameCode: "Lobby",
            GameProviderId: gameProviderId,
            GameProviderCode: "EVOTF",
            GameExternalId: "EVOLobby",
            Type: GameType.LiveDealerLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Lobby",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "EVOTF");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.LiveDealer, actual.GameCategory);
        Assert.Equal((int)GameType.LiveDealer, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task Lottery()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "AE-LOTTO-075";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 2800069,
            GameCode: "HCM_5min",
            GameProviderId: gameProviderId,
            GameProviderCode: "AWC_AELOTTO",
            GameExternalId: "AE-LOTTO-075",
            Type: GameType.Lottery,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "HCM 5min",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "AWC_AELOTTO");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Lottery, actual.GameCategory);
        Assert.Equal((int)GameType.Lottery, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task LotteryLobby()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "LottoLobby";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 2800001,
            GameCode: "HCM_5min",
            GameProviderId: gameProviderId,
            GameProviderCode: "AWC_AELOTTO",
            GameExternalId: "LottoLobby",
            Type: GameType.LotteryLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Lotto Lobby",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "AWC_AELOTTO");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Lottery, actual.GameCategory);
        Assert.Equal((int)GameType.Lottery, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task No_DataAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "910";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<GameInfo?>(null));

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.Null(actual);
    }

    [Fact]
    public async Task Slot()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "22043";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 5200001,
            GameCode: "sugar_bang_bang",
            GameProviderId: gameProviderId,
            GameProviderCode: "FACHAI",
            GameExternalId: "22043",
            Type: GameType.Slot,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Sugar Bang Bang",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "FACHAI");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Slot, actual.GameCategory);
        Assert.Equal((int)GameType.Slot, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task Sportsbook()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "sports_book";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 4600001,
            GameCode: "sports_book",
            GameProviderId: gameProviderId,
            GameProviderCode: "SBOTF",
            GameExternalId: "sports_book",
            Type: GameType.Sportsbook,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "SportsBook",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "SBOTF");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Sportsbook, actual.GameCategory);
        Assert.Equal((int)info.Type, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task TableGame()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "28001";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 5200040,
            GameCode: "lucky_9",
            GameProviderId: gameProviderId,
            GameProviderCode: "FACHAI",
            GameExternalId: "28001",
            Type: GameType.TableGame,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Lucky 9",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "FACHAI");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.Slot, actual.GameCategory);
        Assert.Equal((int)GameType.TableGame, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task VirtualSports()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "virtual_sports";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 4600002,
            GameCode: "virtual_sports",
            GameProviderId: gameProviderId,
            GameProviderCode: "SBOTF",
            GameExternalId: "virtual_sports",
            Type: GameType.VirtualSports,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "VirtualSports",
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "SBOTF");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.VirtualSports, actual.GameCategory);
        Assert.Equal((int)info.Type, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task VirtualSportsLobby()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();
        var fakeMemoryCache = Substitute.For<IMemoryCache>();
        var absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var gameProviderId = Guid.NewGuid();
        string externaGamelId = "VirtualSportsLobby";

        var key = $"gamerecord:handle:game:{gameProviderId}:{externaGamelId}";

        _ = fakeMemoryCache
            .TryGetValue(
            key: Arg.Is(key),
            value: out _)
            .Returns(false);

        var info = new GameInfo(
            GameId: 100002,
            GameCode: "Virtual Sports Lobby",
            GameProviderId: gameProviderId,
            GameProviderCode: "SBO",
            GameExternalId: "VirtualSportsLobby",
            Type: GameType.VirtualSportsLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: "Virtual Sports Lobby",
            SubGame: false,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: false,
            Html5: true,
            Description: string.Empty,
            EndpointPath: string.Empty,
            RuleUrl: string.Empty,
            AvailableCurrencies: null,
            Localization: string.Empty,
            SubGameProviderCode: "SBO");
        _ = fakeClient
            .GameGetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameExternalId: externaGamelId,
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(info);

        var repository = new GameRepository(
            fakeMemoryCache,
            fakeClient,
            absoluteExpirationRelativeToNow);

        var actual = await repository
            .GetAsync(
            gameProviderId: gameProviderId,
            externaGamelId: externaGamelId);

        Assert.NotNull(actual);
        Assert.Equal(info.GameId, actual.GameId);
        Assert.Equal(info.GameCode, actual.GameCode);
        Assert.Equal(info.GameProviderId, actual.GameProviderId);
        Assert.Equal(info.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(externaGamelId, actual.ExternalGameId);
        Assert.Equal(info.GameExternalId, actual.ExternalGameId);
        Assert.Equal(GameCategories.VirtualSports, actual.GameCategory);
        Assert.Equal((int)GameType.VirtualSports, (int)actual.GameType);
        Assert.Equal(info.SubGameProviderCode, actual.SubGameProviderCode);

        _ = fakeMemoryCache
            .Received(1)
            .Set(
                key: key,
                value: info.GameId,
                absoluteExpirationRelativeToNow: absoluteExpirationRelativeToNow);
    }
}
