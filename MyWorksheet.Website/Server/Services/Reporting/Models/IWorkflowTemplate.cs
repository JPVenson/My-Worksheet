using System.Collections.Generic;
using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public interface IWorkflowTemplate : IReportingDataSource
{
    IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        WorkflowReportArguments arguments);
}