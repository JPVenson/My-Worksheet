using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.ServerManager;

public interface IReportCapability
{
    ProcessorCapability[] ReportCapabilities();
}
