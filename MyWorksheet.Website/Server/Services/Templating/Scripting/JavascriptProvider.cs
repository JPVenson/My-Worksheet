using System;
using System.Threading.Tasks;
using ChakraCore.NET;
using ChakraCore.NET.API;
using MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;
using MyWorksheet.Shared.Helper;
using ServiceLocator.Attributes;

namespace MyWorksheet.ReportService.Services.Scripting;

[SingletonService(typeof(IScriptProvider))]
public class JavascriptProvider : IScriptProvider
{
    public JavascriptProvider()
    {

    }

    public async Task ExecuteScript(IScriptExecutionInfo script)
    {
        await Task.CompletedTask;
        var options = JavaScriptRuntimeAttributes.AllowScriptInterrupt |
                      JavaScriptRuntimeAttributes.DisableBackgroundWork | JavaScriptRuntimeAttributes.DisableEval |
                      JavaScriptRuntimeAttributes.DisableNativeCodeGeneration;
        var done = false;
        try
        {
            using (var runtime = ChakraRuntime.Create(options))
            {
                using (var context = runtime.CreateContext(false))
                {
                    context.Runtime.ServiceNode.PopService<IJSValueConverterService>();
                    context.Runtime.ServiceNode.PushService<IJSValueConverterService>(new AnyValueConverter());

                    foreach (var argument in script.EnvironmentData)
                    {
                        context.GlobalObject.WriteProperty(argument.Key, argument.Value);
                    }
                    Task.Delay(script.Timeout).ContinueWith(e =>
                    {
                        if (!done)
                        {
                            context.Runtime?.TerminateRuningScript();
                        }
                    }).AttachAsyncHandler();
                    try
                    {
                        context.RunScript(script.Code);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            done = true;
        }
    }
}