using System.Collections.Generic;
using System;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public interface IReportingDataSource
{
    Guid Id { get; set; }
    string Key { get; set; }
    string Name { get; set; }
    ReportPurpose[] Purpose { get; set; }

    IObjectSchema QuerySchema();
    IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId);

    IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments);
    ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId);
}