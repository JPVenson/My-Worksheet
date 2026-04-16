using System;
using System.IO;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public class DomainGate : MarshalByRefObject
{
    public byte[] CreateThumbnail(IInMemoryThumbnailProvider provider, Stream stream, string filename, int x, int y)
    {
        return provider.CreateThumbnail(stream, filename, x, y);
        //var domain = CreateDomain();
        //byte[] result = null;
        //domain.DoCallBack(() =>
        //{
        //});
        //return result;
    }
}