using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public class FutureTrackedList<TValue> : FutureList<TValue>, IFutureTrackedList<TValue> where TValue : IEntityObject
{
    public FutureTrackedList(Func<Task<ApiResult<TValue[]>>> loader) : base(loader)
    {
        EntityStates = new List<EntityState<TValue>>();
    }

    public FutureTrackedList(Func<Task<TValue[]>> loader) : base(loader)
    {
        EntityStates = new List<EntityState<TValue>>();
    }

    public IList<EntityState<TValue>> EntityStates { get; set; }

    public IEnumerable<EntityState<TValue>> GetStates()
    {
        lock (this)
        {
            return EntityStates.ToArray();
        }

    }

    public EntityState<TValue> State(TValue value)
    {
        lock (this)
        {
            return EntityStates.FirstOrDefault(e => Equals(e.Entity, value));
        }
    }

    public override void RemoveWhere(Func<TValue, bool> condition)
    {
        lock (this)
        {
            foreach (var entityObject in this.ToArray())
            {
                if (condition(entityObject))
                {
                    Remove(entityObject);
                }
            }
        }
    }

    public override void Reset()
    {
        lock (this)
        {
            EntityStates.Clear();
            Loaded = false;
        }
    }

    public override void AddRange(IEnumerable<TValue> values)
    {
        lock (this)
        {
            foreach (var value in values.Where(e => !Contains(e)))
            {
                EntityStates.Add(new EntityState<TValue>(value, EntityListState.Pristine));
            }
        }
    }

    public override void Add(TValue item)
    {
        if (item is null)
        {
            throw new NotSupportedException("Item cannot be null");
        }

        lock (this)
        {
            if (Contains(item))
            {
                return;
            }

            EntityStates.Add(new EntityState<TValue>(item, EntityListState.Added));
        }
    }

    public override void Clear()
    {
        lock (this)
        {
            foreach (var entityState in EntityStates.ToArray())
            {
                if (entityState.ListState == EntityListState.Added)
                {
                    EntityStates.Remove(entityState);
                }
                else
                {
                    entityState.ListState = EntityListState.Deleted;
                }
            }
        }
    }

    public override bool Contains(TValue item)
    {
        lock (this)
        {
            if (item == null)
            {
                return false;
            }

            return EntityStates.Any(f => f.Entity.State().Equals(item.State()));
        }
    }

    public override void CopyTo(TValue[] array, int arrayIndex)
    {
        lock (this)
        {
            EntityStates.Select(e => e.Entity).ToArray().CopyTo(array, arrayIndex);
        }
    }

    public override bool Remove(TValue item)
    {
        lock (this)
        {

            var hasItem = EntityStates.FirstOrDefault(e => e.Entity.Equals(item));
            if (hasItem == null)
            {
                return false;
            }

            if (hasItem.ListState == EntityListState.Added)
            {
                EntityStates.Remove(hasItem);
                return true;
            }

            hasItem.ListState = EntityListState.Deleted;
            return true;
        }
    }

    public override int Count
    {
        get
        {
            return EntityStates
                .Count(e => e.ListState != EntityListState.Deleted);
        }
    }

    public override bool IsReadOnly
    {
        get
        {
            return Loading;
        }
    }

    public override IEnumerator<TValue> GetEnumerator()
    {
        lock (this)
        {

            if (NeedsLoading)
            {
                LoadWith(Loader());
            }

            var enumerable = EntityStates.ToArray()
                .Where(e => e.ListState != EntityListState.Deleted);
            return enumerable
                .Select(e => e.Entity)
                .GetEnumerator();
        }
    }
}