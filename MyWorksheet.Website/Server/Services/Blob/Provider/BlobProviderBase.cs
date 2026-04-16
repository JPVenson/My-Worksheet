using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.Util;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public abstract class BlobProviderBase : IBlobProvider
{
    public Guid StorageInstance { get; private set; }
    protected readonly StorageProviderData[] Data;
    protected IDbContextFactory<MyworksheetContext> DbContextFactory { get; }

    public BlobProviderBase(Guid storageInstance, StorageProviderData[] data, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        StorageInstance = storageInstance;
        Data = data;
        DbContextFactory = dbContextFactory;
        Timeout = TimeSpan.FromSeconds(5);
    }

    protected string GetDataFromStore(string key)
    {
        return Data.First(e => e.Key.Equals(key)).Value;
    }

    protected string GetEncryptedDataFromStore(string key, Guid userId)
    {
        var value = GetDataFromStore(key);
        if (IsTestInProgress)
        {
            return value;
        }

        var db = DbContextFactory.CreateDbContext();
        var username = db.AppUsers.Find(userId).Username;
        var storageProvider = db.StorageProviders.Find(StorageInstance).StorageKey;
        return ChallangeUtil.DecryptPassword(value, username + storageProvider);
    }

    public StorageEntry CreateFromBlob(MyworksheetContext db, BlobData data, StorageEntityType storageType, Guid userId)
    {
        return new StorageEntry()
        {
            ContentType = data.MimeType,
            FileName = data.Filename,
            IdStorageProvider = StorageInstance,
            IdStorageType = db.StorageTypes.AsNoTracking().Where(e => e.Name == storageType.ToString()).Select(e => e.StorageTypeId).First(),
            StorageKey = Guid.NewGuid().ToString() + ".bin",
            IdAppUser = userId
        };
    }

    protected virtual void DeleteStorageItem(Guid storageEntryId)
    {
        if (IsTestInProgress)
        {
            return;
        }

        var db = DbContextFactory.CreateDbContext();
        db.StorageEntries.Where(e => e.StorageEntryId == storageEntryId)
            .ExecuteUpdate(e => e.SetProperty(f => f.IsDeleted, true));
    }

    public TimeSpan Timeout { get; set; }

    public abstract Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db);
    public abstract Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db);
    public abstract Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db);
    public abstract IObjectSchema GetSchema();
    public bool IsTestInProgress { get; set; }

    public virtual string HelpText(Guid appUserId)
    {
        return null;
    }

    public virtual async Task<IEnumerable<string>> Test(Guid appUserId)
    {
        var testReport = new List<string>();
        IsTestInProgress = true;

        try
        {
            var db = DbContextFactory.CreateDbContext();
            var transaction = await db.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                if (StorageInstance == Guid.Empty)
                {
                    var provider = new StorageProvider()
                    {
                        IdAppUser = appUserId,
                        IsDefaultProvider = false,
                        Name = "TEST_PROVIDER",
                        StorageKey = "TEST_KEY",
                    };
                    db.Add(provider);
                    StorageInstance = provider.StorageProviderId;
                }

                var testData = "This is a Test string from My-Worksheet";

                testReport.Add($"{DateTime.UtcNow}: Begin Upload of test String: '{testData}'");
                var storageEntiry = await SetData(BlobData.GetTestData(new MemoryStream(Encoding.UTF8.GetBytes(testData))), appUserId, StorageEntityType.Test, db);
                testReport.Add($"{DateTime.UtcNow}: End Upload");

                if (storageEntiry == null)
                {
                    testReport.Add($"{DateTime.UtcNow}: The Upload did not work for unknown Reasons");
                    return testReport;
                }

                testReport.Add($"{DateTime.UtcNow}: Begin Download");
                string dlData = "";
                var res = await GetData(storageEntiry.StorageEntryId, appUserId, db);
                if (res.Success)
                {
                    testReport.Add($"{DateTime.UtcNow}: Download failed");
                    using (var data = res.Stream)
                    {
                        testReport.Add($"{DateTime.UtcNow}: End Download");

                        using (var ms = new MemoryStream())
                        {
                            data.CopyTo(ms);
                            dlData = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }

                if (!dlData.Equals(testData, StringComparison.InvariantCulture))
                {
                    testReport.Add($"{DateTime.UtcNow}: The Download did not work for unknown Reasons");
                    return testReport;
                }

                testReport.Add($"{DateTime.UtcNow}: Begin Delete");
                await DeleteData(storageEntiry.StorageEntryId, appUserId, db);
                testReport.Add($"{DateTime.UtcNow}: End Delete");
            }
            finally
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
            }
        }
        catch (AggregateException e)
        {
            testReport.Add($"{DateTime.UtcNow}: Test Did not Succeed because:");
            foreach (var eInnerException in e.Flatten().InnerExceptions)
            {
                testReport.Add(eInnerException.Message);
            }

            return testReport;
        }
        catch (Exception e)
        {
            testReport.Add($"{DateTime.UtcNow}: Test Did not Succeed because: {e.Message}");
            return testReport;
        }
        finally
        {
            IsTestInProgress = false;
        }
        testReport.Add($"{DateTime.UtcNow}: Selftest Success");
        return testReport;
    }

    public abstract long MaxSizeInBytes { get; protected set; }
}