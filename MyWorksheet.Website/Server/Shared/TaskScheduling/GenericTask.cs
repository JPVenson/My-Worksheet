using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public class GenericTask : ITask
{
    public GenericTask(string namedTask, Action<IAppLogger> taskItem, Action<Exception> onFailed)
    {
        TaskItem = taskItem;
        OnFailed = onFailed;
        NamedTask = namedTask;
    }

    public Action<IAppLogger> TaskItem { get; private set; }
    public Action<Exception> OnFailed { get; private set; }
    public async Task Run(TaskContext context)
    {
        IsBusy = true;
        try
        {
            TaskItem(context.Logger);
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            OnFailed(e);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool IsBusy { get; private set; }
    public string NamedTask { get; private set; }
}