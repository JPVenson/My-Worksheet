using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.ReportService.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Webpage.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class AddReportFromWorkflow : IPriorityQueueAction
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IActivityService _activityService;

    public AddReportFromWorkflow(IDbContextFactory<MyworksheetContext> dbContextFactory, IActivityService activityService)
    {
        _dbContextFactory = dbContextFactory;
        _activityService = activityService;
    }

    public const string ActionKey = "ADD_CREATED_REPORT_FROM_WORKFLOW";
    public string Name => "Followup action from a workflow that adds the generated report to an workflow state";
    public string Key => ActionKey;
    public Version Version { get; set; }
    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return true;
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var reportArguments = GenerateReport.ReportArguments.Parse(queueElement.Arguments);

        var statusId = queueElement.Arguments["HistoryId"];

        var nEngineReport = db.NengineRunningTasks.Find(reportArguments.NEngineRunningTaskId);

        if (!nEngineReport.IsDone || nEngineReport.IsFaulted || !nEngineReport.IdStoreageEntry.HasValue)
        {
            var template = db.NengineTemplates.Find(nEngineReport.IdNengineTemplate);

            await _activityService
                .CreateActivity(ActivityTypes.ReportGenerationFailed.CreateActivity(db, template.Name, queueElement.UserId));
            return;
        }

        db.Add(new WorksheetWorkflowStorageMap()
        {
            IdStorageEntry = nEngineReport.IdStoreageEntry.Value,
            IdWorksheetStatusHistory = (Guid)statusId,
            IdAppUser = queueElement.UserId
        });
        db.SaveChanges();

        await Task.CompletedTask;
    }
}