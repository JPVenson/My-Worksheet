using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Monetary;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleTaskAt(1, 0, 0, 0)]
public class RefreshExchangeRates : BaseTask
{
    private readonly IValueExchangeService _exchangeService;

    public RefreshExchangeRates(IValueExchangeService exchangeService, IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        _exchangeService = exchangeService;
    }

    public override Task DoWorkAsync(TaskContext context)
    {
        return _exchangeService.RefreshExchangeRates();
    }

    public override string NamedTask { get; protected set; } = "Refresh Exchange rates from EZB";
}