using System;
using System.Collections.Generic;

namespace MyWorksheet.ReportService.Services.Scripting;

public class ScriptExecutionInfo : IScriptExecutionInfo
{
    public ScriptExecutionInfo(TimeSpan timeout, string code)
    {
        Timeout = timeout;
        Code = code;
        EnvironmentData = new Dictionary<string, object>();
    }

    public TimeSpan Timeout { get; }
    public string Code { get; }
    public IDictionary<string, object> EnvironmentData { get; }
}