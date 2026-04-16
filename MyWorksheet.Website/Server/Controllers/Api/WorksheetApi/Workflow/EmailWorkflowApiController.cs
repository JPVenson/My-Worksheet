using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.WorksheetApi.Workflow;

[Route("api/Workflow/EmailWorkflow")]
public class EmailWorkflowApiControllerBase : ApiControllerBase
{
    private readonly WorksheetWorkflowManager _workflow;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public EmailWorkflowApiControllerBase(WorksheetWorkflowManager workflow, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _workflow = workflow;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IActionResult> SetConfirmendState(string worksheetGuid, bool rejected)
    {
        var baseUrl = "/EmailWorkflow?";
        using var db = _dbContextFactory.CreateDbContext();

        var wsh = db.WorksheetStatusHistories
            .FirstOrDefault(f => f.SystemComment == worksheetGuid);
        if (wsh == null)
        {
            return Redirect(baseUrl + "invalid=true");
        }

        var ws = db.Worksheets.Find(wsh.IdWorksheet);
        if (ws == null)
        {
            return Redirect(baseUrl + "invalid=true");
        }

        var proj = db.Projects.Find(ws.IdProject);
        if (proj == null)
        {
            return Redirect(baseUrl + "invalid=true");
        }

        var confState = await _workflow.SetWorksheetWorkflowStep(db, ws,
            rejected ? WorksheetStatusType.Rejected.ConvertToGuid() : WorksheetStatusType.Confirmed.ConvertToGuid(),
            proj.IdCreator, "", new Dictionary<string, object>());

        if (confState)
        {
            return Redirect(baseUrl + "invalid=false");
        }
        return Redirect(baseUrl + "invalid=" + Uri.EscapeDataString(confState.Reason));
    }
}