using Newtonsoft.Json;

namespace MyWorksheet.Website.Shared.ViewModels;

public class OperationResultBase<TObject, TError> : ViewModelBase
    where TError : class
{
    public OperationResultBase(TObject successObject)
    {
        Object = successObject;
        Success = true;
    }
    public OperationResultBase(TError error)
    {
        Error = error;
        Success = false;
    }

    [JsonConstructor]
    protected OperationResultBase()
    {

    }

    public TObject Object { get; set; }
    public bool Success { get; set; }
    public TError Error { get; set; }

    public void InclusiveResult<E>(OperationResultBase<E, TError> otherOperationResult)
    {
        otherOperationResult.Success = otherOperationResult.Success & Success;
        otherOperationResult.Error = otherOperationResult.Error ?? Error;
    }
}