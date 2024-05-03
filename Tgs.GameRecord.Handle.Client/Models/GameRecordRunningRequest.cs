using System.Text.Json.Nodes;

namespace Tgs.GameRecord.Handle.Client.Models;
public record GameRecordRunningRequest(
	string GameProviderCode,
	string PlayerCode,
	string ExternalRoundId,
	string ExternalBetId,
	string ExternalGameId,
	DateTimeOffset ExternalCreatedOn,
	Common.Protos.BetType BetType,
	decimal Bet,
	decimal ValidBet,
	JsonNode AdditionalData);
