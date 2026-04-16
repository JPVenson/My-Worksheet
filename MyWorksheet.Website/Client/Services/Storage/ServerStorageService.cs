using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Storage;

[SingletonService()]
public class ServerStorageService : LazyLoadedService
{
    private readonly HttpService _httpService;

    public ServerStorageService(HttpService httpService)
    {
        _httpService = httpService;
        Providers = new FutureList<StorageProviderInfo>(async () => await httpService.StorageApiAccess.GetStorageProviderTypes());
        Providers.WhenLoaded(OnDataLoaded);
        ProviderSchema = new Dictionary<string, IObjectSchemaInfo>();
        PreviewTokensMap = new Dictionary<string, ResourceToken>();
    }

    public IFutureList<StorageProviderInfo> Providers { get; set; }
    public IDictionary<string, IObjectSchemaInfo> ProviderSchema { get; set; }

    public IDictionary<string, ResourceToken> PreviewTokensMap { get; set; }

    public async Task<IObjectSchemaInfo> GetSchema(string providerKey)
    {
        if (ProviderSchema.TryGetValue(providerKey, out var schema))
        {
            return schema;
        }

        var storageProviderDataStructure = (await _httpService.StorageApiAccess.GetStorageProviderDataStructure(providerKey)).UnpackOrThrow().Object;
        ProviderSchema[providerKey] = storageProviderDataStructure;
        return storageProviderDataStructure;
    }

    private const string _fallbackUrl = "";

    public string GetValidResourceThumbnailUrl(Guid storageEntryId, string size)
    {
        var token = GetToken(storageEntryId);
        if (token != null)
        {
            return _httpService.StorageApiAccess.ThumbnailUrl(storageEntryId, token, size);
        }

        return _fallbackUrl;
    }

    private string GetToken(Guid storageEntryId)
    {
        if (PreviewTokensMap.TryGetValue(storageEntryId.ToString(), out var token))
        {
            if (token.Token == null)
            {
                return null;
            }
            if (token.ValidUntil < DateTimeOffset.UtcNow)
            {
                token.Token = null;
                GenerateTokenFor(storageEntryId);
                return null;
            }

            return token.Token;
        }
        else
        {
            PreviewTokensMap[storageEntryId.ToString()] = new ResourceToken()
            {
                ValidUntil = DateTimeOffset.MinValue,
                Token = null,
                StorageId = storageEntryId
            };
            GenerateTokenFor(storageEntryId);
            return null;
        }
    }

    private void GenerateTokenFor(Guid storageEntryStorageEntryId)
    {
        _httpService.StorageApiAccess.CreateTokenForStorageEntry(storageEntryStorageEntryId)
            .AsTask()
            .ContinueWith(t =>
            {
                var previewTokens = PreviewTokensMap[storageEntryStorageEntryId.ToString()];
                previewTokens.Token = t.Result.Object;
                previewTokens.ValidUntil = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(1100);
                OnDataLoaded();
            });
    }
}

public class ResourceToken
{
    public Guid StorageId { get; set; }
    public string Token { get; set; }
    public DateTimeOffset ValidUntil { get; set; }
}