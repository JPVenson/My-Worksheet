using System;
using System.Collections.Generic;

namespace MyWorksheet.ReportService.Services.Scripting;

public interface IScriptExecutionInfo
{
    TimeSpan Timeout { get; }
    string Code { get; }
    IDictionary<string, object> EnvironmentData { get; }
}