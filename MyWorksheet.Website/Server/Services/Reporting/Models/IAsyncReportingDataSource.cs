using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public interface IAsyncReportingDataSource
{
    Task<IDictionary<string, object>> GetDataAsync(MyworksheetContext config, Guid userId, ReportingExecutionParameterValue[] arguments);
}