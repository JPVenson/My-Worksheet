using System.Collections.Generic;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class FullProjectDataReporting
{
    public ProjectReporting Project { get; set; }
    public List<FullProjectWorksheetDataReporting> Worksheets { get; set; }
}
