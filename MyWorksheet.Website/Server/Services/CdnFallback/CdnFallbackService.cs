using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.CdnFallback;

[SingletonService(typeof(ICdnFallbackService))]
public class CdnFallbackService : ICdnFallbackService
{
    private readonly IOptions<CdnSettings> _cdnSettings;
    private readonly IAppLogger _appLogger;
    private readonly ILocalFileProvider _localFileProvider;

    public CdnFallbackService(IOptions<CdnSettings> cdnSettings,
        IAppLogger appLogger,
        ILocalFileProvider localFileProvider)
    {
        _cdnSettings = cdnSettings;
        _appLogger = appLogger;
        _localFileProvider = localFileProvider;
        CdnResolvedPaths = new ConcurrentDictionary<string, string>();
        TextPaths = new ConcurrentDictionary<string, string>();
    }

    public IDictionary<string, string> CdnResolvedPaths { get; set; }
    public IDictionary<string, string> TextPaths { get; set; }

    private Task _createLookups;

    private string _requireJsPaths;

    public async Task<string> GetRequireJsPaths()
    {
        return _requireJsPaths ?? (_requireJsPaths = await CreateRequireJsPaths());
    }

    private async Task<string> CreateRequireJsPaths()
    {
        if (_createLookups == null)
        {
            return "";
        }
        await _createLookups;
        var sb = new StringBuilder();

        foreach (var cdnResolvedPath in CdnResolvedPaths)
        {
            var cdnFallbackPath = ConvertToLocalPath(cdnResolvedPath.Value);
            sb.AppendLine($@"""{cdnResolvedPath.Key}"": [""{cdnResolvedPath.Value}"",""{cdnFallbackPath}""],");
        }

        foreach (var cdnResolvedPath in TextPaths)
        {
            var cdnFallbackPath = ConvertToLocalPath(cdnResolvedPath.Value);
            sb.AppendLine($@"""{cdnResolvedPath.Key}"": [""{cdnResolvedPath.Value}"",""{cdnFallbackPath}""],");
        }

        return sb.ToString();
    }

    public string ConvertToLocalPath(string cdnFile)
    {
        var lookupPath = _cdnSettings.Value.Cache.Path;

        var fileUri = new Uri(cdnFile.Replace("!text", ""));
        var filePath = fileUri.LocalPath;

        //if (!filePath.EndsWith(".js"))
        //{
        //	filePath = filePath + ".js";
        //}
        var targetFileName = filePath.Replace("/", "_");
        return Path.Combine(lookupPath, targetFileName);
    }

    public Task LookupCdnPaths()
    {

        //CdnResolvedPaths.Add("resizeSensor", "/scripts/lib/ResizeSensor/ResizeSensor");

        return _createLookups = Task.Run(async () =>
        {
            TextPaths.AsParallel().ForAll(async e =>
            {
                _appLogger.LogInformation($"Check CDN Text fallback for '{e.Key}'");
                var localFile = ConvertToLocalPath(e.Value);
                var fileUri = new Uri(e.Value);

                if (_localFileProvider.Exists(localFile))
                {
                    _appLogger.LogInformation($"CDN fallback for '{e.Key}' exists");
                    return;
                }

                _appLogger.LogInformation($"CDN fallback load '{e.Key}'");
                var httpClient = new HttpClient();

                var toLookupPath = fileUri.OriginalString;
                try
                {
                    var cdnFallbackContent = await httpClient.GetByteArrayAsync(toLookupPath);
                    await _localFileProvider.WriteAll(localFile, cdnFallbackContent);
                    _appLogger.LogInformation($"CDN fallback load '{e.Key}' done");
                }
                catch (Exception exception)
                {
                    _appLogger.LogError($"Cdn lookup for {e.Key} failed", null, new Dictionary<string, string>()
                    {
                        {"Exception", JsonConvert.SerializeObject(exception) }
                    });
                }
            });

            CdnResolvedPaths.AsParallel().ForAll(async e =>
            {
                _appLogger.LogInformation($"Check CDN fallback for '{e.Key}'");

                var localFile = ConvertToLocalPath(e.Value);
                if (localFile.EndsWith(".js"))
                {
                    localFile = localFile + ".js";
                }
                var fileUri = new Uri(e.Value);

                if (_localFileProvider.Exists(localFile))
                {
                    _appLogger.LogInformation($"CDN fallback for '{e.Key}' exists");
                    return;
                }

                _appLogger.LogInformation($"CDN fallback load '{e.Key}'");
                var httpClient = new HttpClient();

                var toLookupPath = fileUri.OriginalString;

                if (!toLookupPath.EndsWith(".js") && !toLookupPath.EndsWith("!text"))
                {
                    toLookupPath = toLookupPath + ".js";
                }

                toLookupPath = toLookupPath.Replace("!text", "");

                try
                {
                    var cdnFallbackContent = await httpClient.GetByteArrayAsync(toLookupPath);
                    await _localFileProvider.WriteAll(localFile, cdnFallbackContent);
                    _appLogger.LogInformation($"CDN fallback load '{e.Key}' done");
                }
                catch (Exception exception)
                {
                    _appLogger.LogError($"Cdn lookup for {e.Key} failed", null, new Dictionary<string, string>()
                    {
                        {"Exception", JsonConvert.SerializeObject(exception) }
                    });
                }
            });
        });
    }
}