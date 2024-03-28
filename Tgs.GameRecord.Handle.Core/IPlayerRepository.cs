using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IPlayerRepository
{
    public ValueTask<PlayerInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default);
}
