using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameBetsRapidTestWorkerPublishRepository
{
	ValueTask PublishAsync(GameBetsRapidTestWorkerPublish source, CancellationToken cancellationToken = default);
}
