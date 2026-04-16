using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs;

public interface ILoggerHub
{
    Task LogEntries(AppLoggerLogViewModel[] entries);
}