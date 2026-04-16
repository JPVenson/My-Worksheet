using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Util.Promise;

public class ValueState<TValue> : IEntityState
{
    private TValue _entity;

    public virtual TValue Entity
    {
        get { return _entity; }
        set
        {
            _entity = value;
        }
    }

    public ValueState(TValue entity)
    {
        _entity = entity;
        PristineState = GetValueState() ?? 0;
    }

    protected int PristineState { get; set; }

    private int? GetValueState()
    {
        if (Entity is IEntityObject eo)
        {
            return eo.State();
        }

        return Entity?.GetHashCode();
    }

    public virtual bool IsObjectDirty
    {
        get { return (GetValueState() ?? 0) != PristineState; }
    }

    public virtual void SetPristine()
    {
        PristineState = GetValueState() ?? 0;
    }

    public static implicit operator ValueState<TValue>(TValue value)
    {
        if (value == null)
        {
            return null;
        }

        return new ValueState<TValue>(value);
    }

    public static implicit operator ValueState<TValue>(ApiResult<TValue> value)
    {
        if (!value.Success)
        {
            return null;
        }

        return new ValueState<TValue>(value.Object);
    }
}