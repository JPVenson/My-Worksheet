using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class ReportingDataStructureBase<T>
{
    [JsonComment("Reporting/DataTemplate.Comment.DataSource")]
    public T DataSource { get; set; }
}