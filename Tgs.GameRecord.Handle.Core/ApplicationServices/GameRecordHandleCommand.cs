using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

public record GameRecordHandleCommand(
    string GameProviderCode,
    string ExternalRoundId,
    string ExternalGameId,
    string PlayerCode,
    GameRecordStatus Status,
    GameRecordClientStatus ClientStatus,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss,
    string AdditionalData);
