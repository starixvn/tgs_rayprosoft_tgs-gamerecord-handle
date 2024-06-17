using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordHandleCommandServiceUnitTest
{
	[Fact]
	public async Task BetStates_Is_Waiting()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Waiting,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
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

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Waiting,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Running()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Running,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Running,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Void()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Void,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Void,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Refund()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Refund,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Refund,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Refund_Cashout()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Refund,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.Cashout,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Refund,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.Cashout,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Cancel()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Cancel,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Cancel,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Lose()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Loss,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.4m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Loss,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Win()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Win,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Win_FreeGame()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.FreeGame,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.5m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: 0.1m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Win,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.FreeGame,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_Draw()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Draw,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.Draw,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_HalfWin()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.HalfWin,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.2m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.HalfWin,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetStates_Is_HalfLose()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.HalfLoss,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.2m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.2m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2023, 08, 24, 05, 39, 39, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		await sut.ExecuteAsync(command: command);

		_ = fakeGameRecordDispatcher
			.Received(1)
			.PushCommandAsync(
			command: Arg.Is(new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: GameBets.Common.Enums.BetState.HalfLoss,
			   BetType: GameBets.Common.Enums.BetType.Normal,
			   FreeGameState: GameBets.Common.Enums.FreeGameState.None,
			   GambleState: GameBets.Common.Enums.GambleState.None,
			   CashoutState: GameBets.Common.Enums.CashoutState.None,
			   ResettleState: GameBets.Common.Enums.ResettleState.None,
			   JackpotState: GameBets.Common.Enums.JackpotState.None,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ExternalRoundId_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: string.Empty,
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync(command: command).AsTask());

		_ = fakeGameRecordDispatcher
			.Received(0)
			.PushCommandAsync(
			command: Arg.Any<GameBetsRapidTestWorkerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ExternalBetId_Is_Empty()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "PG-1694585239319900160",
			ExternalBetId: string.Empty,
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		_ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ExecuteAsync(command: command).AsTask());

		_ = fakeGameRecordDispatcher
			.Received(0)
			.PushCommandAsync(
			command: Arg.Any<GameBetsRapidTestWorkerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_game_bet_will_take_more_than_180_days_to_settle()
	{
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeGameRecordDispatcher = Substitute.For<IGameRecordDispatcher>();

		var command = new GameRecordHandleCommand(
			GameProviderCode: "AWC_PGS",
			PlayerCode: "4ugse0rsnufp",
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: BetState.Win,
			BetType: BetType.Normal,
			FreeGameState: FreeGameState.None,
			GambleState: GambleState.None,
			CashoutState: CashoutState.None,
			ResettleState: ResettleState.None,
			JackpotState: JackpotState.None,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.32m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var currentTime = new DateTimeOffset(2024, 02, 25, 06, 33, 23, TimeSpan.Zero);
		_ = fakeTimeProvider.GetUtcNow()
		   .Returns(currentTime);

		var sut = new GameRecordHandleCommandService(
			fakeTimeProvider,
			fakeGameRecordDispatcher);

		_ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
			 () => sut.ExecuteAsync(
				 command: command)
			 .AsTask());

		_ = fakeGameRecordDispatcher
			.Received(0)
			.PushCommandAsync(
			command: Arg.Any<GameBetsRapidTestWorkerCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
