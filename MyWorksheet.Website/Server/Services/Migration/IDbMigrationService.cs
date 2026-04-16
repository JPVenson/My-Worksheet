using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Services.Migration;

public interface IDbMigrationService
{
    Task ExecuteDataMigration();
    Task ReportDataMigration(IAppLogger tempLogger);
}