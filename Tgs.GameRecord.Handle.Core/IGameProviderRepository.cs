namespace Tgs.GameRecord.Handle.Core;

public interface IGameProviderRepository
{
	ValueTask<Guid> GetIdAsync(string code, CancellationToken cancellationToken = default);

	IAsyncEnumerable<string> QueryByCodeAsync(CancellationToken cancellationToken = default);
}
