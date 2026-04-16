using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Util.WorksheetItemUtil;

public static class MergeReporter
{
    public static MergeReport MergeReport(DateTimeOffset fromTime, DateTimeOffset toTime,
        MyworksheetContext db, Guid idWorksheet)
    {
        var wsItems = db.WorksheetItems
            .Where(f => f.IdWorksheet == idWorksheet)
            .Where(f => f.DateOfAction > fromTime && f.DateOfAction < toTime)
            .ToArray();
        return MergeReport(fromTime, toTime, wsItems);
    }

    public static MergeReport MergeReport(DateTimeOffset fromTime, DateTimeOffset toTime, IEnumerable<WorksheetItem> onItems)
    {
        var onItemsList = onItems
            .Select(f => new
            {
                WsItem = f,
                FromTime = f.DateOfAction.Date.Add(TimeSpan.FromMinutes(f.FromTime)),
                ToTime = f.DateOfAction.Date.Add(TimeSpan.FromMinutes(f.ToTime)),
            })
            .Where(e =>
                e.FromTime < fromTime && e.ToTime > toTime
                ||
                e.FromTime > fromTime && e.ToTime < toTime
                ||
                e.FromTime < toTime && e.ToTime > toTime
                ||
                e.FromTime == fromTime && e.ToTime == toTime
            )
            .ToArray();
        return new MergeReport()
        {
            Overlapping = onItemsList.Select(f => f.WsItem).ToArray()
        };
    }
}

public class MergeReport
{
    public WorksheetItem[] Overlapping { get; set; }
}