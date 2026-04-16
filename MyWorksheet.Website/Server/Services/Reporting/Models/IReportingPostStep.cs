using System.IO;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public interface IReportingPostStep
{
    string Name { get; set; }
    string Key { get; set; }

    Stream Process(Stream input);
}