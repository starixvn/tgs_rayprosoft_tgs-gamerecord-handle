using System.Net;

namespace Tgs.GameRecord.Handle.Core;

public interface ISnowflakeWork
{
    long GetDatacenterId();

    long GetId();

    long GetWorkerId();
}
