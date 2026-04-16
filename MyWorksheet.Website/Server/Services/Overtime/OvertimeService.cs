using System;
using System.Linq;
using MyWorksheet.Helper.Db;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;
using MyWorksheet.Website.Server.Services.Worktime;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Overtime;

[ScopedService()]
public class OvertimeService
{
    private readonly IUserWorktimeService _userWorktimeService;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public OvertimeService(IUserWorktimeService userWorktimeService,
        IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _userWorktimeService = userWorktimeService;
        _dbContextFactory = dbContextFactory;
    }

    public void AccountOvertime(Worksheet worksheet)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var project = db.Projects.Find(worksheet.IdProject);
        if (!project.BookToOvertimeAccount)
        {
            return;
        }

        var activeOvertimeAccount = db.OvertimeAccounts
            .Where(f => f.IdProject == project.ProjectId)
            .Where(f => f.IsActive == true)
            .Where(f => f.IdAppUser == worksheet.IdCreator)
            .FirstOrDefault();

        if (activeOvertimeAccount == null)
        {
            return;
        }

        var wsItems = db.WorksheetItems
            .Where(f => f.IdWorksheet == worksheet.WorksheetId)
            .ToArray();

        var workloadForProject = _userWorktimeService.GetWorkloadForProject(db, project.ProjectId, worksheet.IdCreator);

        decimal worktime = 0;
        decimal meanWorktime = 0;

        foreach (var weekedWsItems in wsItems.GroupBy(e => GlobalFormatter.WeekOfDate(e.DateOfAction)))
        {
            for (int i = 0; i < (int)DayOfWeek.Saturday; i++)
            {
                var dayOfWeek = (DayOfWeek)i;
                meanWorktime += workloadForProject.MeanWorktimeForDay(dayOfWeek);
                worktime += weekedWsItems.Where(e => e.DateOfAction.DayOfWeek == dayOfWeek)
                    .Select(f => f.ToTime - f.FromTime).Sum();
            }

            //worktime += 
            //var overtime = Math.Max(0, weekedWsItems.GroupBy(f => f.DateOfAction.Day)
            //	.Select(f =>
            //		f.Select(e => e.ToTime - e.FromTime).Sum() -
            //		workloadForProject.GetMeanWorktimeForDay(f.First().DateOfAction.DateTime)).Sum());
            //overtimeInWorksheet += overtime;

            //if (overtime > 0)
            //{
            //	overtimeBookings.Add(new OvertimeTransaction()
            //	{
            //		DateOfAction = DateTimeOffset.Now.Date,
            //		Value = overtime,
            //		IdOvertimeAccount = activeOvertimeAccount.OvertimeAccountId,
            //		Withdraw = false,
            //	});
            //}
        }

        var overtime = meanWorktime - worktime;
        if (overtime > 0)
        {
            var nowPlus = DateTimeOffset.Now;
            db.Add(new OvertimeTransaction()
            {
                DateOfActionOffset = (short)nowPlus.Offset.TotalMinutes,
                DateOfAction = nowPlus.ToUniversalTime(),
                Value = overtime,
                IdOvertimeAccount = activeOvertimeAccount.OvertimeAccountId,
                Withdraw = false
            });
            activeOvertimeAccount.OvertimeValue += overtime;
        }

        if (overtime < 0)
        {
            var nowMinus = DateTimeOffset.Now;
            db.Add(new OvertimeTransaction()
            {
                DateOfActionOffset = (short)nowMinus.Offset.TotalMinutes,
                DateOfAction = nowMinus.ToUniversalTime(),
                Value = overtime * -1,
                IdOvertimeAccount = activeOvertimeAccount.OvertimeAccountId,
                Withdraw = true
            });
            activeOvertimeAccount.OvertimeValue += overtime;
        }
        db.SaveChanges();
    }
}