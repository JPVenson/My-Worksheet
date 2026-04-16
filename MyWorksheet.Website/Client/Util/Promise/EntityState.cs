using System;
using System.Collections;
using System.Collections.Generic;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public class EntityState<TValue> : IEntityState<TValue>
    where TValue : IEntityObject
{
    public EntityState(TValue value, EntityListState listState)
    {
        ListState = listState;
        Entity = value;
        PristineState = GetState();
    }

    public EntityState(TValue value) : this(value, value?.GetModelIdentifier() == Guid.Empty ? EntityListState.Added : EntityListState.Pristine)
    {
    }

    public EntityListState ListState { get; set; }
    public TValue Entity { get; private set; }
    public ViewModelState PristineState { get; set; }

    public void SetPristine()
    {
        PristineState = GetState();
        ListState = EntityListState.Pristine;
    }

    public bool IsObjectDirty
    {
        get { return GetState()?.Equals(PristineState) == false || ListState != EntityListState.Pristine; }
    }

    private ViewModelState GetState()
    {
        return Entity?.SnapshotState();
    }

    public static implicit operator EntityState<TValue>(TValue value)
    {
        if (value == null)
        {
            return null;
        }

        return new EntityState<TValue>(value);
    }

    public static implicit operator EntityState<TValue>(ApiResult<TValue> value)
    {
        if (!value.Success)
        {
            return null;
        }

        return new EntityState<TValue>(value.Object);
    }

    public ApiEntityState<TValue> AsApiEntity()
    {
        if (ListState == EntityListState.Deleted)
        {
            return new ApiEntityState<TValue>(Entity?.GetModelIdentifier());
        }

        if (ListState == EntityListState.Added)
        {
            return new ApiEntityState<TValue>(Entity, EntityListState.Added);
        }

        if (IsObjectDirty)
        {
            return new ApiEntityState<TValue>(Entity, EntityListState.Unknown);
        }

        return null;
    }
}