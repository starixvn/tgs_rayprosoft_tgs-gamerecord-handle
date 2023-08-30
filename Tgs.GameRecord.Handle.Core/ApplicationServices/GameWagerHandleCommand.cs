using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

public record GameWagerHandleCommand(
    string GameProviderCode,
    string PlayerCode,
    WagerStates WagerState,
    WagerTypes WagerType,
    FreeGameStates FreeGameState,
    GambleStates GambleState,
    CashoutStates CashoutState,
    ResettleStates ResettleState,
    string ExternalRoundId,
    string ExternalWagerId,
    string ExternalGameId,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    DateTimeOffset ExternalPayoffOn,
    DateTimeOffset ExternalResettleOn,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss,
    string AdditionalData);
