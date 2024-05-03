namespace Tgs.GameRecord.Handle.Core.Models;

public record GameInfo(
	int GameId,
	string GameCode,
	Guid GameProviderId,
	string GameProviderCode,
	string ExternalGameId,
	string SubGameProviderCode);
