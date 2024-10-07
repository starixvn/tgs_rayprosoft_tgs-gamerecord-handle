using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class GameBetsRapidTestWorkerPublishRepositoryUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameBetsRapidTestWorkerPublishRepository>.Instance;
		var fakeOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeClient = Substitute.For<IGameBetsRapidTestWorkerClient>();

		var source = new Core.Models.GameBetsRapidTestWorkerPublish(
			GameProviderId: Guid.NewGuid(),
			GameProviderCode: "AWC_PGS",
			GameProviderConfigurationId: Guid.NewGuid(),
			SubGameProviderCode: "AWC_PGS",
			GameId: 1,
			ResellerId: Guid.NewGuid(),
			ResellerCode: "DEMO",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			CurrencyCode: "CNY",
			WalletId: Guid.NewGuid(),
			ExternalRoundId: "1694585239319900160",
			ExternalBetId: "PG-1694585239319900160",
			ExternalGameId: "PG-SLOT-051",
			ExternalCreatedOn: DateTimeOffset.Parse("2023-08-24T05:39:39Z"),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			ExternalPayoffOn: DateTimeOffset.UnixEpoch,
			ExternalResettleOn: DateTimeOffset.UnixEpoch,
			BetState: GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
			BetType: GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
			FreeGameState: GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None,
			GambleState: GameBets.RapidTest.Worker.Common.Protos.GambleState.None,
			CashoutState: GameBets.RapidTest.Worker.Common.Protos.CashoutState.None,
			ResettleState: GameBets.RapidTest.Worker.Common.Protos.ResettleState.None,
			JackpotState: GameBets.RapidTest.Worker.Common.Protos.JackpotState.None,
			CurrencyRatio: 1.0m,
			Bet: 4.0m,
			ValidBet: 2.0m,
			Settle: 2.38m,
			WinLoss: 0.38m,
			Turnover: 1.62m,
			Commission: 0.1m,
			Contribute: 0.01m,
			Jackpot: 0.123m,
			AdditionalData: new JsonObject
			{
				["ExternalRoundId"] = "PG-1694585239319900160",
				["PlatformTxId"] = "1694585239319900160"
			}.ToJsonString());

		var sut = new GameBetsRapidTestWorkerPublishRepository(
			faceLogger,
			fakeOptions,
			fakeClient);

		await sut.PublishAsync(source);

		_ = fakeClient
			.Received(1)
			.HandleAsync(
			source: Arg.Is<GameBets.RapidTest.Worker.Common.ViewModels.GameBetRapidTestWorkerHandleViewModel>(compare =>
				compare.GameProviderId == source.GameProviderId &&
				compare.GameProviderCode == source.GameProviderCode &&
				compare.GameProviderConfigurationId == source.GameProviderConfigurationId &&
				compare.SubGameProviderCode == source.SubGameProviderCode &&
				compare.GameId == source.GameId &&
				compare.ResellerId == source.ResellerId &&
				compare.ResellerCode == source.ResellerCode &&
				compare.LicenseeId == source.LicenseeId &&
				compare.LicenseeCode == source.LicenseeCode &&
				compare.BrandId == source.BrandId &&
				compare.BrandCode == source.BrandCode &&
				compare.PlayerCode == source.PlayerCode &&
				compare.Username == source.Username &&
				compare.CurrencyCode == source.CurrencyCode &&
				compare.WalletId == source.WalletId &&
				compare.ExternalRoundId == source.ExternalRoundId &&
				compare.ExternalBetId == source.ExternalBetId &&
				compare.ExternalGameId == source.ExternalGameId &&
				compare.ExternalCreatedOn == source.ExternalCreatedOn &&
				compare.ExternalClosedOn == source.ExternalClosedOn &&
				compare.ExternalPayoffOn == source.ExternalPayoffOn &&
				compare.ExternalResettleOn == source.ExternalResettleOn &&
				compare.BetState == source.BetState &&
				compare.BetType == source.BetType &&
				compare.FreeGameState == source.FreeGameState &&
				compare.GambleState == source.GambleState &&
				compare.CashoutState == source.CashoutState &&
				compare.ResettleState == source.ResettleState &&
				compare.JackpotState == source.JackpotState &&
				compare.CurrencyRatio == source.CurrencyRatio &&
				compare.Bet == source.Bet &&
				compare.ValidBet == source.ValidBet &&
				compare.Settle == source.Settle &&
				compare.WinLoss == source.WinLoss &&
				compare.Turnover == source.Turnover &&
				compare.Commission == source.Commission &&
				compare.Contribute == source.Contribute &&
				compare.Jackpot == source.Jackpot &&
				compare.AdditionalData != null),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}