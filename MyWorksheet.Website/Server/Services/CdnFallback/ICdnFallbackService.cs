using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services.CdnFallback;

public interface ICdnFallbackService
{
    IDictionary<string, string> CdnResolvedPaths { get; set; }
    IDictionary<string, string> TextPaths { get; set; }

    Task<string> GetRequireJsPaths();
    Task LookupCdnPaths();
    string ConvertToLocalPath(string cdnFile);
}