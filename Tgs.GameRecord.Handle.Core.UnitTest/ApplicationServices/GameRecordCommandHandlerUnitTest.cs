using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameRecord.Handle.Core.ApplicationServices;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordCommandHandlerUnitTest
{
	[Fact]
	public async Task BetStates_Is_Waiting()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Waiting,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());
		;

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.Bet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Waiting &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Running()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Running,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Running &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Void()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Void,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Void &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Refund()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Refund,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Refund &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Refund_Cashout()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Refund,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.Cashout,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Refund &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.Cashout &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Cancel()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Cancel,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(0.0m),
			turnover: Arg.Is(0.0m),
			commission: Arg.Is(0.0m),
			contribute: Arg.Is(0.0m),
			jackpot: Arg.Is(0.0m),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Cancel &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Lose()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Loss,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: -0.4m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			WinLoss: -0.4m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Loss &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Win()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.38m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Win &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Win_FreeGame()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.FreeGame,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.5m,
			WinLoss: 0.1m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.5m,
			WinLoss: 0.1m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Win &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.FreeGame &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Win_Jackpot()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.Jackpot,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 100.0m,
			WinLoss: 99.6m,
			Turnover: 0.4m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 100.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.401m,
			Settle: 100.0m,
			WinLoss: 99.6m,
			Turnover: 0.4m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 100.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Win &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.Jackpot &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Draw()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Draw,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			WinLoss: 0.0m,
			Turnover: 0.0m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Draw &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_HalfWin()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.HalfWin,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			WinLoss: -0.2m,
			Turnover: 0.2m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			WinLoss: -0.2m,
			Turnover: 0.2m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.HalfWin &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_HalfLose()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.HalfLoss,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			WinLoss: -0.2m,
			Turnover: 0.2m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "PG-SLOT-051",
			GameProviderId: gameProviderId,
			GameProviderCode: "AWC_PGS",
			ExternalGameId: "PG-SLOT-051",
			SubGameProviderCode: "AWC_PGS");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			WinLoss: -0.2m,
			Turnover: 0.2m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(request.Settle),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.HalfLoss &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ExternalRoundId_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: string.Empty,
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ExternalBetId_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "PG-1694585239319900160",
			ExternalBetId: string.Empty,
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_will_take_more_than_60_days_to_settle()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2024, 02, 25, 06, 33, 23, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_provider_does_not_exist()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<Guid>(new ArgumentNullException()));

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_player_does_not_exist()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		_ = fakePlayerRepository
			.GetAsync(
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<Models.PlayerInfo?>(new ArgumentNullException()));

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task The_game_provider_configuration_ID_does_not_exist()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<Guid>(new ArgumentNullException()));

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_does_not_exist()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Win,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			WinLoss: -0.08m,
			Turnover: 0.4m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
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
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<Models.GameInfo?>(new ArgumentNullException()));

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request, token));

		_ = fakePublishRepository
			.Received(0)
			.PublishAsync(
			source: Arg.Any<Models.GameBetsRapidTestWorkerPublish>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Settle_Is_Negative()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakeBrandGameProviderRepository = Substitute.For<IBrandGameProviderRepository>();
		var fakeGameRepository = Substitute.For<IGameRepository>();
		var fakeGameCurrencyRepository = Substitute.For<IGameCurrencyRepository>();
		var fakePublishRepository = Substitute.For<IGameBetsRapidTestWorkerPublishRepository>();

		var request = new GameRecordCommandRequest(
			GameProviderCode: "365TF",
			PlayerCode: "4un7zvpygn8l",
			ExternalRoundId: "666-1718360132-507250-1",
			ExternalBetId: "084b49342a3711efb2de4cd98f535a89",
			ExternalGameId: "666",
			ExternalCreatedOn: DateTimeOffset.Parse("2024-06-14T18:15:37.015+08:00"),
			ExternalClosedOn: DateTimeOffset.Parse("2024-06-14T18:15:58.826+08:00"),
			ExternalPayoffOn: DateTimeOffset.Parse("2024-06-14T18:15:58.826+08:00"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: Common.Protos.BetState.Loss,
			BetType: Common.Protos.BetType.Normal,
			FreeGameState: Common.Protos.FreeGameState.None,
			GambleState: Common.Protos.GambleState.None,
			CashoutState: Common.Protos.CashoutState.None,
			ResettleState: Common.Protos.ResettleState.None,
			JackpotState: Common.Protos.JackpotState.None,
			Bet: 62.700000m,
			ValidBet: 60.820000m,
			Settle: -1.880000m,
			WinLoss: -62.700000m,
			Turnover: 60.820000m,
			Commission: 0.0m,
			Contribute: 0.0m,
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

		var currentTime = new DateTimeOffset(2024, 06, 14, 18, 15, 37, TimeSpan.FromHours(+08));
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var gameProviderId = Guid.NewGuid();
		_ = fakeGameProviderRepository.GetIdAsync(
			code: Arg.Is(request.GameProviderCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderId);

		var playerInfo = new Models.PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
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
			playerCode: Arg.Is(request.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var gameProviderConfigurationId = Guid.NewGuid();
		_ = fakeBrandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			gameProviderId: Arg.Is(gameProviderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameProviderConfigurationId);

		var gameInfo = new Models.GameInfo(
			GameId: 1,
			GameCode: "666",
			GameProviderId: gameProviderId,
			GameProviderCode: "365TF",
			ExternalGameId: "666",
			SubGameProviderCode: "365TF");
		_ = fakeGameRepository.GetAsync(
			gameProviderId: Arg.Is(gameProviderId),
			externalGameId: Arg.Is(request.ExternalGameId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameInfo);

		var gameCurrencyInfo = new Models.GameCurrencyInfo(
			CurrencyRatio: 1,
			Bet: 62.700000m,
			ValidBet: 60.820000m,
			Settle: -1.880000m,
			WinLoss: -62.700000m,
			Turnover: 60.820000m,
			Commission: 0.0m,
			Contribute: 0.0m,
			Jackpot: 0.0m);
		_ = fakeGameCurrencyRepository.GetAsync(
			gameProviderCode: Arg.Is(gameInfo.GameProviderCode),
			currencyCode: Arg.Is(playerInfo.CurrencyCode),
			bet: Arg.Is(request.Bet),
			validBet: Arg.Is(request.ValidBet),
			settle: Arg.Is(0.0m),
			winLoss: Arg.Is(request.WinLoss),
			turnover: Arg.Is(request.Turnover),
			commission: Arg.Is(request.Commission),
			contribute: Arg.Is(request.Contribute),
			jackpot: Arg.Is(request.Jackpot),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(gameCurrencyInfo);

		var sut = new GameRecordCommandHandler(
			fakeTimeProvider,
			fakeGameProviderRepository,
			fakePlayerRepository,
			fakeBrandGameProviderRepository,
			fakeGameRepository,
			fakeGameCurrencyRepository,
			fakePublishRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakePublishRepository
			.Received(1)
			.PublishAsync(
			source: Arg.Is<Models.GameBetsRapidTestWorkerPublish>(compare =>
				compare.GameProviderId == gameProviderId &&
				compare.GameProviderCode == request.GameProviderCode &&
				compare.GameProviderConfigurationId == gameProviderConfigurationId &&
				compare.SubGameProviderCode == gameInfo.SubGameProviderCode &&
				compare.GameId == gameInfo.GameId &&
				compare.ResellerId == playerInfo.ResellerId &&
				compare.ResellerCode == playerInfo.ResellerCode &&
				compare.LicenseeId == playerInfo.LicenseeId &&
				compare.LicenseeCode == playerInfo.LicenseeCode &&
				compare.BrandId == playerInfo.BrandId &&
				compare.BrandCode == playerInfo.BrandCode &&
				compare.PlayerCode == playerInfo.PlayerCode &&
				compare.Username == playerInfo.Username &&
				compare.CurrencyCode == playerInfo.CurrencyCode &&
				compare.WalletId == playerInfo.WalletId &&
				compare.ExternalRoundId == request.ExternalRoundId &&
				compare.ExternalBetId == request.ExternalBetId &&
				compare.ExternalGameId == request.ExternalGameId &&
				compare.ExternalCreatedOn == request.ExternalCreatedOn &&
				compare.ExternalClosedOn == request.ExternalClosedOn &&
				compare.ExternalPayoffOn == request.ExternalPayoffOn &&
				compare.ExternalResettleOn == request.ExternalResettleOn &&
				compare.BetState == GameBets.RapidTest.Worker.Common.Protos.BetState.Loss &&
				compare.BetType == GameBets.RapidTest.Worker.Common.Protos.BetType.Normal &&
				compare.FreeGameState == GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None &&
				compare.GambleState == GameBets.RapidTest.Worker.Common.Protos.GambleState.None &&
				compare.CashoutState == GameBets.RapidTest.Worker.Common.Protos.CashoutState.None &&
				compare.ResettleState == GameBets.RapidTest.Worker.Common.Protos.ResettleState.None &&
				compare.JackpotState == GameBets.RapidTest.Worker.Common.Protos.JackpotState.None &&
				compare.CurrencyRatio == gameCurrencyInfo.CurrencyRatio &&
				compare.Bet == gameCurrencyInfo.Bet &&
				compare.ValidBet == gameCurrencyInfo.ValidBet &&
				compare.Settle == gameCurrencyInfo.Settle &&
				compare.WinLoss == gameCurrencyInfo.WinLoss &&
				compare.Turnover == gameCurrencyInfo.Turnover &&
				compare.Commission == gameCurrencyInfo.Commission &&
				compare.Contribute == gameCurrencyInfo.Contribute &&
				compare.Jackpot == gameCurrencyInfo.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
