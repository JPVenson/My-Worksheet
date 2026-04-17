using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyWorksheet.Website.Server.Services.Migration;

public interface IDbMigrationService
{
    Task ExecuteDataMigration();
    Task ReportDataMigration(ILogger tempLogger);
}