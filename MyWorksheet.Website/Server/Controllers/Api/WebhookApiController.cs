using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Webpage.Services.WebHooks;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ExternalDomainValidator;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api;

[RevokableAuthorize]
[Route("api/Webhook/OutgoingApi")]
public class WebhookApiControllerBase : ApiControllerBase
{
    private static readonly string[] _allowedSchemas = { "http", "https" };
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;
    private readonly WebHookService _service;
    private readonly IExternalDomainValidator _externalDomainValidator;
    private readonly INumberRangeService _numberRangeService;

    public WebhookApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IMapperService mapper,
        WebHookService service,
        IExternalDomainValidator externalDomainValidator,
        INumberRangeService numberRangeService)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _service = service;
        _externalDomainValidator = externalDomainValidator;
        _numberRangeService = numberRangeService;
    }

    [NonAction]
    private string CreateSecret(string username)
    {
        var secret = "";
        var rand = new Random(username.GetHashCode() + DateTime.UtcNow.Millisecond);
        for (var i = 0; i < 12; i++)
        {
            secret += (char)rand.Next('0', 'z');
        }
        return secret;
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/Webhooks")]
    public IActionResult GetAllWebhooks(Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var outgoingWebhooks = db.OutgoingWebhooks
            .Where(f => f.IdCreator == userId)
            .Where(f => f.IsDeactivated == false)
            .ToArray();
        var outgoingWebhookModels = _mapper.ViewModelMapper.Map<OutgoingWebhookModelGet[]>(outgoingWebhooks);
        return Data(outgoingWebhookModels);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/Get")]
    public IActionResult GetSingle(Guid webhookId, Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<OutgoingWebhookModelGet>(db.OutgoingWebhooks
            .Where(f => f.IdCreator == userId)
            .Where(f => f.OutgoingWebhookId == webhookId)
            .FirstOrDefault()));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/Log")]
    public IActionResult GetLogAdmin(Guid webhookId, int page, int size)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<PageResultSet<OutgoingWebhookActionLogModel>>(db
            .OutgoingWebhookActionLogs.Where(f => f.IdOutgoingWebhook == webhookId)
            .OrderByDescending(f => f.DateOfAction)
            .ForPagedResult(page, size)));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpPost]
    [Route("Admin/Create")]
    public async Task<IActionResult> CreateWebhookData([FromBody] OutgoingWebhookModelGet model, Guid userId)
    {
        var webhook = _mapper.ViewModelMapper.Map<OutgoingWebhook>(model);
        webhook.IdCreator = userId;
        webhook.IsActive = false;
        var isValid = _externalDomainValidator.ValidateUrl(webhook.CallingUrl, _allowedSchemas);
        if (!isValid.IsValid)
        {
            return BadRequest(isValid.Error);
        }
        webhook.Secret = CreateSecret(User.Identity.Name);

        using var db = _dbContextFactory.CreateDbContext();

        var transaction = await db.Database.BeginTransactionAsync();
        await using (transaction.ConfigureAwait(false))
        {
            webhook.NumberRangeEntry = await _numberRangeService.GetNumberRangeAsync(db, WebhookNumberRangeFactory.NrCode, userId, webhook);
            db.Add(webhook);

            await db.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        var outgoingWebhookModelGet = _mapper.ViewModelMapper.Map<OutgoingWebhookModelGet>(webhook);
        return Data(outgoingWebhookModelGet);
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpPost]
    [Route("Admin/Update")]
    public async Task<IActionResult> UpdateWebhookData([FromBody] OutgoingWebhookModelGet model, Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var webhook = db.OutgoingWebhooks.Find(model.OutgoingWebhookId);
        model.NumberRangeEntry = webhook.NumberRangeEntry;
        webhook = _mapper.ViewModelMapper.Map(model, webhook);
        webhook.IdCreator = userId;
        var isValid = _externalDomainValidator.ValidateUrl(webhook.CallingUrl, _allowedSchemas);
        if (!isValid.IsValid)
        {
            return BadRequest(isValid.Error);
        }
        webhook.Secret = CreateSecret(User.Identity.Name);
        var transaction = await db.Database.BeginTransactionAsync();
        await using (transaction.ConfigureAwait(false))
        {
            db.OutgoingWebhooks.Where(e => e.OutgoingWebhookId == webhook.OutgoingWebhookId).ExecuteUpdate(e => e.SetProperty(f => f.IsDeactivated, true));
            webhook.NumberRangeEntry = await _numberRangeService.GetNumberRangeAsync(db, WebhookNumberRangeFactory.NrCode, userId, webhook);
            db.Add(webhook);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }

        return Data(_mapper.ViewModelMapper.Map<OutgoingWebhookModelGet>(webhook));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpPost]
    [Route("Admin/Delete")]
    public IActionResult DeleteWebhookData(Guid webhookId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.OutgoingWebhooks.Where(e => e.OutgoingWebhookId == webhookId)
            .ExecuteUpdate(e => e.SetProperty(f => f.IsDeactivated, true).SetProperty(f => f.IsActive, false));
        return Data();
    }

    [RevokableAuthorize]
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateWebhookData([FromBody] OutgoingWebhookModelGet model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        return await CreateWebhookData(model, User.GetUserId()).ConfigureAwait(false);
    }

    [RevokableAuthorize]
    [HttpPost]
    [Route("Update")]
    public async Task<IActionResult> UpdateWebhookData([FromBody] OutgoingWebhookModelGet model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        using var db = _dbContextFactory.CreateDbContext();
        var hook = db.OutgoingWebhooks.Find(model.OutgoingWebhookId);

        if (hook.IdCreator != User.GetUserId())
        {
            return Unauthorized();
        }

        return await UpdateWebhookData(model, User.GetUserId());
    }

    [RevokableAuthorize()]
    [HttpPost]
    [Route("Delete")]
    public IActionResult DeleteWebhookDataForMe(Guid webhookId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var hook = db.OutgoingWebhooks.Find(webhookId);

        if (hook.IdCreator != User.GetUserId())
        {
            return Unauthorized();
        }

        return DeleteWebhookData(webhookId);
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("ExampleCase")]
    public IActionResult WebhookTestData(Guid caseId)
    {
        var type = WebHookTypes.YieldTypes().FirstOrDefault(f => f.Id == caseId);
        if (type == null)
        {
            return BadRequest();
        }
        var defaultData = type.DefaultContent() as WebHookObject;
        //var jSchemaGenerator = new JSchemaGenerator();
        //var jSchema = jSchemaGenerator.Generate(defaultData);
        return Data(JsonSchemaExtensions.JsonSchema(defaultData));
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("Webhooks")]
    public IActionResult GetAllWebhooks()
    {
        return GetAllWebhooks(User.GetUserId());
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("Get")]
    public IActionResult GetSingle(Guid webhookId)
    {
        return GetSingle(webhookId, User.GetUserId());
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("Log")]
    public IActionResult GetLog(Guid webhookId, int page, int size)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var hook = db.OutgoingWebhooks.Find(webhookId);
        if (hook.IdCreator != User.GetUserId())
        {
            return Unauthorized();
        }

        return GetLogAdmin(webhookId, page, size);
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("WebhookCase")]
    public IActionResult GetAllWebhooksCases()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<OutgoingWebhookCaseModel[]>(db.OutgoingWebhookCases));
    }
}