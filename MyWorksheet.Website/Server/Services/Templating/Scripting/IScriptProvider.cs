using System.Threading.Tasks;

namespace MyWorksheet.ReportService.Services.Scripting;

public interface IScriptProvider
{
    Task ExecuteScript(IScriptExecutionInfo script);
}