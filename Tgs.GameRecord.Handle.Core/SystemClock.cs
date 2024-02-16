using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Core;

internal class SystemClock : ISystemClock
{
	public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}
