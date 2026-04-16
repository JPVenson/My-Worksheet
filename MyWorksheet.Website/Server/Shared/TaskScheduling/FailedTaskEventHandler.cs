using System;

namespace MyWorksheet.Website.Server.Shared.TaskScheduling;

public delegate void FailedTaskEventHandler(ITask task, Exception exception, ExceptionHandler handler);