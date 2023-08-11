using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Core.Models;

public record GameModel(
    int GameId,
    string GameCode,
    Guid GameProviderId,
    string GameProviderCode,
    string ExternalGameId,
    GameRecordGameType GameType,
    string SubGameProviderCode);
