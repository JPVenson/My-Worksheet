using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("api/CdnFallbackApi")]
public class CdnFallbackApiControllerBase : ApiControllerBase
{
    private readonly ILocalFileProvider _fileProvider;
    private readonly IOptions<CdnSettings> _cdnSettings;

    public CdnFallbackApiControllerBase(ILocalFileProvider fileProvider, IOptions<CdnSettings> cdnSettings)
    {
        _fileProvider = fileProvider;
        _cdnSettings = cdnSettings;
    }

    [HttpGet]
    [Route("CdnFallback")]
    public async Task<HttpResponseMessage> LoadCdnFallback(string url)
    {
        var cdn = _cdnSettings.Value.Cache.Enumeration.FirstOrDefault(e => e.StartsWith(url));
        if (cdn == null)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
        var targetPath =
            new string(url.Select(f => (Path.GetInvalidFileNameChars().Contains(f) ? ';' : f)).ToArray());
        var path = string.Format(_cdnSettings.Value.Cache.Path, cdn, targetPath);
        if (_fileProvider.Exists(path))
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent((await _fileProvider.ReadAllAsync(path))),
                StatusCode = HttpStatusCode.OK,
            };
        }
        return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }
}