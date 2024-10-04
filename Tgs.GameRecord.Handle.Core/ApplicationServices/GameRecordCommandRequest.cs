using MediatR;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;
public record GameRecordCommandRequest(
	string GameProviderCode,
	string PlayerCode,
	string ExternalRoundId,
	string ExternalBetId,
	string ExternalGameId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	DateTimeOffset ExternalPayoffOn,
	DateTimeOffset ExternalResettleOn,
	Common.Protos.BetState BetState,
	Common.Protos.BetType BetType,
	Common.Protos.FreeGameState FreeGameState,
	Common.Protos.GambleState GambleState,
	Common.Protos.CashoutState CashoutState,
	Common.Protos.ResettleState ResettleState,
	Common.Protos.JackpotState JackpotState,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal WinLoss,
	decimal Turnover,
	decimal Commission,
	decimal Contribute,
	decimal Jackpot,
	string AdditionalData) : IRequest;
