namespace Tgs.GameRecord.Handle.Client;

public interface IGameRecordHandleClient
{
	public ValueTask HandleAsync(Common.ViewModels.GameRecordHandleViewModel source, CancellationToken cancellationToken = default);
}
