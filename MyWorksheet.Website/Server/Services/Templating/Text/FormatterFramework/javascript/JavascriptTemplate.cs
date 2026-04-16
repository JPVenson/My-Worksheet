using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.ReportService.Services.Scripting;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using Morestachio.Formatter.Framework;
using Morestachio.Helper;

namespace MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;

public class JavascriptTemplate : ITemplate
{
    private readonly string _code;
    private readonly Stream _resultStream;
    private readonly IEnumerable<MorestachioFormatterModel> _formatter;
    private readonly IScriptService _scriptService;
    public IDictionary<string, object> EnviormentData { get; private set; }

    public JavascriptTemplate(
        string code,
        IDictionary<string, object> enviormentData,
        Stream resultStream,
        IEnumerable<MorestachioFormatterModel> formatter,
        IScriptService scriptService)
    {
        _code = code;
        _resultStream = resultStream;
        _formatter = formatter;
        _scriptService = scriptService;
        EnviormentData = enviormentData;
    }

    public async Task<string> RenderTemplateAsync(IDictionary<string, object> arguments, UserContext userContext)
    {
        using (var template = await RenderTemplateStreamAsync(arguments, userContext))
        {
            return template.Stringify(true, Encoding.Default);
        }
    }

    public async Task<Stream> RenderTemplateStreamAsync(IDictionary<string, object> arguments, UserContext userContext)
    {
        //var options = JavaScriptRuntimeAttributes.AllowScriptInterrupt |
        //			  JavaScriptRuntimeAttributes.DisableBackgroundWork | JavaScriptRuntimeAttributes.DisableEval |
        //			  JavaScriptRuntimeAttributes.DisableNativeCodeGeneration;
        //await Task.CompletedTask;
        var streamWriter = new JsDocumentStreamWrapper(_resultStream);
        try
        {
            var scriptProvider = _scriptService.GetFor(ScriptService.Javascript);

            var reportScriptService = new ScriptExecutionInfo(TimeSpan.FromMinutes(2), _code);
            foreach (var argument in arguments)
            {
                reportScriptService.EnvironmentData[argument.Key] = argument.Value;
            }
            reportScriptService.EnvironmentData.Add("Out", streamWriter);
            reportScriptService.EnvironmentData.Add("Formatter", new FormatterWrapper(_formatter));
            await scriptProvider.ExecuteScript(reportScriptService);
        }
        finally
        {
            streamWriter.Dispose();
        }

        return _resultStream;
    }
}