using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public interface ITask
{
    Task Run(TaskContext context);
    bool IsBusy { get; }

    string NamedTask { get; }

    // TODO MaxRunTime - maximum time this should be able to run for before scheduler thinks its bombed out
    // TODO Priority - used to prioritise scheduledtasks
    // TODO IdleWait - how long the scheduler has to be idle after starting before this task is "run" for the first time
}

public class TaskContext
{
    public TaskContext(IAppLogger logger)
    {
        Logger = logger;
    }

    public IAppLogger Logger { get; }
    public bool IsManualInvocation { get; set; }
}