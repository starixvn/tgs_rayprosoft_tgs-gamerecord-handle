using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json.Nodes;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameBetsRapidTestWorkerPublishCommandServiceUnitTest
{
	[Fact]
	public async Task BetState_Is_Waiting()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Waiting,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Waiting,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Running()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Running,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Running,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Void()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Void,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Void,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Refund()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Refund,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Refund,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Refund_Cashout()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Refund,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.Cashout,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Refund,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.Cashout,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Cancel()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Cancel,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Cancel,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Loss()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Loss,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Loss,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Win()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
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
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Win_Jackpot()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
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
			GameBets.Common.Enums.JackpotState.Jackpot,
			CurrencyRatio: 1.0m,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: -0.08m,
			Jackpot: 0.08m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.Jackpot,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Win_FreeGame()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Win,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.FreeGame,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
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

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.FreeGame,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_Draw()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.Draw,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: 0.0m,
			Jackpot: 0.0m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.Draw,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_HalfWin()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.HalfWin,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: 0.2m,
			Jackpot: -0.2m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.HalfWin,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task BetState_Is_HalfLoss()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 3500044,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ugse0rsnufp",
			Username: "temmie",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalPayoffOn: DateTimeOffset.Parse("2023-08-24T05:39:39"),
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			GameBets.Common.Enums.BetState.HalfLoss,
			GameBets.Common.Enums.BetType.Normal,
			GameBets.Common.Enums.FreeGameState.None,
			GameBets.Common.Enums.GambleState.None,
			GameBets.Common.Enums.CashoutState.None,
			GameBets.Common.Enums.ResettleState.None,
			GameBets.Common.Enums.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 0.4m,
			ValidBet: 0.4m,
			Settle: 0.4m,
			Commission: 0.0m,
			Turnover: 0.4m,
			WinLoss: 0.2m,
			Jackpot: -0.2m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var service = new GameBetsRapidTestWorkerPublishCommandService(
			faceLogger,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameBets.RapidTest.Worker.Common.GameBetsRapidTestWorkerNatsConst.GameBetsRapidTestWorkerRequest),
			data: Arg.Is(new GameBets.RapidTest.Worker.Common.Protos.GameBetsRapidTestWorkerRequest
			{
				GameProviderId = command.GameProviderId,
				GameProviderCode = command.GameProviderCode,
				GameProviderConfigurationId = command.GameProviderConfigurationId,
				SubGameProviderCode = command.SubGameProviderCode,
				GameId = command.GameId,
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				PlayerCode = command.PlayerCode,
				Username = command.Username,
				CurrencyCode = command.CurrencyCode,
				WalletId = command.WalletId,
				ExternalRoundId = command.ExternalRoundId,
				ExternalBetId = command.ExternalBetId,
				ExternalGameId = command.ExternalGameId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = command.ExternalResettleOn.ToTimestamp(),
				BetState = GameBets.RapidTest.Worker.Common.Protos.BetState.HalfLoss,
				BetType = GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
				FreeGameState = GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
				GambleState = GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
				CashoutState = GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
				ResettleState = GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
				JackpotState = GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
				CurrencyRatio = command.CurrencyRatio,
				Bet = command.Bet,
				ValidBet = command.ValidBet,
				Settle = command.Settle,
				Commission = command.Commission,
				Turnover = command.Turnover,
				WinLoss = command.WinLoss,
				Jackpot = command.Jackpot,
				AdditionalData = command.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
