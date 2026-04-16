using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Maintainace;

public interface IMaintenanceService
{
    MyWorksheet.Website.Server.Models.Maintainace CurrentMode { get; set; }
}

[SingletonService(typeof(IMaintenanceService))]
public class MaintenanceService : IMaintenanceService
{
    public MyWorksheet.Website.Server.Models.Maintainace CurrentMode { get; set; }
}