using System.Collections.Generic;

namespace MyWorksheet.ReportService.Services.Scripting;

public class ScriptService : IScriptService
{
    public ScriptService()
    {
        ScriptProvider = new Dictionary<string, IScriptProvider>
        {
            { Javascript, new JavascriptProvider() }
        };
    }

    public const string Javascript = "JS";

    public IDictionary<string, IScriptProvider> ScriptProvider { get; private set; }

    public IScriptProvider GetFor(string reportingKey)
    {
        return ScriptProvider[reportingKey];
    }
}