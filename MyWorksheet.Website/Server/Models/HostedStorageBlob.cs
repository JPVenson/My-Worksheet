using System;

namespace MyWorksheet.Website.Server.Models;

public partial class HostedStorageBlob
{
    public Guid HostedStorageBlobId { get; set; }

    public Guid Key { get; set; }

    public byte[] Value { get; set; }
}
