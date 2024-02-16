namespace Tgs.GameRecord.Handle.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }
}
