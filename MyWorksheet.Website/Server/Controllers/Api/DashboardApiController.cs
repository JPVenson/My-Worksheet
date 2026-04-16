using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using MyWorksheet.Helper.Db;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/DashboardApi")]
[RevokableAuthorize]
public class DashboardApiControllerBase : ApiControllerBase
{
    public DashboardApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper,
        WorksheetWorkflowManager worksheetWorkflowManager)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _worksheetWorkflowManager = worksheetWorkflowManager;
    }

    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;
    private readonly WorksheetWorkflowManager _worksheetWorkflowManager;

    [HttpGet]
    [Route("DashboardPluginInfos")]
    public IActionResult GetDashboardPositions()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<DashboardPluginInfoViewModel[]>(db.DashboardPlugins.Where(f => f.IdAppUser == User.GetUserId()).ToArray()));
    }

    [HttpPost]
    [Route("UpdateDashboardPluginInfos")]
    public async Task<IActionResult> UpdateDashboardPositions([FromBody] DashboardPluginInfoViewModel[] dashboardPlugins)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var pluginInfos = db.DashboardPlugins
            .Where(f => f.IdAppUser == User.GetUserId())
            .ToArray();

        var newPluginInfos = _mapper.ViewModelMapper.Map<DashboardPlugin[]>(dashboardPlugins)
            .GroupBy(e => e.ArgumentsQuery)
            .Select(e => e.First())
            .ToArray();

        foreach (var dashboardPlugin in newPluginInfos)
        {
            dashboardPlugin.IdAppUser = User.GetUserId();
        }
        db.DoCreateDeleteOrUpdate(pluginInfos, newPluginInfos, f => f.ArgumentsQuery);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return Data();
    }

    [HttpGet]
    [Route("GetInvoicesFor")]
    public IActionResult GetInvoicesFor(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var projectsInOrganisations = ProjectsInOrganisation.Query(db, User.GetUserId())
            .Where(e => (e.IdOrganisation == null && e.IdCreator == User.GetUserId()) || (e.IdOrganisation != null && e.IdAppUser == User.GetUserId()))
            .Where(e =>
                e.Relations.Contains(UserToOrgRoles.ProjectManager.Id)).ToArray();

        if (!projectsInOrganisations.Any())
        {
            return Data(new object[0]);
        }

        var result = new List<InvoiceViewModel>();

        foreach (var projectsInOrganisation in projectsInOrganisations)
        {
            var getProjectModel = _mapper.ViewModelMapper.Map<GetProjectModel>(projectsInOrganisation);
            foreach (var workflow in _worksheetWorkflowManager.WorksheetWorkflows)
            {
                var targetSteps = workflow.Value.AwaitingPaymentStep
                                    .Select(f => f.ConvertToGuid())
                                    .Concat(workflow.Value.Closed.Select(f => f.ConvertToGuid()))
                                    .ToArray();
                var worksheetsInWorkflow = db.Worksheets
                    .Where(f => f.IdWorksheetWorkflow == workflow.Key)
                    .Where(f => f.IdProject == projectsInOrganisation.ProjectId)
                    .Where(e => targetSteps.Contains(e.IdCurrentStatus ?? Guid.Empty))
                    .ToArray();

                foreach (var worksheet in worksheetsInWorkflow)
                {
                    var lastHistoryEntry = db.WorksheetStatusHistories
                        .Where(f => f.IdWorksheet == worksheet.WorksheetId)
                        .OrderBy(e => e.DateOfAction).First();

                    var wsItems = db.WorksheetItems
                        .Where(f => f.IdWorksheet == worksheet.WorksheetId)
                        .ToArray()
                        .Select(e => e.ToTime - e.FromTime)
                        .AggregateIf(e => e.Any(), (e, f) => e + f);

                    result.Add(new InvoiceViewModel()
                    {
                        Project = getProjectModel,
                        Worksheet = _mapper.ViewModelMapper.Map<WorksheetModel>(worksheet),
                        DateOfSubmit = lastHistoryEntry.DateOfAction,
                        TimesWorked = wsItems
                    });
                }
            }
        }

        return Data(result);
    }

    [HttpGet]
    [Route("GetWorksheetsInWorkflowWithStatus")]
    public IActionResult GetWorksheetsInWorkflowWithStatus(Guid workflowStatusId, Guid? workflowId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheetsWithWorkflow = _mapper.ViewModelMapper.Map<WorksheetInWorkflowViewModel[]>(db.Worksheets
            .Where(f => f.IdWorksheetWorkflow == workflowId)
            .Where(f => f.IdCurrentStatus == workflowStatusId)
            .ToArray());

        foreach (var worksheetInWorkflowViewModel in worksheetsWithWorkflow)
        {
            var wsItems = db.WorksheetItems
                .Where(f => f.IdWorksheet == worksheetInWorkflowViewModel.Worksheet.WorksheetId)
                .ToArray();
            if (wsItems.Any())
            {
                worksheetInWorkflowViewModel.WorkedTimes =
                    wsItems.Select(e => e.ToTime - e.FromTime).Aggregate((e, f) => e + f);
            }
        }

        return Data(worksheetsWithWorkflow);
    }

    [HttpGet]
    [Route("ActiveWorksheets")]
    public IActionResult GetCurrentWorksheetDays()
    {
        using var db = _dbContextFactory.CreateDbContext();

        /*
        CREATE VIEW ""DashboardWorksheets"" AS
SELECT
    p.""Name"" AS ""ProjectName"",
    w.*,
    COALESCE(
        (SELECT FALSE FROM ""WorksheetItem"" wi
         WHERE wi.""Id_Worksheet"" = w.""Worksheet_Id""
           AND wi.""DateOfAction""::date = CURRENT_DATE
         LIMIT 1),
        TRUE
    ) AS ""HasDaysOpen""
FROM ""Worksheet"" w
    JOIN ""Project"" p ON p.""Project_Id"" = w.""Id_Project""
WHERE w.""Hidden"" = FALSE
  AND worksheet_is_submitted(w.""Worksheet_Id"") = FALSE;
        
        */
        var query = db.Worksheets
            .Include(e => e.IdProjectNavigation)
            .Where(f => f.IdCreator == User.GetUserId())
            .Where(e => e.IdProjectNavigation.Hidden == false 
                && e.IdCurrentStatus != Guid.Empty 
                && e.IdCurrentStatusNavigation.AllowModifications) //TODO e.IdCurrentStatus > 2
            .Where(e => e.EndTime > DateTime.UtcNow.AddDays(-1) && e.StartTime < DateTime.UtcNow)
            .Select(e => new
            {
                Worksheet = e,
                Name = e.IdProjectNavigation.Name,
                HasDaysOpen = e.WorksheetItems.Any(f => f.DateOfAction.Date == DateTime.Today)
            });
        return Data(_mapper.ViewModelMapper.Map<DashboardWorksheetModel[]>(query.ToArray()));
    }

    [HttpGet]
    [Route("DefaultReports")]
    public IActionResult GetDefaultReportings()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var worksheets = db.NengineTemplates
            .Where(f => f.IdCreator == null);
        return Data(_mapper.ViewModelMapper.Map<NEngineTemplateLookup[]>(worksheets));
    }
}