using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class ReportingDataStructureBase<TVm>
{
    [JsonComment("Reporting/DataTemplate.Comment.DataSource")]
    public TVm DataSource { get; set; }
}