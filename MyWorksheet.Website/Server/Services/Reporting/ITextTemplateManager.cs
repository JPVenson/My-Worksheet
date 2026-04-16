using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using MyWorksheet.Website.Server.Services.ServerManager;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

namespace MyWorksheet.Website.Server.Services.Reporting;

public interface ITextTemplateManager : IReportCapability
{
    IDictionary<string, ITemplateEngine> KnownTextFormatter { get; }
    event OnReportPositionChanged ReportPositionChanged;
    int GetPositionOfTemplate(Guid templateId, Guid creatorId);

    Task<StandardOperationResultBase<Guid>> RunReport(MyworksheetContext db,
        ScheduleReportModel model, Guid appUserId);

    StandardOperationResultBase<NengineTemplate> PrepareReport(MyworksheetContext db,
        ScheduleReportModel model,
        Guid appUserId,
        IDictionary<string, object> dataSourceElements);

    Task<StandardOperationResultBase<Guid>> GenerateTemplate(Guid templateId,
        string template,
        TextTemplateDataQuery values,
        PriorityManagerLevel level,
        Guid creatorId,
        Guid storageProviderId,
        bool preview,
        IDictionary<string, object> additonalReportInfos = null);

    void OnTemplateProcessed(Guid reportArgumentsTemplateId);
}