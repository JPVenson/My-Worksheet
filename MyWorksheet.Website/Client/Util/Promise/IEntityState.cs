namespace MyWorksheet.Website.Client.Util.Promise;

public interface IEntityState
{
    bool IsObjectDirty { get; }
    void SetPristine();
}

public interface IEntityState<out T> : IEntityState
{
    T Entity { get; }
}