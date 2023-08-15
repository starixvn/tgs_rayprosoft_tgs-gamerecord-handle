using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.MessageQueue;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordMessageCommandServiceUnitTest
{
    [Fact]
    public async Task Normal()
    {
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new GameRoundsWorkPublishCommand(
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: "KY",
            GameProviderConfigurationId: Guid.NewGuid(),
            SubGameProviderCode: "KY",
            GameId: 1000017,
            GameType: GameRecordGameType.CardGame,
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
            ExchangeRate: 1,
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

        var service = new GameRoundsWorkPublishCommandService(
            fakeMessageSender);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            subject: Arg.Is(NatsConst.GameRoundWorkerPublish),
            data: Arg.Any<ReadOnlyMemory<byte>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
