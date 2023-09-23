using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordHandleDataPublishCommandServiceUnitTest
{
    [Fact]
    public async Task Normal()
    {
        var faceLogger = NullLogger<GameRecordHandleDataPublishCommandService>.Instance;
        var fakeSnowflakeWork = Substitute.For<ISnowflakeWork>();
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new GameRecordHandleDataPublishCommand(
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
            Status: GameRecordStatus.Closed,
            ClientStatus: GameRecordClientStatus.Win,
            ExternalRoundId: "50-1691469296-370404138-1",
            ExternalGameId: "910",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 12:34:56.000 +0800"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08 12:35:18.000 +0800"),
            CurrencyRatio: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            PlayerWinLoss: 4,
            AdditionalData: new JsonObject
            {
                ["externalRoundId"] = "50-1691469296-370404138-1"
            }.ToJsonString());

        var service = new GameRecordHandleDataPublishCommandService(
            faceLogger,
            fakeSnowflakeWork,
            fakeMessageSender);

        await service.ExecuteAsync(command: command);

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            subject: Arg.Is(NatsConst.GameRecordHandleDataPublish),
            data: Arg.Any<ReadOnlyMemory<byte>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
