using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameRepository
{
    public ValueTask<GameInfo?> GetAsync(Guid gameProviderId, string externalGameId, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<GameExternaldInfo> QueryByExternaldAsync(string gameProviderCode, CancellationToken cancellationToken = default);
}
