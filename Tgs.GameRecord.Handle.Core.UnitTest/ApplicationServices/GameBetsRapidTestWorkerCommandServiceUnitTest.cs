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
}
