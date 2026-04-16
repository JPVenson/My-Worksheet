using System;

namespace MyWorksheet.Website.Server.Models;

public partial class Maintainace
{
    public Guid MaintainaceId { get; set; }

    public string Reason { get; set; }

    public DateTime From { get; set; }

    public DateTime Until { get; set; }

    public string CallerIp { get; set; }

    public string CompiledView { get; set; }

    public bool Completed { get; set; }
}
