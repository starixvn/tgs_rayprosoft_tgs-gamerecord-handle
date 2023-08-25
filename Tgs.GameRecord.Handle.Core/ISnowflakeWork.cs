namespace Tgs.GameRecord.Handle.Core;

public interface ISnowflakeWork
{
    long GetDatacenterId();

    long GetWorkerId();

    long NextId();
}
