using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordInfoGetServiceUnitTest
{
    [Fact]
    public async Task CurrencyCode_IDR_Loss_ExchangeRate_1000()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameRecordInfoQuery(
            GameProviderCode: "AWC_PGS",
            ExternalRoundId: "50-1691469296-370404138-1",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            Status: GameRecordStatus.Closed,
            ClientStatus: GameRecordClientStatus.Loss,
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08T15:56:50+08:00"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08T15:57:14+08:00"),
            Bet: 20,
            ValidBet: 8,
            Settle: 8,
            Commission: 0,
            Turnover: 8,
            PlayerWinLoss: -12);

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
            GameRecordGameType.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            ExchangeRate: 1000,
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
            playerWinLoss: Arg.Is(query.PlayerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameRecordInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

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
        Assert.Equal(query.Status, actual.Status);
        Assert.Equal(query.ClientStatus, actual.ClientStatus);
        Assert.Equal(query.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(query.ExternalGameId, actual.ExternalGameId);
        Assert.Equal(query.ExternalCreatedOn, actual.ExternalCreatedOn);
        Assert.Equal(query.ExternalClosedOn, actual.ExternalClosedOn);
        Assert.Equal(1000, actual.ExchangeRate);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.PlayerWinLoss);
    }

    [Fact]
    public async Task Loss()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameRecordInfoQuery(
            GameProviderCode: "KY",
            ExternalRoundId: "50-1691462098-370341703-1",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            Status: GameRecordStatus.Closed,
            ClientStatus: GameRecordClientStatus.Loss,
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 10:34:58.000 +0800"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08 10:35:21.000 +0800"),
            Bet: 4,
            ValidBet: 4,
            Settle: 0,
            Commission: 0,
            Turnover: 4,
            PlayerWinLoss: -4);

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
            GameRecordGameType.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            ExchangeRate: 1,
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
            playerWinLoss: Arg.Is(query.PlayerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameRecordInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

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
        Assert.Equal(query.Status, actual.Status);
        Assert.Equal(query.ClientStatus, actual.ClientStatus);
        Assert.Equal(query.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(query.ExternalGameId, actual.ExternalGameId);
        Assert.Equal(query.ExternalCreatedOn, actual.ExternalCreatedOn);
        Assert.Equal(query.ExternalClosedOn, actual.ExternalClosedOn);
        Assert.Equal(1, actual.ExchangeRate);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.PlayerWinLoss);
    }

    [Fact]
    public async Task Open()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameRecordInfoQuery(
            GameProviderCode: "KY",
            ExternalRoundId: "50-1691462098-370341703-1",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            Status: GameRecordStatus.Open,
            ClientStatus: GameRecordClientStatus.Open,
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 10:34:58.000 +0800"),
            ExternalClosedOn: DateTimeOffset.UnixEpoch,
            Bet: 4,
            ValidBet: 4,
            Settle: 0,
            Commission: 0,
            Turnover: 0,
            PlayerWinLoss: 0);

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
            GameRecordGameType.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            ExchangeRate: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 0,
            Commission: 0,
            Turnover: 0,
            PlayerWinLoss: 0);

        _ = fakeGameCurrency
            .GetAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            bet: Arg.Is(query.Bet),
            validBet: Arg.Is(query.ValidBet),
            settle: Arg.Is(query.Settle),
            commission: Arg.Is(query.Commission),
            turnover: Arg.Is(query.Turnover),
            playerWinLoss: Arg.Is(query.PlayerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameRecordInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

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
        Assert.Equal(query.Status, actual.Status);
        Assert.Equal(query.ClientStatus, actual.ClientStatus);
        Assert.Equal(query.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(query.ExternalGameId, actual.ExternalGameId);
        Assert.Equal(query.ExternalCreatedOn, actual.ExternalCreatedOn);
        Assert.Equal(DateTime.UnixEpoch, actual.ExternalClosedOn);
        Assert.Equal(1, actual.ExchangeRate);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.PlayerWinLoss);
    }

    [Fact]
    public async Task Win()
    {
        var fakeBrandGameProvider = Substitute.For<IBrandGameProvider>();
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameCurrency = Substitute.For<IGameCurrency>();

        var query = new GameRecordInfoQuery(
            GameProviderCode: "KY",
            ExternalRoundId: "50-1691469296-370404138-1",
            ExternalGameId: "910",
            PlayerCode: "4ucmqplxb029",
            Status: GameRecordStatus.Closed,
            ClientStatus: GameRecordClientStatus.Win,
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 12:34:56.000 +0800"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08 12:35:18.000 +0800"),
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            PlayerWinLoss: 4);

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
            GameRecordGameType.CardGame,
            "KY");
        _ = fakeGame
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            externaGamelId: Arg.Is(query.ExternalGameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(game);

        var gameCurrencyResult = new GameCurrencyModel(
            ExchangeRate: 1,
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
            playerWinLoss: Arg.Is(query.PlayerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameCurrencyResult);

        var service = new GameRecordInfoGetService(
            fakeBrandGameProvider,
            fakeGame,
            fakeGameProvider,
            fakePlayer,
            fakeGameCurrency);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

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
        Assert.Equal(query.Status, actual.Status);
        Assert.Equal(query.ClientStatus, actual.ClientStatus);
        Assert.Equal(query.ExternalRoundId, actual.ExternalRoundId);
        Assert.Equal(query.ExternalGameId, actual.ExternalGameId);
        Assert.Equal(query.ExternalCreatedOn, actual.ExternalCreatedOn);
        Assert.Equal(query.ExternalClosedOn, actual.ExternalClosedOn);
        Assert.Equal(1, actual.ExchangeRate);
        Assert.Equal(gameCurrencyResult.Bet, actual.Bet);
        Assert.Equal(gameCurrencyResult.ValidBet, actual.ValidBet);
        Assert.Equal(gameCurrencyResult.Settle, actual.Settle);
        Assert.Equal(gameCurrencyResult.Commission, actual.Commission);
        Assert.Equal(gameCurrencyResult.Turnover, actual.Turnover);
        Assert.Equal(gameCurrencyResult.PlayerWinLoss, actual.PlayerWinLoss);
    }
}
