using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameBets.RapidTest.Worker.Client;

using Valhalla.MessageQueue;

using Tgs.GameRecord.Handle.IntegrationTest;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common;

namespace Tgs.GameRecord.Handle.IntegrationTest;

public class GameRecordHandleClientUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var source = new Common.ViewModels.GameRecordHandleViewModel
		{
			GameProviderCode = "LY",
			PlayerCode = "test",
			ExternalRoundId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
			ExternalBetId = Guid.NewGuid().ToString(),
			ExternalGameId = "901",
			ExternalCreatedOn = DateTimeOffset.UtcNow.AddSeconds(-10),
			ExternalClosedOn = DateTimeOffset.UtcNow,
			ExternalPayoffOn = DateTimeOffset.UtcNow,
			ExternalResettleOn = DateTimeOffset.UnixEpoch,
			BetState = Common.Protos.BetState.Win,
			BetType = Common.Protos.BetType.Normal,
			FreeGameState = Common.Protos.FreeGameState.None,
			GambleState = Common.Protos.GambleState.None,
			CashoutState = Common.Protos.CashoutState.None,
			ResettleState = Common.Protos.ResettleState.None,
			JackpotState = Common.Protos.JackpotState.None,
			Bet = 2.0m,
			ValidBet = 2.0m,
			Settle = 4.0m,
			Commission = 0.0m,
			Turnover = 2.0m,
			WinLoss = 2.0m,
			Jackpot = 0.0m,
			AdditionalData = "{\"AllBet\": 2, \"GameID\": \"888-1722937124-92240583-21\", \"KindID\": 910, \"Profit\": 2, \"ChairID\": 0, \"Revenue\": 0, \"TableID\": 182020001, \"Accounts\": \"200995_4un4b9l34304\", \"LineCode\": \"200995_GPI\", \"ServerID\": 9101, \"CardValue\": \"0c32050b06001\", \"CellScore\": 2, \"ChannelID\": 200995, \"UserCount\": 1, \"GameEndTime\": \"2024-08-06T17:39:03+08:00\", \"GameStartTime\": \"2024-08-06T17:38:44+08:00\"}"
		};

		var fakeMessageSender = Substitute.For<IMessageSender>();
		var web = new GameRecordHandleApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeMessageSender)));

		var httpClient = web.CreateDefaultClient();
		var client = new GameRecordHandleClient(httpClient);

		await client.HandleAsync(source);

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameRecordHandleNatsConst.GameRecordHandleRequest),
			data: Arg.Is(new Common.Protos.GameRecordHandleRequest
			{
				GameProviderCode = source.GameProviderCode,
				PlayerCode = source.PlayerCode,
				ExternalRoundId = source.ExternalRoundId,
				ExternalBetId = source.ExternalBetId,
				ExternalGameId = source.ExternalGameId,
				ExternalCreatedOn = source.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = source.ExternalClosedOn.ToTimestamp(),
				ExternalPayoffOn = source.ExternalPayoffOn.ToTimestamp(),
				ExternalResettleOn = source.ExternalResettleOn.ToTimestamp(),
				BetState = source.BetState,
				BetType = source.BetType,
				FreeGameState = source.FreeGameState,
				GambleState = source.GambleState,
				CashoutState = source.CashoutState,
				ResettleState = source.ResettleState,
				JackpotState = source.JackpotState,
				Bet = source.Bet,
				ValidBet = source.ValidBet,
				Settle = source.Settle,
				Commission = source.Commission,
				Turnover = source.Turnover,
				WinLoss = source.WinLoss,
				Jackpot = source.Jackpot,
				AdditionalData = source.AdditionalData
			}),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameRecordHandleNatsConst.GameRecordHandleExpiryRequest),
			data: Arg.Is(new Common.Protos.GameRecordHandleExpiryRequest
			{
				GameProviderCode = source.GameProviderCode,
				ExternalGameId = source.ExternalGameId
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
