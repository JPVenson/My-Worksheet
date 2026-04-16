using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Models.Configurations;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.ManualFlow;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;

[SingletonService()]
public class WorksheetWorkflowManager
{
    private readonly ObjectChangedService _objectChangedService;

    public WorksheetWorkflowManager(IDbContextFactory<MyworksheetContext> dbContextFactory,
        ActivatorService activatorService,
        ObjectChangedService objectChangedService)
    {
        _objectChangedService = objectChangedService;
        WorksheetWorkflows = new Dictionary<Guid, IWorksheetWorkflow>()
        {
            { WorksheetWorkflowConfiguration.ManualWorkflow, activatorService.ActivateType<ManualWorksheetWorkflow>() },
            { WorksheetWorkflowConfiguration.EmailWorkflow, activatorService.ActivateType<EmailWorkflow>() },
        };

        using var db = dbContextFactory.CreateDbContext();
        var workflowSteps = db.WorksheetStatusLookups.ToArray();
        var workflowMaps = db.WorksheetStatusLookupMaps;

        foreach (var statusMap in workflowMaps.GroupBy(e => e.IdWorkflow).ToArray())
        {
            var workflow = WorksheetWorkflows[statusMap.Key];
            foreach (var worksheetStatusLookup in statusMap.GroupBy(e => e.IdFromStatus))
            {
                var fromStatus = workflowSteps.First(e => e.WorksheetStatusLookupId == worksheetStatusLookup.Key);
                var toStatus = workflowSteps.Where(e =>
                        worksheetStatusLookup.Any(f => f.IdToStatus == e.WorksheetStatusLookupId))
                    .ToArray();

                workflow.AllowedTransitions.Add(
                    new GenericWorksheetStatusType(fromStatus),
                    toStatus
                        .Select(e => new GenericWorksheetStatusType(e))
                        .Cast<IWorksheetStatusType>()
                        .ToArray());
            }

            foreach (var rightStatus in workflow.AllowedTransitions.SelectMany(f => f.Value))
            {
                workflow.WorksheetWorkflowSteps[rightStatus.ConvertToGuid()] = new WorksheetWorkflowGenericStep(rightStatus);
            }

            var workflowEntity = db.WorksheetWorkflows
                .Where(f => f.ProviderKey == workflow.ProviderKey)
                .First();

            workflow.NeedsCData = workflowEntity.NeedsCustomData.GetValueOrDefault();
            workflow.DefaultStep = workflow.AllowedTransitions.SelectMany(e => e.Value)
                .FirstOrDefault(e => e.ConvertToGuid() == workflowEntity.IdDefaultStep);
            // workflow.NoModifications = workflow.AllowedTransitions.SelectMany(e => e.Value)
            // 	.FirstOrDefault(e => e.ConvertToGuid() == workflowEntity.IdNoModificationsStep);

            workflow.InitDone();
        }
    }

    public IDictionary<Guid, IWorksheetWorkflow> WorksheetWorkflows { get; }

    public IWorksheetStatusType GetStepFromId(Guid workflowId, Guid? workflowStep)
    {
        if (!WorksheetWorkflows.TryGetValue(workflowId, out var workflow))
        {
            return null;
        }
        return workflow.AllowedTransitions.SelectMany(e => e.Value).FirstOrDefault(e => e.ConvertToGuid().Equals(workflowStep));
    }

    public Task<IObjectSchema> GetPossibleWorkflowStepInfo(MyworksheetContext db, Guid? workflow, Guid nextStep, Guid userId,
        Guid worksheetId)
    {
        if (!workflow.HasValue)
        {
            return Task.FromResult<IObjectSchema>(JsonSchema.EmptyNotNull);
        }

        if (!WorksheetWorkflows.ContainsKey(workflow.Value))
        {
            return Task.FromResult<IObjectSchema>(JsonSchema.EmptyNotNull);
        }
        var wf = WorksheetWorkflows[workflow.Value];

        if (!wf.WorksheetWorkflowSteps.ContainsKey(nextStep))
        {
            return Task.FromResult<IObjectSchema>(JsonSchema.EmptyNotNull);
        }

        return wf.WorksheetWorkflowSteps[nextStep].GetSchema(db, userId, worksheetId);
    }

    public IDictionary<IWorksheetStatusType, IWorksheetStatusType[]> GetWorkflowTransitions(MyworksheetContext db, Guid workflow)
    {
        return WorksheetWorkflows[workflow].AllowedTransitions;
    }


    public IWorksheetStatusType[] GetPossibleWorkflowStep(MyworksheetContext db, Guid? workflow, Guid? currentStep)
    {
        if (!workflow.HasValue)
        {
            return new IWorksheetStatusType[0];
        }

        if (!WorksheetWorkflows.ContainsKey(workflow.Value))
        {
            return new IWorksheetStatusType[0];
        }

        var wf = WorksheetWorkflows[workflow.Value];

        if (currentStep == null)
        {
            return new[] { wf.DefaultStep };
        }

        var transition = wf.AllowedTransitions.FirstOrDefault(e => e.Key.ConvertToGuid() == currentStep);

        if (transition.Value == null)
        {
            return new IWorksheetStatusType[0];
        }

        return transition.Value;
    }

    public IWorksheetStatusType GetDefaultStepFor(Guid workflowId)
    {
        IWorksheetWorkflow workflow;
        if (!WorksheetWorkflows.TryGetValue(workflowId, out workflow))
        {
            return null;
        }

        return workflow.DefaultStep;
    }

    public async Task<QuestionableBoolean> SetWorksheetWorkflowStep(MyworksheetContext db,
        Worksheet worksheet,
        Guid nextStatusId,
        Guid userId,
        string reason,
        IDictionary<string, object> additonalData,
        Guid? workflowDataId = null,
        bool setOnlyStatus = false)
    {
        if (!worksheet.IdWorksheetWorkflow.HasValue)
        {
            return false.Because("No Workflow was set");
        }

        return await SetWorksheetWorkflowStep(db, worksheet, GetStepFromId(worksheet.IdWorksheetWorkflow.Value, nextStatusId), userId,
            reason, additonalData, workflowDataId, setOnlyStatus);
    }

    public async Task<QuestionableBoolean> SetWorksheetWorkflowStep(MyworksheetContext db,
        Worksheet worksheet,
        IWorksheetStatusType nextStatus,
        Guid userId,
        string reason,
        IDictionary<string, object> additonalData,
        Guid? workflowDataId = null,
        bool setOnlyStatus = false)
    {
        IWorksheetWorkflow workflow;
        if (!worksheet.IdWorksheetWorkflow.HasValue)
        {
            workflow = WorksheetWorkflows.FirstOrDefault().Value;
            nextStatus = nextStatus ?? workflow.DefaultStep;
        }
        else
        {
            if (!WorksheetWorkflows.TryGetValue(worksheet.IdWorksheetWorkflow.Value, out workflow))
            {
                return false.Because("The selected Workflow is Not available");
            }
        }

        var nextStatusId = nextStatus.ConvertToGuid();

        QuestionableBoolean questionableBoolean = true;

        IWorksheetWorkflowStep nextStep = null;
        if (!setOnlyStatus)
        {
            var worksheetIdCurrentStatus = worksheet.IdCurrentStatus;
            if (worksheetIdCurrentStatus is not null && !worksheetIdCurrentStatus.Equals(Guid.Empty))
            {
                var currentStep = workflow.AllowedTransitions.FirstOrDefault(e => e.Key.ConvertToGuid().Equals(worksheetIdCurrentStatus)).Value;

                if (currentStep == null || currentStep.All(e => e.ConvertToGuid() != nextStatus.ConvertToGuid()))
                {
                    return false.Because("Workflow Status transition is not allowed");
                }
            }
            else
            {
                if (!workflow.WorksheetWorkflowSteps.ContainsKey(nextStatusId))
                {
                    return false.Because("Workflow Status does not exist");
                }
            }

            nextStep = workflow.WorksheetWorkflowSteps[nextStatusId];

            if (workflowDataId.HasValue)
            {
                var dataOfWorkflow = db.WorksheetWorkflowData
                    .Where(f => f.IdWorksheetWorkflowMap == workflowDataId.Value)
                    .ToDictionary(e => e.Key, e => e.Value);

                var data = JsonSchemaExtensions.MapToSchema(dataOfWorkflow, workflow.GetSchema(db, userId),
                        JsonSchema.Delimiter)
                    .Expand();

                additonalData = additonalData.Concat(data).ToDictionary(e => e.Key, e => e.Value);
            }
            questionableBoolean = await nextStep.OnChange(db, worksheet, worksheet.IdCurrentStatus, additonalData);
        }

        if (!questionableBoolean)
        {
            return questionableBoolean;
        }

        var worksheetStatusHistory = new WorksheetStatusHistory
        {
            IdWorksheet = worksheet.WorksheetId,
            DateOfAction = DateTime.UtcNow,
            IdChangeUser = userId,
            IdPostState = nextStatus.ConvertToGuid(),
            IdPreState = null,
            Reason = reason,
        };

        db.Add(worksheetStatusHistory);
        worksheet.IdCurrentStatus = nextStatus.ConvertToGuid();

        if (!setOnlyStatus && nextStep != null)
        {
            questionableBoolean = await nextStep.AfterChange(db,
                worksheet,
                worksheet.IdCurrentStatus,
                worksheetStatusHistory.WorksheetStatusHistoryId,
                additonalData);

            if (questionableBoolean == false)
            {
                return questionableBoolean;
            }
        }

        await db.SaveChangesAsync().ConfigureAwait(false);

        await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, typeof(Worksheet), worksheet.WorksheetId, null, userId);
        return questionableBoolean;
    }

    public bool GetCanModify(Guid worksheetWorkflowId, Guid? worksheetIdCurrentStatus)
    {
        IWorksheetWorkflow workflow;
        if (!WorksheetWorkflows.TryGetValue(worksheetWorkflowId, out workflow))
        {
            return false.Because("The selected Workflow is Not avaiable");
        }

        return workflow.CanModify(GetStepFromId(worksheetWorkflowId, worksheetIdCurrentStatus));
    }
}
