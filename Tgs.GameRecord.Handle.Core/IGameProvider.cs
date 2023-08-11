namespace Tgs.GameRecord.Handle.Core;

public interface IGameProvider
{
    ValueTask<Guid> GetIdAsync(
        string code,
        CancellationToken cancellationToken = default);
}
