using System.IO;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Server.Services.Blob;

public class BlobManagerGetOperationResult : StandardOperationResultBase<Stream>
{
    public BlobManagerGetOperationResult(Stream successObject) : base(successObject)
    {
    }

    public BlobManagerGetOperationResult(string error) : base(error)
    {
    }
}