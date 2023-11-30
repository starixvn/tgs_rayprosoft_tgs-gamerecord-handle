using System.Net;
using Microsoft.Extensions.Options;
using Snowflake.Core;

namespace Tgs.GameRecord.Handle.Core;

internal class SnowflakeWork(
    IdWorker idWorker) : ISnowflakeWork
{
    public long GetDatacenterId()
        => idWorker.DatacenterId;

    public long GetId()
        => idWorker.NextId();

    public long GetWorkerId()
        => idWorker.WorkerId;
}
