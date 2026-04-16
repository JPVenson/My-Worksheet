using System.Linq;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleOnDemand()]
public class ResetWebhookCounterTask : BaseTask
{
    public ResetWebhookCounterTask(IDbContextFactory<MyworksheetContext> dbContextFactory, IUserQuotaService userQuotaService) : base(dbContextFactory)
    {
        _userQuotaService = userQuotaService;
    }

    private readonly IUserQuotaService _userQuotaService;

    public override void DoWork(TaskContext taskContext)
    {
        using var db = DbContectFactory.CreateDbContext();

        db.UserQuota.Where(e => e.QuotaType == _userQuotaService.UserQuotaParts[Quotas.Webhooks].TypeKey)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaValue, 0));
    }

    public override string NamedTask { get; protected set; } = "Reset webhook counter";
}

[ScheduleOnDemand()]
public class ResetReportCounterTask : BaseTask
{
    public ResetReportCounterTask(IDbContextFactory<MyworksheetContext> dbContextFactory, IUserQuotaService userQuotaService) : base(dbContextFactory)
    {
        _userQuotaService = userQuotaService;
    }

    private readonly IUserQuotaService _userQuotaService;

    public override void DoWork(TaskContext taskContext)
    {
        using var db = DbContectFactory.CreateDbContext();
        db.UserQuota.Where(e => e.QuotaType == _userQuotaService.UserQuotaParts[Quotas.ConurrentReports].TypeKey)
            .ExecuteUpdate(f => f.SetProperty(e => e.QuotaValue, 0));
    }

    public override string NamedTask { get; protected set; } = "Reset Report counter";
}

[ScheduleOnDemand()]
public class ResetUnworkedTasksTask : BaseTask
{
    public ResetUnworkedTasksTask(IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
    }

    public override void DoWork(TaskContext taskContext)
    {
        using var db = DbContectFactory.CreateDbContext();
        db.PriorityQueueItems.Where(e => !e.Done)
            .ExecuteUpdate(f => f.SetProperty(e => e.Done, true).SetProperty(f => f.Error, "Reset Manually"));
    }

    public override string NamedTask { get; protected set; } = "Reset Scheduled Tasks";
}