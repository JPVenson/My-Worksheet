using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.Reporting.TemplateProvider;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Server.Util.Extentions;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit;

namespace MyWorksheet.Website.Server.Controllers.Api.Reporting;

[Route("api/TemplateManagementApi")]
[RevokableAuthorize]
public class ReportingManagementApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly ILogger<ReportingManagementApiController> _logger;
    private readonly ITemplateProviderService _templateProviderService;
    private readonly IBlobManagerService _blobManagerService;
    private readonly ITempFileTokenService _tempFileTokenService;
    private readonly ObjectChangedService _objectChangedHub;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IMapperService _mapper;

    public ReportingManagementApiController(IMapperService mapper,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        ILogger<ReportingManagementApiController> logger,
        ITemplateProviderService templateProviderService,
        IBlobManagerService blobManagerService,
        ITempFileTokenService tempFileTokenService,
        ObjectChangedService objectChangedHub,
        IHostEnvironment hostEnvironment)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _templateProviderService = templateProviderService;
        _blobManagerService = blobManagerService;
        _tempFileTokenService = tempFileTokenService;
        _objectChangedHub = objectChangedHub;
        _hostEnvironment = hostEnvironment;
    }

    [HttpGet]
    [Route("GetPurposes")]
    public IActionResult GetPurposes()
    {
        return Data(ReportPurposes.Yield());
    }

    [HttpGet]
    [Route("Get")]
    public IActionResult GetTemplates(string purpose = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.NengineTemplates.Where(e => e.IdCreator == null || e.IdCreator == User.GetUserId());

        if (purpose != null)
        {
            var sourcesWithType = ReportDataSources.Yield().Where(e =>
            {
                try
                {
                    return e.Purpose.Select(f => f.Key).Contains(purpose);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(e + " HAS NO PURPOSE!");
                }

                return false;
            });
            query = query.Where(f => sourcesWithType.Select(f => f.Key).Contains(f.UsedDataSource));
        }
        return Data(_mapper.ViewModelMapper.Map<NEngineTemplateLookup[]>(query.ToArray()));
    }

    [HttpGet]
    [Route("GetContent")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var template = await _templateProviderService.FindTemplate(db, id);
        using (template.Item2)
        {
            return Data(new StreamReader(template.Item2).ReadToEnd());
        }
    }

    [HttpGet]
    [Route("Search")]
    public IActionResult GetTemplates(int page, int pageSize, string search = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.NengineTemplates.Where(e => e.IdCreator == null || e.IdCreator == User.GetUserId());

        if (!String.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search));
        }


        return Data(_mapper.ViewModelMapper.Map<PageResultSet<NEngineTemplateLookup>>(query.OrderBy(e => e.IdCreator).ForPagedResult(page, pageSize)));
    }

    [HttpGet]
    [Route("GetSingle")]
    public IActionResult GetTemplate(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();

        var template = db.NengineTemplates
            .Where(e => e.IdCreator == null || e.IdCreator == User.GetUserId())
            .Where(e => e.NengineTemplateId == id)
            .FirstOrDefault();

        return Data(_mapper.ViewModelMapper.Map<NEngineTemplateLookup>(template));
    }

    [HttpGet]
    [Route("History")]
    public IActionResult GetHistory(Guid templateId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var template = db.NengineTemplates.Find(templateId);
        if (template.IdCreator.HasValue && template.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var history = _mapper.ViewModelMapper.Map<NEngineHistoryGet[]>(db
        .NengineRunningTasks.Where(e => e.IdNengineTemplate == templateId && e.IsPreview == false)
            .ToArray());
        return Data(history);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateTemplate([FromBody] NEngineTemplateCreate model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var nEngineTemplate = _mapper.ViewModelMapper.Map<NengineTemplate>(model);
        nEngineTemplate.IdCreator = User.GetUserId();
        db.Add(nEngineTemplate);
        db.SaveChanges();
        await _objectChangedHub.SendObjectChanged(ChangeEventTypes.Removed, nEngineTemplate, Request.GetSignalId(), nEngineTemplate.IdCreator ?? User.GetUserId());
        return Data(_mapper.ViewModelMapper.Map<NEngineTemplateLookup>(nEngineTemplate));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var findTemplate = db.NengineTemplates.Find(id);
        if (findTemplate == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (findTemplate.IdCreator.HasValue)
        {
            if (findTemplate.IdCreator.Value != User.GetUserId())
            {
                return Unauthorized("Common/InvalidId".AsTranslation());
            }
        }
        else
        {
            if (!User.IsInRole(Roles.AdminRoleName))
            {
                return Unauthorized("Common/InvalidId".AsTranslation());
            }
        }

        var tasks = db.NengineRunningTasks.Where(e => e.IdNengineTemplate == id).ToList();

        foreach (var runningTask in tasks)
        {
            if (runningTask.IdStoreageEntry != null)
            {
                await _blobManagerService.DeleteData(runningTask.IdStoreageEntry.Value, User.GetUserId());
            }
        }

        db.NengineRunningTasks.RemoveRange(tasks);
        db.NengineTemplates.Remove(findTemplate);
        db.SaveChanges();
        await _objectChangedHub.SendObjectChanged(ChangeEventTypes.Removed, findTemplate, Request.GetSignalId(), findTemplate.IdCreator ?? User.GetUserId());
        return Data();
    }

    [HttpPost]
    [Route("Update")]
    public async Task<IActionResult> UpdateTemplate([FromBody] NEngineTemplateUpdate model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var findTemplate = db.NengineTemplates.Find(model.NEngineTemplateId);
        if (findTemplate == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!User.IsInRole(Roles.AdminRoleName))
        {
            if (findTemplate.IdCreator.HasValue)
            {
                if (findTemplate.IdCreator.Value != User.GetUserId())
                {
                    return Unauthorized("Common/InvalidId".AsTranslation());
                }
            }

            if (model.IdCreator.HasValue)
            {
                if (model.IdCreator.Value != User.GetUserId())
                {
                    return Unauthorized("Common/InvalidId".AsTranslation());
                }
            }
        }

        _mapper.ViewModelMapper.Map(model, findTemplate);
        db.Update(findTemplate);
        db.SaveChanges();
        await _objectChangedHub.SendObjectChanged(ChangeEventTypes.Changed, findTemplate, Request.GetSignalId(), model.IdCreator ?? User.GetUserId());
        //AccessElement<ReportHubInfo>.Instance.TriggerReportChanged(model.NEngineTemplateId);

        db.NengineRunningTasks.Where(e => e.IdNengineTemplate == model.NEngineTemplateId)
        .ExecuteUpdate(e => e.SetProperty(f => f.IsObsolete, true));

        return Data(_mapper.ViewModelMapper.Map<NEngineTemplateLookup>(findTemplate));
    }

    [HttpPost]
    [Route("ReduceHistory")]
    public async Task<IActionResult> ReduceHistory([FromBody] NEngineHistoryDelete historyToDelete)
    {
        if (historyToDelete == null)
        {
            throw new ArgumentNullException(nameof(historyToDelete));
        }

        using var db = _dbContextFactory.CreateDbContext();
        var countToDeleteQ = db.NengineRunningTasks.Where(e => historyToDelete.Ids.Contains(e.NengineRunningTaskId) && e.IdCreator == User.GetUserId()).ToList();
        var countToDelete = countToDeleteQ.FirstOrDefault();

        foreach (var item in countToDeleteQ)
        {
            if (item.IdStoreageEntry != null)
            {
                await _blobManagerService.DeleteData(item.IdStoreageEntry.Value, User.GetUserId());
            }
            db.NengineRunningTasks.Remove(item);
        }

        db.SaveChanges();

        return Data();
    }

    [HttpGet]
    [Route("IssueReportDownloadToken")]
    public IActionResult IssueReportToken(Guid reportId, Guid? historyId = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        NengineRunningTask target;
        if (!historyId.HasValue)
        {
            target = db.NengineRunningTasks.FirstOrDefault(e => e.IdNengineTemplate == reportId && e.IsPreview && e.IdCreator == User.GetUserId());
        }
        else
        {
            target = db.NengineRunningTasks.FirstOrDefault(e => historyId.Value == e.NengineRunningTaskId && e.IdCreator == User.GetUserId());
        }

        if (target == null || target.IdCreator != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (target.IdStoreageEntry != null)
        {
            return Data(_tempFileTokenService.IssueFileToken(User.GetUserId(), target.IdStoreageEntry.Value,
                Request.HttpContext.Connection.RemoteIpAddress.ToString()));
        }

        return Data();
    }

    [HttpGet]
    [Route("DownloadPreviewTemplate")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadPreviewTemplate(Guid reportId, string token, bool displayInline = false)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var template = db.NengineTemplates.Find(reportId);

        var run = db.NengineRunningTasks.FirstOrDefault(e => e.IdNengineTemplate == reportId && e.IsPreview == true);
        var httpResponseMessage = await PrepareDownload(db, displayInline, run, template, token);
        return httpResponseMessage;
    }

    [NonAction]
    private async Task<IActionResult> PrepareDownload(MyworksheetContext db, bool displayInline, NengineRunningTask run, NengineTemplate template, string token)
    {
        if (run == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (!run.IsDone)
        {
            return BadRequest("Reporting/TemplateStillProcessing".AsTranslation());
        }

        if (run.IsFaulted)
        {
            //if (_hostEnvironment.IsDevelopment())
            //{
            //	return Data(JsonConvert.SerializeObject(_mapper.ViewModelMapper.Map<NEngineHistoryGet>(run),
            //		Formatting.Indented));
            //}

            return Data(run.ErrorText);

        }

        if (!run.IdStoreageEntry.HasValue)
        {
            return BadRequest("Reporting/TemplateDeleted".AsTranslation());
        }


        var tokenInfo = _tempFileTokenService
            .RedeemToken(token, run.IdStoreageEntry.Value, Request.HttpContext.Connection.RemoteIpAddress.ToString(), true);
        if (tokenInfo == null)
        {
            return Redirect("/error");
        }

        var stream = await _blobManagerService.GetData(run.IdStoreageEntry.Value, run.IdCreator);

        if (!stream.Success)
        {
            return BadRequest("Reporting/TemplateContentDeleted".AsTranslation());
        }

        Response.Headers.Add("x-mw-storageId", run.IdStoreageEntry.Value.ToString());

        var storageEntry = db.StorageEntries.Find(run.IdStoreageEntry.Value);
        var fileName = Path.ChangeExtension(storageEntry.FileName ?? template.Name, template.FileExtention);

        fileName = Path.GetInvalidFileNameChars()
            .Aggregate(fileName, (current, invalidFileNameChar) => current.Replace(invalidFileNameChar.ToString(), ""));

        string contentType;
        if (displayInline)
        {
            if (run.IsFaulted)
            {
                contentType = MimeTypes.GetMimeType(".json");
            }
            else if (template.FileExtention.ToLower() == "pdf")
            {
                contentType = MimeTypes.GetMimeType(".pdf");
            }
            else if (template.FileExtention.ToLower() == "html")
            {
                contentType = MimeTypes.GetMimeType(".html");
            }
            else
            {
                contentType = MimeTypes.GetMimeType(".txt");
            }

            //if (template.FileExtention.ToLower() == "pdf" || template.FileExtention.ToLower() == "html" && !run.IsFaulted)
            //{
            //	contentType = MimeTypes.GetMimeType(template.FileExtention);
            //}
            //else
            //{
            //	contentType = MimeTypes.GetMimeType(".txt");
            //}
            Response.ContentType = contentType;
            Response.Headers.Add("pragma", "no-cache, public");
            Response.Headers.Add("cache-control", "private, nocache, must-revalidate, maxage=3600");
            var contentDispositionHeader = new System.Net.Http.Headers.ContentDispositionHeaderValue("inline")
            {
                FileName = fileName,
                CreationDate = run.DateCreated,
                ModificationDate = run.DateRun.GetValueOrDefault(),
                ReadDate = DateTime.UtcNow,
                Size = stream.Object.Length,
            };
            var dispositionText = contentDispositionHeader.ToString();
            Response.Headers.Add("content-disposition", dispositionText);
        }
        else
        {
            var contentDispositionHeader = new System.Net.Http.Headers.ContentDispositionHeaderValue("download")
            {
                FileName = fileName,
                CreationDate = run.DateCreated,
                ModificationDate = run.DateRun.GetValueOrDefault(),
                ReadDate = DateTime.UtcNow,
                Size = stream.Object.Length,
            };
            contentType = MimeKit.MimeTypes.GetMimeType("." + template.FileExtention);
            var dispositionText = contentDispositionHeader.ToString();
            Response.Headers.Add("content-disposition", dispositionText);
        }

        return File(stream.Object, contentType);
    }

    [HttpGet]
    [Route("DownloadGeneratedTemplate")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadTemplate(Guid runningTemplateId, string token, bool displayInline = false)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var run = db.NengineRunningTasks.Find(runningTemplateId);
        var template = db.NengineTemplates.Find(run.IdNengineTemplate);
        var httpResponseMessage = await PrepareDownload(db, displayInline, run, template, token);
        return httpResponseMessage;
    }

    [HttpGet]
    [Route("GetScheduledReports")]
    public IActionResult GetScheduledReports()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<NEngineHistoryGet[]>(db.NengineRunningTasks.Where(e => e.IdCreator == User.GetUserId() && e.IsDone == false && e.IsPreview == false && e.IsFaulted == false)));
    }
}