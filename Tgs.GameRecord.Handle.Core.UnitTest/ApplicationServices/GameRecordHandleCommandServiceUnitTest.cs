using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.UnitTest.ApplicationServices;

public class GameRecordHandleCommandServiceUnitTest
{
    [Fact]
    public async Task Normal()
    {
        var fakeGameRecordInfo = Substitute.For<IGetService<GameRecordInfoQuery, GameRecordInfoResult>>();
        var fakeGameRecordMessage = Substitute.For<ICommandService<GameRoundsWorkPublishCommand>>();

        var command = new GameRecordHandleCommand(
            GameProviderCode: "KY",
            PlayerCode: "4ucmqplxb029",
            Status: GameRecordStatus.Closed,
            ClientStatus: GameRecordClientStatus.Win,
            ExternalRoundId: "50-1691469296-370404138-1",
            ExternalGameId: "910",
            ExternalCreatedOn: DateTimeOffset.Parse("2023-08-08 12:34:56.000 +0800"),
            ExternalClosedOn: DateTimeOffset.Parse("2023-08-08 12:35:18.000 +0800"),
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

        var gameRecordInfoQuery = new GameRecordInfoQuery(
            GameProviderCode: command.GameProviderCode,
            ExternalRoundId: command.ExternalRoundId,
            ExternalGameId: command.ExternalGameId,
            PlayerCode: command.PlayerCode,
            Status: command.Status,
            ClientStatus: command.ClientStatus,
            ExternalCreatedOn: command.ExternalCreatedOn,
            ExternalClosedOn: command.ExternalClosedOn,
            Bet: command.Bet,
            ValidBet: command.ValidBet,
            Settle: command.Settle,
            Commission: command.Commission,
            Turnover: command.Turnover,
            PlayerWinLoss: command.PlayerWinLoss);
        var gameRecordInfoResult = new GameRecordInfoResult(
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
            PlayerWinLoss: 4);
        _ = fakeGameRecordInfo
            .GetAsync(
            query: Arg.Is(gameRecordInfoQuery),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameRecordInfoResult);

        var service = new GameRecordHandleCommandService(
            fakeGameRecordInfo,
            fakeGameRecordMessage);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        var gameMessageCommand = new GameRoundsWorkPublishCommand(
            GameProviderId: gameRecordInfoResult.GameProviderId,
            GameProviderCode: gameRecordInfoResult.GameProviderCode,
            GameProviderConfigurationId: gameRecordInfoResult.GameProviderConfigurationId,
            SubGameProviderCode: gameRecordInfoResult.SubGameProviderCode,
            GameId: gameRecordInfoResult.GameId,
            GameType: gameRecordInfoResult.GameType,
            ResellerId: gameRecordInfoResult.ResellerId,
            ResellerCode: gameRecordInfoResult.ResellerCode,
            LicenseeId: gameRecordInfoResult.LicenseeId,
            LicenseeCode: gameRecordInfoResult.LicenseeCode,
            BrandId: gameRecordInfoResult.BrandId,
            BrandCode: gameRecordInfoResult.BrandCode,
            PlayerCode: gameRecordInfoResult.PlayerCode,
            Username: gameRecordInfoResult.Username,
            CurrencyCode: gameRecordInfoResult.CurrencyCode,
            WalletId: gameRecordInfoResult.WalletId,
            Status: gameRecordInfoResult.Status,
            ClientStatus: gameRecordInfoResult.ClientStatus,
            ExternalRoundId: gameRecordInfoResult.ExternalRoundId,
            ExternalGameId: gameRecordInfoResult.ExternalGameId,
            ExternalCreatedOn: gameRecordInfoResult.ExternalCreatedOn,
            ExternalClosedOn: gameRecordInfoResult.ExternalClosedOn,
            ExchangeRate: gameRecordInfoResult.ExchangeRate,
            Bet: gameRecordInfoResult.Bet,
            ValidBet: gameRecordInfoResult.ValidBet,
            Settle: gameRecordInfoResult.Settle,
            Commission: gameRecordInfoResult.Commission,
            Turnover: gameRecordInfoResult.Turnover,
            PlayerWinLoss: gameRecordInfoResult.PlayerWinLoss,
            AdditionalData: command.AdditionalData);

        _ = fakeGameRecordMessage
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(gameMessageCommand),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
