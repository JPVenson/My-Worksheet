using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Budget;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Webpage.Helper;

public static class WorksheetHelper
{
    public static async Task DeleteWorksheet(MyworksheetContext db, Guid worksheetId, Guid userId,
        IBudgetService budgetService,
        IBlobManagerService blobManagerService)
    {
        var transaction = await db.Database.BeginTransactionAsync();
        await using (transaction.ConfigureAwait(false))
        {
            var ws = db.Worksheets.Find(worksheetId);

            await db.WorksheetTracks.Where(e => e.IdWorksheet == worksheetId).ExecuteDeleteAsync().ConfigureAwait(false);
            await db.WorksheetItemStatuses.Where(e => e.IdWorksheet == worksheetId).ExecuteDeleteAsync().ConfigureAwait(false);

            var totalTimes = db.WorksheetItems
                .Where(f => f.IdWorksheet == worksheetId).Select(e => e.ToTime - e.FromTime).Sum();
            budgetService.Substract(db, ws.IdProject, userId, totalTimes, true);

            await db.WorksheetItems.Where(e => e.IdWorksheet == worksheetId).ExecuteDeleteAsync().ConfigureAwait(false);

            var storageEntrys = db.WorksheetWorkflowStorageMaps.Where(e => e.IdWorksheetStatusHistoryNavigation.IdWorksheet == worksheetId);

            await foreach (var worksheetWorkflowStorageMap in storageEntrys.AsAsyncEnumerable())
            {
                await blobManagerService.DeleteData(worksheetWorkflowStorageMap.IdStorageEntry, userId);
            }

            await storageEntrys.ExecuteDeleteAsync().ConfigureAwait(false);
            await db.WorksheetStatusHistories.Where(e => e.IdWorksheet == worksheetId).ExecuteDeleteAsync().ConfigureAwait(false);
            db.Worksheets.Remove(ws);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}