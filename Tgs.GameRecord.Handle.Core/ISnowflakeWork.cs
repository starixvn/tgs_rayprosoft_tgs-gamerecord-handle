using System.Net;

namespace Tgs.GameRecord.Handle.Core;

public interface ISnowflakeWork
{
    long GetDatacenterId();

    IPAddress GetIPAddress();

    long GetWagerId();

    long GetWorkerId();
}
