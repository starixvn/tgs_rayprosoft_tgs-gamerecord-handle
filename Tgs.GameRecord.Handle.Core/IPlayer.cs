using Tgs.GameCommand.Common;

namespace Tgs.GameRecord.Handle.Core;

public interface IPlayer
{
    public ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default);
}
