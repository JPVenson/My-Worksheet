using System;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;

namespace MyWorksheet.Website.Server.Services.Reporting.Text;

[Serializable]
public class TextTemplateDataQuery
{
    public SerializableObjectDictionary<string, object> Values { get; set; }
    public string GeneratedQuery { get; set; }
    public string DataSourceName { get; set; }
    public string[] FollowUpAction { get; set; } = new string[0];
}