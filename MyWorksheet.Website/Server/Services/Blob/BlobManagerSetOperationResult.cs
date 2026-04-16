using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Server.Services.Blob;

public class BlobManagerSetOperationResult : StandardOperationResultBase<StorageEntry>
{
    public BlobManagerSetOperationResult(StorageEntry successObject) : base(successObject)
    {
    }

    public BlobManagerSetOperationResult(string error) : base(error)
    {
    }
}