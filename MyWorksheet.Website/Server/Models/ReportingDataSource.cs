using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ReportingDataSource
{
    public Guid ReportingDataSourceId { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }
}
