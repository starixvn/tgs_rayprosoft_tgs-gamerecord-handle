using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameWagerPublishCommandServiceUnitTest
{
    [Fact]
    public async Task Normal()
    {
        var faceLogger = NullLogger<GameWagerPublishCommandService>.Instance;
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new GameWagerPublishCommand(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "KY",
            GameId: 1000017,
            GameCategory: GameCategories.CardGame,
            GameType: GameTypes.CardGame,
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
            WagerState: WagerStates.Win,
            WagerType: WagerTypes.Normal,
            FreeGameState: FreeGameStates.None,
            GambleState: GambleStates.None,
            CashoutState: CashoutStates.None,
            ResettleState: ResettleStates.None,
            ExternalRoundId: "50-1691469296-370404138-1",
            ExternalWagerId: "50-1691469296-370404138-1",
            ExternalGameId: "910",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 12:34:56.000 +0800"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08 12:35:18.000 +0800"),
            ExternalPayoffOn: DateTimeOffset.Parse("2023-08-08 12:35:18.000 +0800"),
            ExternalResettleOn: DateTimeOffset.UnixEpoch,
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4,
            AdditionalData: new JsonObject
            {
                ["externalRoundId"] = "50-1691469296-370404138-1"
            }.ToJsonString());
        var wagerId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _ = fakeSnowflakeWork
            .NextId()
            .Returns(wagerId);

        var service = new GameWagerPublishCommandService(
            faceLogger,
            fakeSnowflakeWork,
            fakeMessageSender);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            subject: Arg.Is(NatsConst.GameRecordHandleDataPublish),
            data: Arg.Any<ReadOnlyMemory<byte>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
