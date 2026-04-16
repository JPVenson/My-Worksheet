using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Util;

public class TaskList : IAsyncDisposable
{
    public TaskList()
    {
        _tasks = new List<Task>();
    }

    private IList<Task> _tasks;

    public void Add(Task task)
    {
        _tasks.Add(task);
    }

    public void Add(ValueTask task)
    {
        _tasks.Add(task.AsTask());
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_tasks);
    }
}