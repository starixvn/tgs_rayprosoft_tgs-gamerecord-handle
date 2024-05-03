using System.Text.Json.Nodes;

namespace Tgs.GameRecord.Handle.Client.Models;
public record GameRecordRequest(
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
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	decimal Jackpot,
	JsonNode AdditionalData);
