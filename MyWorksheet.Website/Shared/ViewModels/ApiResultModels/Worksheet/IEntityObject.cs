using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public interface IEntityObject
{
    int State();
    ViewModelState SnapshotState();
    Guid? GetModelIdentifier();
}