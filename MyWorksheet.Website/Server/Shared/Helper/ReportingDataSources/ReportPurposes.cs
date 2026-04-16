using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public static class ReportPurposes
{
    public static ReportPurpose General { get; private set; } = new ReportPurpose(null, "General", "The general Category for Reports");
    public static ReportPurpose Worksheet { get; private set; } = new ReportPurpose("Worksheet", "Worksheet Template", "Category for Worksheets");

    public static IEnumerable<ReportPurpose> Yield()
    {
        yield return General;
        yield return Worksheet;
    }
}

public class ReportPurpose
{
    public ReportPurpose(string key, string name, string description)
    {
        Key = key;
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Key { get; private set; }
    public string Description { get; private set; }
}