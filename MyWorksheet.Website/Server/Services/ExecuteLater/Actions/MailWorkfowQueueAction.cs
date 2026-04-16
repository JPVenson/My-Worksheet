using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Mail;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.EntityFrameworkCore;
using MyWorksheet.Website.Server.Services.ExecuteLater.Actions;
using Newtonsoft.Json;

namespace MyWorksheet.ReportService.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class MailWorkfowQueueAction : IPriorityQueueAction
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IActivityService _activityService;
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;
    private readonly IBlobManagerService _blobManagerService;
    private readonly IMailServiceProvider _mailServiceProvider;
    private readonly IAppLogger _appLogger;

    public MailWorkfowQueueAction(
    IDbContextFactory<MyworksheetContext> dbContextFactory,
    IActivityService activityService,
    WorksheetWorkflowManager worksheetWorkflowManager,
    IBlobManagerService blobManagerService,
    IMailServiceProvider mailServiceProvider,
    IAppLogger appLogger)
    {
        _dbContextFactory = dbContextFactory;
        _activityService = activityService;
        _worksheetWorkflowManager = worksheetWorkflowManager;
        _blobManagerService = blobManagerService;
        _mailServiceProvider = mailServiceProvider;
        _appLogger = appLogger;
    }

    public const string ActionKey = ExternalSchedulableTasks.SEND_MAIL_FOR_WORKFLOW;

    public string Name => "Followup action for the mail report creation job";
    public string Key => ActionKey;
    public Version Version { get; set; }

    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return new DictionaryElementsValidator<string, object>(arguments)
        .OfType<Guid>("preId")
        .OfType<Guid>("NEngineRunningTaskId")
        .Result;
    }

    public class ReportArguments : ArgumentsBase
    {
        public static ReportArguments Parse(IDictionary<string, object> queueElementArguments)
        {
            var ra = new ReportArguments();
            ra.ReportCreationArguments = GenerateReport.ReportArguments.Parse(queueElementArguments);
            ra.FlowArguments = WorkflowReportArguments.Parse(queueElementArguments);
            ra.SetOrAbort<Guid>(queueElementArguments, e => ra.FromStatusId = e, "preId");
            return ra.GetIfValid() as ReportArguments;
        }

        public GenerateReport.ReportArguments ReportCreationArguments { get; set; }
        public WorkflowReportArguments FlowArguments { get; set; }
        public Guid FromStatusId { get; set; }
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        await using var db = _dbContextFactory.CreateDbContext();
        var arguments = ReportArguments.Parse(queueElement.Arguments);

        var runResult = db.NengineRunningTasks.Find(arguments.ReportCreationArguments.NEngineRunningTaskId);
        var worksheet = runResult != null ? db.Worksheets.Find(arguments.FlowArguments.Worksheet) : null;
        var project = worksheet != null ? db.Projects.Find(worksheet.IdProject) : null;

        try
        {
            if (runResult == null || worksheet == null || project == null)
            {
                _appLogger.LogError("MailWorkfowQueueAction: could not load required entities", null,
                new Dictionary<string, string>
                {
{ "NEngineRunningTaskId", arguments.ReportCreationArguments.NEngineRunningTaskId.ToString() },
{ "WorksheetId", arguments.FlowArguments.Worksheet.ToString() }
                });
                return;
            }

            if (runResult.IsFaulted)
            {
                await _activityService.CreateActivity(
                ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, runResult.ErrorText));
                await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
                "SYSTEM: Could not send mail. Revert status. Mail creation failed.", null, null, true);
                return;
            }

            var stream = await _blobManagerService.GetData(runResult.IdStoreageEntry.Value, runResult.IdCreator);
            if (!stream.Success)
            {
                await _activityService.CreateActivity(
                ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, stream.Error));
                await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
                "SYSTEM: Could not send mail. Revert status. Receiving the data from the used storage provider failed.", null, null, true);
                return;
            }

            using (stream.Object)
            {
                var creator = db.AppUsers.Find(worksheet.IdCreator);
                Organisation organisation = null;
                if (project.IdOrganisation.HasValue)
                {
                    organisation = db.Organisations.Find(project.IdOrganisation.Value);
                }

                if (organisation == null || organisation.IsDeleted)
                {
                    await _activityService.CreateActivity(
                    ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, "Organisation was deleted or not set"));
                    await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
                    "SYSTEM: Could not send mail. Revert status. Error while resolving mail address.", null, null, true);
                    return;
                }

                var receiverAddress = db.Addresses.Find(organisation.IdAddress);
                if (creator == null || receiverAddress == null)
                {
                    await _activityService.CreateActivity(
                    ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, "Creator or receiver address not found"));
                    await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
                    "SYSTEM: Could not send mail. Revert status. Creator or address missing.", null, null, true);
                    return;
                }

                var questionableBoolean = await _mailServiceProvider.ApplicationMailService.SendMail(
                new WorksheetStatusChangedMail(stream.Object.Stringify(false, Encoding.UTF8), creator.Username, project.Name, worksheet.WorksheetId.ToString()),
                queueElement.UserId,
                receiverAddress.EmailAddress);

                if (!questionableBoolean)
                {
                    await _activityService.CreateActivity(
                    ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, questionableBoolean.Reason));
                    await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
                    "SYSTEM: Could not send mail. Revert status. Error while sending the mail.", null, null, true);
                }
            }
        }
        catch (Exception e)
        {
            await _activityService.CreateActivity(
            ActivityTypes.MailWorkflowFailed.CreateActivity(db, worksheet, project, queueElement.UserId, "Unknown Error. We are Informed and will inspect that!"));
            await _worksheetWorkflowManager.SetWorksheetWorkflowStep(db, worksheet, arguments.FromStatusId, queueElement.UserId,
            "SYSTEM: Could not send mail. Revert status.", null, null, true);
            _appLogger.LogError("Error due Email Workflow", null, new Dictionary<string, string>()
{
{ "Exception", JsonConvert.SerializeObject(e) }
});
        }
    }
}
