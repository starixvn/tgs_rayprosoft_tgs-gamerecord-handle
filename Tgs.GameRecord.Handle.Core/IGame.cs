using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGame
{
    public ValueTask<GameModel> GetAsync(
        Guid gameProviderId,
        string externaGamelId,
        CancellationToken cancellationToken = default);
}
