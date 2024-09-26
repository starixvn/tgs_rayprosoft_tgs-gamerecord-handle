using System.Text.Json.Nodes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameBetsRapidTestWorkerCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "IDR");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1000017,
			GameCode: "910",
			GameProviderId: gameProviderId,
			GameProviderCode: "KY",
			ExternalGameId: "910",
			SubGameProviderCode: "KY");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(command.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m);

		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(command.Bet),
			validBet: Arg.Is(command.ValidBet),
			settle: Arg.Is(command.Settle),
			commission: Arg.Is(command.Commission),
			turnover: Arg.Is(command.Turnover),
			winLoss: Arg.Is(command.WinLoss),
			jackpot: Arg.Is(command.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		await service.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerPublishCommand(
				GameProviderId: gameProviderId,
				GameProviderCode: command.GameProviderCode,
				GameProviderConfigurationId: gameProviderConfigurationId,
				SubGameProviderCode: gameInfo.SubGameProviderCode,
				GameId: gameInfo.GameId,
				ResellerId: playerInfo.ResellerId,
				ResellerCode: playerInfo.ResellerCode,
				LicenseeId: playerInfo.LicenseeId,
				LicenseeCode: playerInfo.LicenseeCode,
				BrandId: playerInfo.BrandId,
				BrandCode: playerInfo.BrandCode,
				PlayerCode: playerInfo.PlayerCode,
				Username: playerInfo.Username,
				CurrencyCode: playerInfo.CurrencyCode,
				WalletId: playerInfo.WalletId,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalGameId: command.ExternalGameId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				ExternalPayoffOn: command.ExternalPayoffOn,
				ExternalResettleOn: command.ExternalResettleOn,
				CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
				Bet: gameCurrencyInfo.Bet,
				ValidBet: gameCurrencyInfo.ValidBet,
				Settle: gameCurrencyInfo.Settle,
				Commission: gameCurrencyInfo.Commission,
				Turnover: gameCurrencyInfo.Turnover,
				WinLoss: gameCurrencyInfo.WinLoss,
				Jackpot: gameCurrencyInfo.Jackpot,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_provider_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.ThrowsAsync(new ArgumentNullException());

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetsRapidTestWorkerPublishCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_player_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((Models.PlayerInfo?)null);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetsRapidTestWorkerPublishCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task The_game_provider_configuration_ID_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "IDR");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.ThrowsAsync(new ArgumentNullException());

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetsRapidTestWorkerPublishCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_does_not_exist()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "IDR");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(command.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((Models.GameInfo?)null);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<GameBetsRapidTestWorkerPublishCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running_Set_ExternalClosedOn_Is_1970_Settle_Is_0_Commission_Is_0_Turnover_Is_0_WinLoss_Is_0_Jackpot_Is_0()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Running,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1000017,
			GameCode: "910",
			GameProviderId: gameProviderId,
			GameProviderCode: "KY",
			ExternalGameId: "910",
			SubGameProviderCode: "KY");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(command.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m);

		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(command.Bet),
			validBet: Arg.Is(command.ValidBet),
			settle: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		await service.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerPublishCommand(
				GameProviderId: gameProviderId,
				GameProviderCode: command.GameProviderCode,
				GameProviderConfigurationId: gameProviderConfigurationId,
				SubGameProviderCode: gameInfo.SubGameProviderCode,
				GameId: gameInfo.GameId,
				ResellerId: playerInfo.ResellerId,
				ResellerCode: playerInfo.ResellerCode,
				LicenseeId: playerInfo.LicenseeId,
				LicenseeCode: playerInfo.LicenseeCode,
				BrandId: playerInfo.BrandId,
				BrandCode: playerInfo.BrandCode,
				PlayerCode: playerInfo.PlayerCode,
				Username: playerInfo.Username,
				CurrencyCode: playerInfo.CurrencyCode,
				WalletId: playerInfo.WalletId,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalGameId: command.ExternalGameId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: DateTimeOffset.UnixEpoch,
				ExternalPayoffOn: DateTimeOffset.UnixEpoch,
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
				Bet: gameCurrencyInfo.Bet,
				ValidBet: gameCurrencyInfo.ValidBet,
				Settle: 0.00m,
				Commission: 0.00m,
				Turnover: 0.00m,
				WinLoss: 0.00m,
				Jackpot: 0.00m,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Waiting_Set_ExternalClosedOn_Is_1970_Settle_Is_0_Commission_Is_0_Turnover_Is_0_WinLoss_Is_0_Jackpot_Is_0()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Waiting,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "CNY");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1000017,
			GameCode: "910",
			GameProviderId: gameProviderId,
			GameProviderCode: "KY",
			ExternalGameId: "910",
			SubGameProviderCode: "KY");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(command.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m);

		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(command.Bet),
			validBet: Arg.Is(command.ValidBet),
			settle: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		await service.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerPublishCommand(
				GameProviderId: gameProviderId,
				GameProviderCode: command.GameProviderCode,
				GameProviderConfigurationId: gameProviderConfigurationId,
				SubGameProviderCode: gameInfo.SubGameProviderCode,
				GameId: gameInfo.GameId,
				ResellerId: playerInfo.ResellerId,
				ResellerCode: playerInfo.ResellerCode,
				LicenseeId: playerInfo.LicenseeId,
				LicenseeCode: playerInfo.LicenseeCode,
				BrandId: playerInfo.BrandId,
				BrandCode: playerInfo.BrandCode,
				PlayerCode: playerInfo.PlayerCode,
				Username: playerInfo.Username,
				CurrencyCode: playerInfo.CurrencyCode,
				WalletId: playerInfo.WalletId,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalGameId: command.ExternalGameId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: DateTimeOffset.UnixEpoch,
				ExternalPayoffOn: DateTimeOffset.UnixEpoch,
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
				Bet: gameCurrencyInfo.Bet,
				ValidBet: gameCurrencyInfo.ValidBet,
				Settle: 0.00m,
				Commission: 0.00m,
				Turnover: 0.00m,
				WinLoss: 0.00m,
				Jackpot: 0.00m,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Settle_Is_Negative()
	{
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakeService = Substitute.For<ICommandService<GameBetsRapidTestWorkerPublishCommand>>();

		var command = new GameBetsRapidTestWorkerCommand(
			GameProviderCode: "365TF",
			PlayerCode: "4un7zvpygn8l",
			ExternalRoundId: "666-1718360132-507250-1",
			ExternalBetId: "084b49342a3711efb2de4cd98f535a89",
			ExternalGameId: "666",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-06-14T18:15:37.015+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-06-14T18:15:58.826+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-06-14T18:15:58.826+08:00"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Loss,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			Bet: 62.700000m,
			ValidBet: 60.820000m,
			Settle: -1.880000m,
			Commission: 0m,
			Turnover: 60.820000m,
			WinLoss: -62.700000m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["txId"] = "084b49342a3711efb2de4cd98f535a89",
				["bonus"] = 0.0,
				["chair"] = 0,
				["tiles"] = "[a3,d8,d2,d3,c2, c7,b7,d11,c9,d6, b13,a6,b2,a1,b3, d5,b6,a7,a8,c6, a2,a4,c13,d4,b8]",
				["banker"] = 0,
				["gameId"] = 666,
				["playId"] = 1,
				["profit"] = -62.7,
				["reward"] = 0.0,
				["status"] = "Pay",
				["userId"] = "4un7zvpygn8l",
				["currency"] = "HKD",
				["outMoney"] = 0.0,
				["validbet"] = 60.82,
				["betAmount"] = 347.34,
				["payAmount"] = 284.64,
				["commission"] = 0.0,
				["createTime"] = "06/14/2024 18:15:37.048",
				["extension1"] = "cardp123",
				["gameNumber"] = "666-1718360132-507250-1",
				["updateTime"] = "06/14/2024 18:15:58.912",
				["betTransTime"] = "06/14/2024 18:15:37.015",
				["payTransTime"] = "06/14/2024 18:15:58.826",
				["realBetAmount"] = 62.7,
				["realPayAmount"] = 0.0
			}.ToJsonString());

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(command.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4un7zvpygn8l",
			Username: "4un7zvpygn8l",
			WalletId: Guid.NewGuid(),
			CurrencyCode: "HKD");
		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 5600012,
			GameCode: "happy_ox",
			GameProviderId: gameProviderId,
			GameProviderCode: "365TF",
			ExternalGameId: "666",
			SubGameProviderCode: "365TF");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(command.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 62.700000m,
			ValidBet: 60.820000m,
			Settle: -1.880000m,
			Commission: 0.0m,
			Turnover: 60.820000m,
			WinLoss: -62.700000m,
			Jackpot: 0.0m);

		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(command.Bet),
			validBet: Arg.Is(command.ValidBet),
			settle: Arg.Is(0.0m),
			commission: Arg.Is(command.Commission),
			turnover: Arg.Is(command.Turnover),
			winLoss: Arg.Is(command.WinLoss),
			jackpot: Arg.Is(command.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var service = new GameBetsRapidTestWorkerCommandService(
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakeService);

		await service.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerPublishCommand(
				GameProviderId: gameProviderId,
				GameProviderCode: command.GameProviderCode,
				GameProviderConfigurationId: gameProviderConfigurationId,
				SubGameProviderCode: gameInfo.SubGameProviderCode,
				GameId: gameInfo.GameId,
				ResellerId: playerInfo.ResellerId,
				ResellerCode: playerInfo.ResellerCode,
				LicenseeId: playerInfo.LicenseeId,
				LicenseeCode: playerInfo.LicenseeCode,
				BrandId: playerInfo.BrandId,
				BrandCode: playerInfo.BrandCode,
				PlayerCode: playerInfo.PlayerCode,
				Username: playerInfo.Username,
				CurrencyCode: playerInfo.CurrencyCode,
				WalletId: playerInfo.WalletId,
				BetState: command.BetState,
				BetType: command.BetType,
				FreeGameState: command.FreeGameState,
				GambleState: command.GambleState,
				CashoutState: command.CashoutState,
				ResettleState: command.ResettleState,
				JackpotState: command.JackpotState,
				ExternalRoundId: command.ExternalRoundId,
				ExternalBetId: command.ExternalBetId,
				ExternalGameId: command.ExternalGameId,
				ExternalCreatedOn: command.ExternalCreatedOn,
				ExternalClosedOn: command.ExternalClosedOn,
				ExternalPayoffOn: command.ExternalPayoffOn,
				ExternalResettleOn: command.ExternalResettleOn,
				CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
				Bet: gameCurrencyInfo.Bet,
				ValidBet: gameCurrencyInfo.ValidBet,
				Settle: gameCurrencyInfo.Settle,
				Commission: gameCurrencyInfo.Commission,
				Turnover: gameCurrencyInfo.Turnover,
				WinLoss: gameCurrencyInfo.WinLoss,
				Jackpot: gameCurrencyInfo.Jackpot,
				AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
