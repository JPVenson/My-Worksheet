using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public static class TaskTimerManager
{
    static TaskTimerManager()
    {
        SharedTimers = new ConcurrentDictionary<int, TaskTimer>();
    }

    public static ConcurrentDictionary<int, TaskTimer> SharedTimers { get; private set; }

    public static TaskTimer GetOrCreateFromCache(int delay, string tag, IAppLogger logger)
    {
        return SharedTimers.GetOrAdd(delay, f => new TaskTimer(f, tag, logger));
    }

    public static IEnumerable<TaskTimer> GetFromCache(string tag)
    {
        return SharedTimers.Where(e => e.Value.Tag == tag).Select(e => e.Value);
    }
}