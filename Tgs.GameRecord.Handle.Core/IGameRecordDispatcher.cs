using Tgs.GameRecord.Handle.Core.ApplicationServices;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameRecordDispatcher
{
	ValueTask PushCommandAsync(GameRecordCommandRequest command, CancellationToken cancellationToken = default);

	void ExtendExpiry(string gameProviderCode, string externalGameId);
}
