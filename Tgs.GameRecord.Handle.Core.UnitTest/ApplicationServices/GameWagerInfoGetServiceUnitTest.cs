using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameWagerInfoGetServiceUnitTest
{
    [Fact]
    public async Task CurrencyCode_IDR_Loss_ExchangeRate_1000()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameWagerInfoQuery(
            GameProviderCode: "AWC_PGS",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            WagerState: WagerStates.Lose,
            Bet: 20,
            ValidBet: 8,
            Settle: 8,
            Commission: 0,
            Turnover: 8,
            WinLoss: -12);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetIdAsync(
            code: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "IDR");
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandGameProvider
            .GetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var game = new GameModel(
            1000017,
            "910",
            gameProviderId,
            "KY",
            "910",
            GameCategories.CardGame,
            GameTypes.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            CurrencyRatio: 1000,
            Bet: 20000,
            ValidBet: 8000,
            Settle: 8000,
            Commission: 0,
            Turnover: 8000,
            PlayerWinLoss: -12000);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(query.Settle),
            commission: Arg.Is(query.Commission),
            turnover: Arg.Is(query.Turnover),
            playerWinLoss: Arg.Is(query.WinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameWagerInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(game.GameProviderId, actual.GameProviderId);
        Assert.Equal(game.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(game.SubGameProviderCode, actual.SubGameProviderCode);
        Assert.Equal(game.GameId, actual.GameId);
        Assert.Equal(game.GameType, actual.GameType);
        Assert.Equal(player.ResellerId, actual.ResellerId);
        Assert.Equal(player.ResellerCode, actual.ResellerCode);
        Assert.Equal(player.LicenseeId, actual.LicenseeId);
        Assert.Equal(player.LicenseeCode, actual.LicenseeCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(player.BrandCode, actual.BrandCode);
        Assert.Equal(player.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.Username, actual.Username);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(1000, actual.CurrencyRatio);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.WinLoss);
    }

    [Fact]
    public async Task Loss()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameWagerInfoQuery(
            GameProviderCode: "KY",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            WagerState: WagerStates.Lose,
            Bet: 4,
            ValidBet: 4,
            Settle: 0,
            Commission: 0,
            Turnover: 4,
            WinLoss: -4);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetIdAsync(
            code: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandGameProvider
            .GetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var game = new GameModel(
            1000017,
            "910",
            gameProviderId,
            "KY",
            "910",
            GameCategories.CardGame,
            GameTypes.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 0,
            Commission: 0,
            Turnover: 4,
            PlayerWinLoss: -4);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(query.Settle),
            commission: Arg.Is(query.Commission),
            turnover: Arg.Is(query.Turnover),
            playerWinLoss: Arg.Is(query.WinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameWagerInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(game.GameProviderId, actual.GameProviderId);
        Assert.Equal(game.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(game.SubGameProviderCode, actual.SubGameProviderCode);
        Assert.Equal(game.GameId, actual.GameId);
        Assert.Equal(game.GameType, actual.GameType);
        Assert.Equal(player.ResellerId, actual.ResellerId);
        Assert.Equal(player.ResellerCode, actual.ResellerCode);
        Assert.Equal(player.LicenseeId, actual.LicenseeId);
        Assert.Equal(player.LicenseeCode, actual.LicenseeCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(player.BrandCode, actual.BrandCode);
        Assert.Equal(player.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.Username, actual.Username);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(1, actual.CurrencyRatio);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.WinLoss);
    }

    [Fact]
    public async Task Running()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameWagerInfoQuery(
            GameProviderCode: "KY",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            WagerState: WagerStates.Running,
            Bet: 4.0m,
            ValidBet: 4.0m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 4.0m,
            WinLoss: 0.0m);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetIdAsync(
            code: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandGameProvider
            .GetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var game = new GameModel(
            1000017,
            "910",
            gameProviderId,
            "KY",
            "910",
            GameCategories.CardGame,
            GameTypes.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            CurrencyRatio: 1,
            Bet: 4.0m,
            ValidBet: 4.0m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 0.0m,
            PlayerWinLoss: 0.0m);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.0m),
            playerWinLoss: Arg.Is(0.0m),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameWagerInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(game.GameProviderId, actual.GameProviderId);
        Assert.Equal(game.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(game.SubGameProviderCode, actual.SubGameProviderCode);
        Assert.Equal(game.GameId, actual.GameId);
        Assert.Equal(game.GameType, actual.GameType);
        Assert.Equal(player.ResellerId, actual.ResellerId);
        Assert.Equal(player.ResellerCode, actual.ResellerCode);
        Assert.Equal(player.LicenseeId, actual.LicenseeId);
        Assert.Equal(player.LicenseeCode, actual.LicenseeCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(player.BrandCode, actual.BrandCode);
        Assert.Equal(player.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.Username, actual.Username);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(1, actual.CurrencyRatio);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(0.0m, actual.Settle);
        Assert.Equal(0.0m, actual.Commission);
        Assert.Equal(0.0m, actual.Turnover);
        Assert.Equal(0.0m, actual.WinLoss);
    }

    [Fact]
    public async Task Waiting()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameWagerInfoQuery(
            GameProviderCode: "KY",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            WagerState: WagerStates.Waiting,
            Bet: 4.0m,
            ValidBet: 4.0m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 4.0m,
            WinLoss: 0.0m);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetIdAsync(
            code: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandGameProvider
            .GetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var game = new GameModel(
            1000017,
            "910",
            gameProviderId,
            "KY",
            "910",
            GameCategories.CardGame,
            GameTypes.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            CurrencyRatio: 1,
            Bet: 4.0m,
            ValidBet: 4.0m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 0.0m,
            PlayerWinLoss: 0.0m);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(0.0m),
            playerWinLoss: Arg.Is(0.0m),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameWagerInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(game.GameProviderId, actual.GameProviderId);
        Assert.Equal(game.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(game.SubGameProviderCode, actual.SubGameProviderCode);
        Assert.Equal(game.GameId, actual.GameId);
        Assert.Equal(game.GameType, actual.GameType);
        Assert.Equal(player.ResellerId, actual.ResellerId);
        Assert.Equal(player.ResellerCode, actual.ResellerCode);
        Assert.Equal(player.LicenseeId, actual.LicenseeId);
        Assert.Equal(player.LicenseeCode, actual.LicenseeCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(player.BrandCode, actual.BrandCode);
        Assert.Equal(player.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.Username, actual.Username);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(1, actual.CurrencyRatio);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(0.0m, actual.Settle);
        Assert.Equal(0.0m, actual.Commission);
        Assert.Equal(0.0m, actual.Turnover);
        Assert.Equal(0.0m, actual.WinLoss);
    }

    [Fact]
    public async Task Win()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameWagerInfoQuery(
            GameProviderCode: "KY",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            WagerState: WagerStates.Win,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetIdAsync(
            code: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandGameProvider
            .GetGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var game = new GameModel(
            1000017,
            "910",
            gameProviderId,
            "KY",
            "910",
            GameCategories.CardGame,
            GameTypes.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            PlayerWinLoss: 4);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(query.Settle),
            commission: Arg.Is(query.Commission),
            turnover: Arg.Is(query.Turnover),
            playerWinLoss: Arg.Is(query.WinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameWagerInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service.GetAsync(query: query);

        Assert.NotNull(actual);
        Assert.Equal(game.GameProviderId, actual.GameProviderId);
        Assert.Equal(game.GameProviderCode, actual.GameProviderCode);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.Equal(game.SubGameProviderCode, actual.SubGameProviderCode);
        Assert.Equal(game.GameId, actual.GameId);
        Assert.Equal(game.GameType, actual.GameType);
        Assert.Equal(player.ResellerId, actual.ResellerId);
        Assert.Equal(player.ResellerCode, actual.ResellerCode);
        Assert.Equal(player.LicenseeId, actual.LicenseeId);
        Assert.Equal(player.LicenseeCode, actual.LicenseeCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(player.BrandCode, actual.BrandCode);
        Assert.Equal(player.PlayerCode, actual.PlayerCode);
        Assert.Equal(player.Username, actual.Username);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(1, actual.CurrencyRatio);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.WinLoss);
    }
}
