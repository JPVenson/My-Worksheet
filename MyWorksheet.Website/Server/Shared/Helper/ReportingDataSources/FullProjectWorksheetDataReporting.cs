using System.Collections.Generic;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class FullProjectWorksheetDataReporting
{
    public WorksheetReporting Worksheet { get; set; }
    public List<WorksheetItemReporting> WorksheetItems { get; set; }
}
