using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Client;

public interface IGameRecordHandleClient
{
    public ValueTask MessageAsync(
        string gameProviderCode,
        string externalRoundId,
        string externalGameId,
        string playerCode,
        GameRecordStatus status,
        GameRecordClientStatus clientStatus,
        DateTimeOffset externalCreatedOn,
        DateTimeOffset? externalClosedOn,
        decimal bet,
        decimal validBet,
        decimal settle,
        decimal commission,
        decimal turnover,
        decimal playerWinLoss,
        string additionalData,
        CancellationToken cancellationToken);
}
