using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Activity;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class SendWebhook : IPriorityQueueAction
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IUserQuotaService _userQuotaService;
    private readonly IActivityService _activityService;

    public SendWebhook(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IUserQuotaService userQuotaService,
        IActivityService activityService)
    {
        _dbContextFactory = dbContextFactory;
        _userQuotaService = userQuotaService;
        _activityService = activityService;
    }

    public const string ActionKey = "SEND_WEBHOOK";
    public string Name => "Sends a Webhook";
    public string Key => ActionKey;
    public Version Version { get; set; }

    public void PrepaireWebhookData(IWebHookObject data)
    {
        data.CreatedAt = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return new DictionaryElementsValidator<string, object>(arguments)
            .OfType<Guid>("typeId")
            .Is<IWebHookObject>("webhookData")
            .OfType<string>("callerIp")
            .Result;
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        var userId = queueElement.UserId;
        var typeId = (Guid)queueElement.Arguments.GetOrNull("typeId");
        var callerIp = (string)queueElement.Arguments.GetOrNull("callerIp");
        var data = (IWebHookObject)queueElement.Arguments.GetOrNull("webhookData");


        var hookInfos = (await PrepareWebhookForSend(userId, typeId, callerIp)).Where(e => e.Item2.Success).ToArray();
        foreach (var hookInfo in hookInfos)
        {
            PrepaireWebhookData(data);
            await ExecuteSendWebhoData(data, hookInfo.Item1, hookInfo.Item2);
        }
    }

    private async Task<Tuple<OutgoingWebhook, OutgoingWebhookActionLog>[]> PrepareWebhookForSend(Guid userId, Guid webhookTypeId, string callerIp)
    {
        using var dbEntities = _dbContextFactory.CreateDbContext();
        var hookInfos = new List<Tuple<OutgoingWebhook, OutgoingWebhookActionLog>>();
        var webhook = dbEntities.OutgoingWebhooks.Where(f => f.IsActive)
            .Where(f => f.IdOutgoingWebhookCase == webhookTypeId)
            .Where(f => f.IdCreator == userId)
            .ToArray();
        foreach (var outgoingWebhook in webhook)
        {
            var outgoingWebhookActionLog = new OutgoingWebhookActionLog
            {
                DateOfAction = DateTime.UtcNow,
                IdOutgoingWebhook = outgoingWebhook.OutgoingWebhookId,
                Success = true,
                IdAppUser = userId,
                InitiatorIp = callerIp,
                ReturnCode = 0
            };
            if (!(await _userQuotaService.Add(userId, 1, Quotas.Webhooks)))
            {
                outgoingWebhookActionLog.Success = false;
                outgoingWebhookActionLog.Error = "Exceeded Quota";
            }

            dbEntities.Add(outgoingWebhookActionLog);
            //AccessElement<WebhookHubInfo>.Instance.SendHistoryChanged((int) userId, outgoingWebhook.OutgoingWebhookId);
            if (outgoingWebhookActionLog.Success)
            {
                hookInfos.Add(new Tuple<OutgoingWebhook, OutgoingWebhookActionLog>(outgoingWebhook, outgoingWebhookActionLog));
            }
        }
        dbEntities.SaveChanges();

        return hookInfos.ToArray();
    }

    private async Task CheckAutoDisableForWebhookData(OutgoingWebhook hookInfos, string callerIp)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var countExceptionsInPast = db.OutgoingWebhookActionLogs.Where(f => f.IdOutgoingWebhook == hookInfos.OutgoingWebhookId)
            .Where(f => f.Success == false)
            .Where(f => f.DateOfAction > DateTime.UtcNow.AddDays(-4))
            .ToArray();
        if (countExceptionsInPast.Length <= 5)
        {
            return;
        }

        hookInfos.IsActive = false;
        db.OutgoingWebhooks.Where(f => f.OutgoingWebhookId == hookInfos.OutgoingWebhookId)
            .ExecuteUpdate(f => f.SetProperty(e => e.IsActive, false));
        db.Add(new OutgoingWebhookActionLog
        {
            Success = false,
            DateOfAction = DateTime.UtcNow,
            Error = "Webhook disabled for 5 or more errors in 4 Days",
            IdOutgoingWebhook = hookInfos.OutgoingWebhookId,
            IdAppUser = hookInfos.IdCreator,
            InitiatorIp = callerIp,
        });

        //AccessElement<WebhookHubInfo>.Instance.SendHistoryChanged(hookInfos.IdCreator, hookInfos.OutgoingWebhookId);
        await _activityService.CreateActivity(ActivityTypes.ActivityWebhookDisabled.CreateActivity(db, hookInfos));
        db.SaveChanges();
    }

    private async Task ExecuteSendWebhoData(
        IWebHookObject data,
        OutgoingWebhook hookInfos,
        OutgoingWebhookActionLog logEntry)
    {
        data.SendAt = DateTime.UtcNow;
        try
        {
            using (var httpClient = new HttpClient())
            {
                HttpBlobStorageProvider.SetCommonHeaders(httpClient);
                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true
                };
                var targetEncoding = Encoding.UTF8;
                var jsonData = JsonConvert.SerializeObject(data);

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-My-Worksheet-Secret", hookInfos.Secret);
                using (var httpContent = new StringContent(jsonData, targetEncoding, "application/json"))
                {
                    var httpResponseMessage = await httpClient.PostAsync(hookInfos.CallingUrl, httpContent).ConfigureAwait(false);
                    logEntry.ReturnCode = (int)httpResponseMessage.StatusCode;
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        return;
                    }

                    logEntry.Success = false;
                    logEntry.Error = httpResponseMessage.ReasonPhrase;
                    await CheckAutoDisableForWebhookData(hookInfos, logEntry.InitiatorIp);
                }
            }
        }
        catch (Exception e)
        {
            logEntry.Success = false;
            logEntry.Error = e.InnerException?.Message ?? e.Message;
            await CheckAutoDisableForWebhookData(hookInfos, logEntry.InitiatorIp);
        }
        finally
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Update(logEntry);
        }
    }
}