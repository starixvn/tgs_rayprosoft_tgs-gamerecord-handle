using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

public record GameWagerInfoQuery(
    string GameProviderCode,
    string PlayerCode,
    string ExternalGameId,
    ClosedStates ClosedState,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss);
