namespace MyWorksheet.ReportService.Services.Scripting;

public interface IScriptService
{
    IScriptProvider GetFor(string reportingKey);
}