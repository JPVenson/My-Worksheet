using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.Services.ExecuteLater.ArgumentStore;

[SingletonService(typeof(IPriorityQueueArgumentStore))]
public class PriorityQueueDbArgumentStore : IPriorityQueueArgumentStore
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly Version _currentVersion = new Version(0, 0, 0, 1);

    public PriorityQueueDbArgumentStore(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc />
    public async Task<PriorityQueueElement> GetFromStore(Guid queueItemId)
    {
        await Task.CompletedTask;
        var db = _dbContextFactory.CreateDbContext();
        var queueItem = db.PriorityQueueItems.Find(queueItemId);
        if (queueItem == null)
        {
            return null;
        }

        return _mapper.SchedulerMapper.Map<PriorityQueueElement>(queueItem);
    }

    public async Task<Tuple<Guid, PriorityQueueElement>[]> GetResume()
    {
        //return null;
        var db = _dbContextFactory.CreateDbContext();
        var openItems = db.PriorityQueueItems
            .Where(f => !f.Done && f.Error == null)
            .Where(f => f.Success == false)
            .ToArray();
        return _mapper.SchedulerMapper.Map<PriorityQueueElement[]>(openItems)
            .Zip(openItems.Select(f => f.PriorityQueueItemId), (element, i) => new Tuple<Guid, PriorityQueueElement>(i, element))
            .ToArray();
    }

    /// <inheritdoc />
    public Guid[] GetChilds(Guid queueItemId)
    {
        var db = _dbContextFactory.CreateDbContext();
        return db.PriorityQueueItems.Where(e => e.IdParent == queueItemId)
            .Select(e => e.PriorityQueueItemId)
            .ToArray();
    }

    /// <inheritdoc />
    public Guid SetInStore(PriorityQueueElement element)
    {
        var db = _dbContextFactory.CreateDbContext();
        var mappedTo = _mapper.SchedulerMapper.Map<PriorityQueueItem>(element);
        mappedTo.Version = _currentVersion.ToString();
        var now = DateTimeOffset.Now;
        mappedTo.DateOfCreationOffset = (short)now.Offset.TotalMinutes;
        mappedTo.DateOfCreation = now.ToUniversalTime();
        db.Add(mappedTo);
        db.SaveChanges();
        return mappedTo.PriorityQueueItemId;
    }

    /// <inheritdoc />
    public Guid SetInStore(PriorityQueueElement element, Guid childOf)
    {
        var db = _dbContextFactory.CreateDbContext();
        var mappedTo = _mapper.SchedulerMapper.Map<PriorityQueueItem>(element);
        mappedTo.IdParent = childOf;
        mappedTo.Version = _currentVersion.ToString();
        var now = DateTimeOffset.Now;
        mappedTo.DateOfCreationOffset = (short)now.Offset.TotalMinutes;
        mappedTo.DateOfCreation = now.ToUniversalTime();
        db.Add(mappedTo);
        db.SaveChanges();
        return mappedTo.PriorityQueueItemId;
    }

    /// <inheritdoc />
    public void SetFailed(Guid taskQueueItemId, Exception exception)
    {
        var db = _dbContextFactory.CreateDbContext();
        var now = DateTimeOffset.Now;
        db.PriorityQueueItems.Where(e => e.PriorityQueueItemId == taskQueueItemId)
            .ExecuteUpdate(e => e
                .SetProperty(f => f.Error, JsonConvert.SerializeObject(exception))
                .SetProperty(f => f.Success, false)
                .SetProperty(f => f.Done, true)
                .SetProperty(f => f.DateOfDoneOffset, (short)now.Offset.TotalMinutes)
                .SetProperty(f => f.DateOfDone, now.ToUniversalTime())
        );
    }

    /// <inheritdoc />
    public void SetDone(Guid taskQueueItemId)
    {
        var db = _dbContextFactory.CreateDbContext();
        db.PriorityQueueItems.Where(e => e.PriorityQueueItemId == taskQueueItemId)
            .ExecuteUpdate(e => e
                .SetProperty(f => f.Success, true)
                .SetProperty(f => f.Done, true)
                .SetProperty(f => f.DateOfDone, DateTimeOffset.Now)
        );
    }
}