using System.Text.Json.Nodes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameWagerHandleCommandServiceUnitTest
{
    [Fact]
    public async Task CancelState_Is_Cancel()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Reject,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.0m,
            Settle: 0.4m,
            Commission: 0m,
            Turnover: 0.0m,
            WinLoss: 0.0m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.0m,
                ["Turnover"] = 0.0m,
                ["WinAmount"] = 0.4,
                ["RealWinAmount"] = 0.0m,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);
        var result = new GameWagerInfoResult(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "AWC_PGS",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "AWC_PGS",
            GameId: 1000017,
            GameCategory: GameCategories.Slot,
            GameType: GameTypes.Slot,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            CurrencyCode: "CNY",
            WalletId: Guid.NewGuid(),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeInfoService.GetAsync(
            query: Arg.Is(query),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork.GetId()
            .Returns(wagerId);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        await sut.ExecuteAsync(command: command);

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            ClosedState: ClosedStates.Closed,
            VoidState: VoidStates.None,
            CancelState: CancelStates.Cancel,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(publishCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClosedState_Is_Closed()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.32m,
            Commission: 0m,
            Turnover: 0.4m,
            WinLoss: -0.08m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0.4,
                ["WinAmount"] = 0.32,
                ["RealWinAmount"] = 0.32,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);
        var result = new GameWagerInfoResult(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "AWC_PGS",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "AWC_PGS",
            GameId: 1000017,
            GameCategory: GameCategories.Slot,
            GameType: GameTypes.Slot,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            CurrencyCode: "CNY",
            WalletId: Guid.NewGuid(),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeInfoService.GetAsync(
            query: Arg.Is(query),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork.GetId()
            .Returns(wagerId);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        await sut.ExecuteAsync(command: command);

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            ClosedState: ClosedStates.Closed,
            VoidState: VoidStates.None,
            CancelState: CancelStates.None,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(publishCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExternalRoundId_Is_Empty()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: string.Empty,
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.32m,
            Commission: 0m,
            Turnover: 0.4m,
            WinLoss: -0.08m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0.4,
                ["WinAmount"] = 0.32,
                ["RealWinAmount"] = 0.32,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        _ = await Assert.ThrowsAsync<ArgumentException>(
             () => sut.ExecuteAsync(
                 command: command)
             .AsTask());

        _ = fakeInfoService
            .Received(0)
            .GetAsync(
            query: Arg.Any<GameWagerInfoQuery>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSnowflakeWork
            .Received(0)
            .GetId();

        _ = fakePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<GameWagerPublishCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExternalWagerId_Is_Empty()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: string.Empty,
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.32m,
            Commission: 0m,
            Turnover: 0.4m,
            WinLoss: -0.08m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0.4,
                ["WinAmount"] = 0.32,
                ["RealWinAmount"] = 0.32,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        _ = await Assert.ThrowsAsync<ArgumentException>(
             () => sut.ExecuteAsync(
                 command: command)
             .AsTask());

        _ = fakeInfoService
            .Received(0)
            .GetAsync(
            query: Arg.Any<GameWagerInfoQuery>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSnowflakeWork
            .Received(0)
            .GetId();

        _ = fakePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<GameWagerPublishCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Game_Does_Not_Exist()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.32m,
            Commission: 0m,
            Turnover: 0.4m,
            WinLoss: -0.08m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0.4,
                ["WinAmount"] = 0.32,
                ["RealWinAmount"] = 0.32,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        _ = fakeInfoService
            .GetAsync(
            query: Arg.Any<GameWagerInfoQuery>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .ThrowsForAnyArgs<NullReferenceException>();

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        _ = await Assert.ThrowsAsync<NullReferenceException>(
             () => sut.ExecuteAsync(
                 command: command)
             .AsTask());

        _ = fakeSnowflakeWork
            .Received(0)
            .GetId();

        _ = fakePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<GameWagerPublishCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task VoidState_Is_void()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Void,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.0m,
            Settle: 0.4m,
            Commission: 0m,
            Turnover: 0.0m,
            WinLoss: 0.0m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.0m,
                ["Turnover"] = 0.0m,
                ["WinAmount"] = 0.4,
                ["RealWinAmount"] = 0.0m,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);
        var result = new GameWagerInfoResult(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "AWC_PGS",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "AWC_PGS",
            GameId: 1000017,
            GameCategory: GameCategories.Slot,
            GameType: GameTypes.Slot,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            CurrencyCode: "CNY",
            WalletId: Guid.NewGuid(),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeInfoService.GetAsync(
            query: Arg.Is(query),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork.GetId()
            .Returns(wagerId);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        await sut.ExecuteAsync(command: command);

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            ClosedState: ClosedStates.Closed,
            VoidState: VoidStates.Void,
            CancelState: CancelStates.None,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(publishCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WagerStates_Is_Running()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Running,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.UnixEpoch,
            ExternalPayoffOn: DateTimeOffset.UnixEpoch,
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 0.0m,
            WinLoss: 0.0m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "1990-01-01T00:00:00Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0,
                ["WinAmount"] = 0,
                ["RealWinAmount"] = 0,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);
        var result = new GameWagerInfoResult(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "AWC_PGS",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "AWC_PGS",
            GameId: 1000017,
            GameCategory: GameCategories.Slot,
            GameType: GameTypes.Slot,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            CurrencyCode: "CNY",
            WalletId: Guid.NewGuid(),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeInfoService.GetAsync(
            query: Arg.Is(query),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork.GetId()
            .Returns(wagerId);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        await sut.ExecuteAsync(command: command);

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            ClosedState: ClosedStates.None,
            VoidState: VoidStates.None,
            CancelState: CancelStates.None,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(publishCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WagerStates_Is_Waiting()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Waiting,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.UnixEpoch,
            ExternalPayoffOn: DateTimeOffset.UnixEpoch,
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 0.0m,
            WinLoss: 0.0m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "1990-01-01T00:00:00Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0,
                ["WinAmount"] = 0,
                ["RealWinAmount"] = 0,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var query = new GameWagerInfoQuery(
            GameProviderCode: command.GameProviderCode,
            PlayerCode: command.PlayerCode,
            ExternalGameId: command.ExternalGameId,
            WagerState: command.WagerState,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            WinLoss: command.WinLoss);
        var result = new GameWagerInfoResult(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "AWC_PGS",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "AWC_PGS",
            GameId: 1000017,
            GameCategory: GameCategories.Slot,
            GameType: GameTypes.Slot,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            CurrencyCode: "CNY",
            WalletId: Guid.NewGuid(),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeInfoService.GetAsync(
            query: Arg.Is(query),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork.GetId()
            .Returns(wagerId);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        await sut.ExecuteAsync(command: command);

        var publishCommand = new GameWagerPublishCommand(
            WagerId: wagerId,
            GameProviderId: result.GameProviderId,
            GameProviderCode: result.GameProviderCode,
            GameProviderConfigurationId: result.GameProviderConfigurationId,
            SubGameProviderCode: result.SubGameProviderCode,
            GameId: result.GameId,
            GameCategory: result.GameCategory,
            GameType: result.GameType,
            ResellerId: result.ResellerId,
            ResellerCode: result.ResellerCode,
            LicenseeId: result.LicenseeId,
            LicenseeCode: result.LicenseeCode,
            BrandId: result.BrandId,
            BrandCode: result.BrandCode,
            PlayerCode: result.PlayerCode,
            Username: result.Username,
            CurrencyCode: result.CurrencyCode,
            WalletId: result.WalletId,
            WagerState: command.WagerState,
            WagerType: command.WagerType,
            ClosedState: ClosedStates.None,
            VoidState: VoidStates.None,
            CancelState: CancelStates.None,
            FreeGameState: command.FreeGameState,
            GambleState: command.GambleState,
            CashoutState: command.CashoutState,
            ResettleState: command.ResettleState,
            ExternalRoundId: command.ExternalRoundId,
            ExternalWagerId: command.ExternalWagerId,
            ExternalGameId: command.ExternalGameId,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            ExternalPayoffOn: command.ExternalPayoffOn,
            ExternalResettleOn: command.ExternalResettleOn,
            CurrencyRatio: result.CurrencyRatio,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            WinLoss: result.WinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(publishCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task This_game_bet_will_take_more_than_100_days_to_settle()
    {
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeInfoService = Substitute.For<IGetService<GameWagerInfoQuery, GameWagerInfoResult>>();
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakePublishService = Substitute.For<ICommandService<GameWagerPublishCommand>>();

        var command = new GameWagerHandleCommand(
            GameProviderCode: "AWC_PGS",
            PlayerCode: "4ugse0rsnufp",
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "1694585239319900160",
            ExternalWagerId: "PG-1694585239319900160",
            ExternalGameId: "PG-SLOT-051",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            Bet: 0.4m,
            ValidBet: 0.4m,
            Settle: 0.32m,
            Commission: 0m,
            Turnover: 0.4m,
            WinLoss: -0.08m,
            AdditionalData: new JsonObject
            {
                ["ExternalRoundId"] = "PG-1694585239319900160",
                ["Platform"] = "PG",
                ["PlatformTxId"] = "1694585239319900160",
                ["GameType"] = "SLOT",
                ["GameCode"] = "PG-SLOT-051",
                ["GameName"] = "Egypt's Book of Mystery",
                ["RoundId"] = "1694585239319900160",
                ["UserId"] = "4ugse0rsnufp",
                ["Currency"] = "CNY",
                ["TxStatus"] = 1,
                ["SettleStatus"] = 0,
                ["BetTime"] = "2023-08-24T05:39:39Z",
                ["TxTime"] = "2023-08-24T05:39:39Z",
                ["UpdateTime"] = "2023-08-24T05:39:39Z",
                ["BetType"] = "1",
                ["BetAmount"] = 0.4,
                ["RealBetAmount"] = 0.4,
                ["Turnover"] = 0.4,
                ["WinAmount"] = 0.32,
                ["RealWinAmount"] = 0.32,
                ["JackpotBetAmount"] = 0,
                ["JackpotWinAmount"] = 0,
                ["GameInfo"] = "{\"freeGameTransactionId\":\"\",\"isAdjustment\":false,\"isEndRound\":false,\"bonusTransactionId\":\"\",\"isBuyFreeSpin\":false,\"isFeature\":false}"
            }.ToJsonString());

        var currentTime = new DateTimeOffset(2024, 02, 16, 06, 33, 23, TimeSpan.Zero);
        _ = fakeSystemClock.CurrentTime
           .Returns(currentTime);

        var sut = new GameWagerHandleCommandService(
            fakeSystemClock,
            fakeInfoService,
            fakeSnowflakeWork,
            fakePublishService);

        _ = await Assert.ThrowsAsync<Exception>(
             () => sut.ExecuteAsync(
                 command: command)
             .AsTask());

        _ = fakeInfoService
            .Received(0)
            .GetAsync(
            query: Arg.Any<GameWagerInfoQuery>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSnowflakeWork
            .Received(0)
            .GetId();

        _ = fakePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Any<GameWagerPublishCommand>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
