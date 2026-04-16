using System;
using System.Collections.Generic;
using MyWorksheet.Shared.Helper;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Website.Server.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Shared.ViewModels.Notifications;
using ServiceLocator.Attributes;

namespace MyWorksheet.Webpage.Services.WebHooks;
//public class WebhookInfo
//{
//	public WebhookInfo()
//	{
//		OutgoingWebhooks = new ConcurrentBag<OutgoingWebhook>();
//	}

//	public ConcurrentBag<OutgoingWebhook> OutgoingWebhooks { get; private set; }

//	public bool CanSendBuffer
//	{
//		get { return SendCounterLengthBuffer > SendCounterBuffer; }
//	}

//	public int SendCounterBuffer { get; set; }
//	public int SendCounterLengthBuffer { get; set; }
//	public DateTime LastCheckedDate { get; set; }

//	public bool CheckSendCounter(DbEntities db, Guid userId)
//	{
//		if (LastCheckedDate < DateTime.UtcNow.Date)
//		{
//			var userCounter = db.UserCounters.Where(f => f.UserId == userId)
//								.FirstOrDefault();
//			if (userCounter == null)
//			{
//				throw new ArgumentNullException(nameof(userCounter), "User does not have a UserCounter");
//			}
//			LastCheckedDate = DateTime.UtcNow.Date;
//			SendCounterLengthBuffer = userCounter.OutgoingWebhookLengthCounter;
//			SendCounterBuffer = userCounter.OutgoingWebhookCounter;
//			return CanSendBuffer;
//		}
//		return CanSendBuffer;
//	}

//	public void LoadForUser(Guid userId, DbEntities db)
//	{
//		var outgoingWebhooks = db.OutgoingWebhooks.Where.Column(f => f.IdCreator).Is
//								 .EqualsTo(userId).ToArray();
//		foreach (var outgoingWebhook in outgoingWebhooks)
//		{
//			OutgoingWebhooks.Add(outgoingWebhook);
//		}

//		CheckSendCounter(db, userId);
//	}
//}

public interface IWebHookService
{
    void PublishEvent(IWebhookType type, IWebHookObject data, Guid user, string calleeIp);
}

[SingletonService(typeof(IWebHookService))]
public class WebHookService : IWebHookService
{
    private readonly IServerPriorityQueueManager _queueManager;

    public WebHookService(IServerPriorityQueueManager queueManager)
    {
        _queueManager = queueManager;
    }

    public void PublishEvent(IWebhookType type, IWebHookObject data, Guid user, string calleeIp)
    {
        _queueManager.Enqueue(PriorityManagerLevel.FireAndForget, SendWebhook.ActionKey, user, new Dictionary<string, object>
        {
            {"typeId", type.Id},
            {"webhookData", data},
            {"callerIp", calleeIp}
        }).AttachNonVerboseAsyncHandler();
    }
}