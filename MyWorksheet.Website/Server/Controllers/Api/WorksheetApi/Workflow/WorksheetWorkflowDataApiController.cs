using System.Collections.Generic;
using System;
using System.Linq;
using MyWorksheet.Helper.Db;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi.Workflow;

[Authorize]
[Route("api/WorksheetWorkflowData")]
public class WorksheetWorkflowDataApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;

    public WorksheetWorkflowDataApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper, WorksheetWorkflowManager worksheetWorkflowManager)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _worksheetWorkflowManager = worksheetWorkflowManager;
    }

    [HttpGet]
    [Route("GetGroups")]
    public IActionResult GetWorkflowGroups(Guid workflowId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflow = db.WorksheetWorkflows.Find(workflowId);
        if (workflow == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var administratorInOrgs = db.OrganisationUserMaps.Where(e => e.IdAppUser == User.GetUserId() && e.IdRelation == UserToOrgRoles.Administrator.Id)
            .Select(e => e.IdOrganisation)
            .ToArray();

        var workflowData = db.WorksheetWorkflowDataMaps.Where(e => e.IdWorksheetWorkflow == workflow.WorksheetWorkflowId)
            .Where(e => (e.IdSharedWithOrganisation == null && e.IdCreator == User.GetUserId()) || administratorInOrgs.Contains(e.IdSharedWithOrganisation.Value))
            .ToArray();

        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowDataMapViewModel[]>(workflowData));
    }

    [HttpGet]
    [Route("GetGroup")]
    public IActionResult GetWorkflowGroup(Guid workflowDataGroupId)
    {
        var workflowData = GetWorkflowGroupsForUser(workflowDataGroupId).FirstOrDefault();

        if (workflowData == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowDataMapViewModel>(workflowData));
    }

    [NonAction]
    private WorksheetWorkflowDataMap[] GetWorkflowGroupsForUser(Guid workflowDataGroupId)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var administratorInOrgs = db.OrganisationUserMaps
            .Where(e => e.IdAppUser == User.GetUserId() && e.IdRelation == UserToOrgRoles.Administrator.Id)
            .Select(e => e.IdOrganisation).ToArray();
        var workflowData = db.WorksheetWorkflowDataMaps.Where(e => e.WorksheetWorkflowDataMapId == workflowDataGroupId)
            .Where(e => (e.IdSharedWithOrganisation == null && e.IdCreator == User.GetUserId()) || administratorInOrgs.Contains(e.IdSharedWithOrganisation.Value))
            ;
        return workflowData.ToArray();
    }

    [HttpGet]
    [Route("GetValues")]
    public IActionResult GetWorkflowGroupData(Guid groupId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflowDataMap = db.WorksheetWorkflowDataMaps.Find(groupId);

        if (workflowDataMap == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var workflowData = db.WorksheetWorkflowData.Where(e => e.IdWorksheetWorkflowMap == workflowDataMap.WorksheetWorkflowDataMapId).ToArray();

        var worksheetWorkflow = _worksheetWorkflowManager.WorksheetWorkflows[workflowDataMap.IdWorksheetWorkflow];

        var objectSchema = worksheetWorkflow.GetSchema(db, User.GetUserId());

        return Data(new WorkflowDataViewModel()
        {
            Values = JsonSchemaExtensions.MapToSchema(workflowData.ToDictionary(e => e.Key, e => e.Value), objectSchema, JsonSchema.Delimiter)
            //.Expand(),
        });
    }

    [HttpGet]
    [Route("GetStructure")]
    public IActionResult GetWorkflowStructure(Guid workflowId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflowDataMap = db.WorksheetWorkflows.Find(workflowId);

        if (workflowDataMap == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var workflowServiceWorksheetWorkflow = _worksheetWorkflowManager.WorksheetWorkflows[workflowId];
        var objectSchema = workflowServiceWorksheetWorkflow.GetSchema(db, User.GetUserId());

        return Data(new WorkflowDataViewModel()
        {
            ObjectSchema = objectSchema as JsonSchema,
        });
    }

    [HttpPost]
    [Route("Create")]
    public IActionResult CreateWorkflowData([FromBody] CreateWorksheetWorkflowDataMapViewModel groupData)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflow = db.WorksheetWorkflows.Find(groupData.IdWorksheetWorkflow);
        if (workflow == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var workflowInfoData = _mapper.ViewModelMapper.Map<WorksheetWorkflowDataMap>(groupData);
        workflowInfoData.IdCreator = User.GetUserId();
        using var transaction = db.Database.BeginTransaction();

        db.Add(workflowInfoData);
        groupData.Fields = groupData.Fields?.ToJsonElements() ?? new Dictionary<string, object>();
        var workflowData = groupData.Fields.Where(e => e.Value is not null).Select(e => new WorksheetWorkflowData()
        {
            Key = e.Key,
            Value = e.Value?.ToString(),            
            IdWorksheetWorkflowMapNavigation = workflowInfoData
        });

        foreach (var worksheetWorkflowData in workflowData)
        {
            db.Add(worksheetWorkflowData);
        }

        db.SaveChanges();
        transaction.Commit();

        return Data(_mapper.ViewModelMapper.Map<WorksheetWorkflowDataMapViewModel>(workflowInfoData));
    }

    [HttpPost]
    [Route("Update")]
    public IActionResult UpdateWorkflowData([FromBody] WorksheetWorkflowDataMapViewModel groupDataMap)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var workflow = db.WorksheetWorkflows.Find(groupDataMap.IdWorksheetWorkflow);
        if (workflow == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var workflowData = GetWorkflowGroupsForUser(groupDataMap.WorksheetWorkflowDataMapId).FirstOrDefault();

        if (workflowData == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var groupDataFields = groupDataMap.Fields.ToJsonElements();

        var worksheetWorkflowData = groupDataFields.Select(e => new WorksheetWorkflowData()
        {
            Key = e.Key,
            Value = e.Value?.ToString()
        }).Where(e => e.Value != null).ToArray();

        using var transaction = db.Database.BeginTransaction();
        {
            db.WorksheetWorkflowDataMaps.Where(e => e.WorksheetWorkflowDataMapId == groupDataMap.WorksheetWorkflowDataMapId)
                .ExecuteUpdate(e => e.SetProperty(f => f.GroupKey, groupDataMap.GroupKey));

            var existingWorkflowData = db.WorksheetWorkflowData.Where(e => e.IdWorksheetWorkflowMap == groupDataMap.WorksheetWorkflowDataMapId).ToArray(); ;
            foreach (var groupDataField in worksheetWorkflowData)
            {
                groupDataField.IdWorksheetWorkflowMap = groupDataMap.WorksheetWorkflowDataMapId;
            }

            db.DoCreateDeleteOrUpdate(existingWorkflowData, worksheetWorkflowData, f => f.Key);

            db.SaveChanges();
            transaction.Commit();
        }
        return Data();
    }
}