using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Settings;

public class CdnCacheSettings
{
    public string Path { get; set; }
    public List<string> Enumeration { get; set; } = new();
}
