using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using Exception = System.Exception;

namespace MyWorksheet.Shared.Helper;

public static class AsyncExtensions
{
    public static ILogger AppLogger { get; set; }

    public static void AttachAsyncHandler(this Task task,
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerNo = 0)
    {
        var callerStack = SynchronizationContext.Current;
        task.ContinueWith((parentTask) =>
        {
            if (parentTask.IsFaulted && parentTask.Exception != null)
            {
                callerStack.Post(e => { throw (Exception)e; }, new AggregateException(
                    $"The task created at \'{callerMemberName}\':{callerNo} has faulted with: " +
                    parentTask.Exception, parentTask.Exception));
            }
        });
    }

    public static void AttachNonVerboseAsyncHandler(this Task task,
        [CallerMemberName] string callerMemberName = "",
        [CallerLineNumber] int callerNo = 0)
    {
        var callerStack = SynchronizationContext.Current;
        task.ContinueWith((parentTask) =>
        {
            if (parentTask.IsFaulted && parentTask.Exception != null)
            {
                AppLogger
                    .LogError("Non verbose task has failed.", LoggerCategories.Server.ToString(), new Dictionary<string, string>()
                    {
                        {"Exception", parentTask.Exception.ToString()},
                        {"Exception.CallerMember", callerMemberName.ToString()},
                        {"Exception.CallerNo", callerNo.ToString()},
                    });
            }
        });
    }
}