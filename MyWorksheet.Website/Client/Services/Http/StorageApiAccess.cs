using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Pages.Shared;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;

namespace MyWorksheet.Website.Client.Services.Http;

public class StorageApiAccess : RestHttpAccessBase<GetStorageProvider, CreateStorageProvider>
{
    public StorageApiAccess(HttpService httpService)
        : base(httpService, "StorageApi")
    {
    }


    public ValueTask<ApiResult<string>> CreateTokenForStorageEntry(Guid storageId)
    {
        return Get<string>(BuildApi("CreateTokenForStorageEntry", new
        {
            storageEntryId = storageId
        }));
    }

    public ValueTask<ApiResult<StorageProviderInfo[]>> GetStorageProviderTypes()
    {
        return Get<StorageProviderInfo[]>(BuildApi("GetStorageProviderTypes"));
    }

    public ValueTask<ApiResult<GetStorageProvider[]>> GetStorageProviders()
    {
        return Get<GetStorageProvider[]>(BuildApi("GetStorageProviders"));
    }

    public ValueTask<ApiResult<GetStorageProvider>> GetStorageProvider(Guid id)
    {
        return Get<GetStorageProvider>(BuildApi("GetStorageProvider", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<StorageProviderStatistics>> GetStorageProviderStatistics()
    {
        return Get<StorageProviderStatistics>(BuildApi("GetStorageProviderStatistics"));
    }

    public ValueTask<ApiResult<StorageTypeViewModel>> GetStorageTypes()
    {
        return Get<StorageTypeViewModel>(BuildApi("GetStorageTypes"));
    }

    public ValueTask<ApiResult<IDictionary<string, object>>> GetStorageProviderData(Guid storageId)
    {
        return Get<IDictionary<string, object>>(BuildApi("GetStorageProviderData", new
        {
            storageId
        }));
    }

    public ValueTask<ApiResult<JsonSchema>> GetStorageProviderDataStructure(string storageKey)
    {
        return Get<JsonSchema>(BuildApi("GetStorageProviderDataStructure", new
        {
            storageKey
        }));
    }

    public ValueTask<ApiResult<PageResultSet<StorageEntryViewModel>>> GetStorageEntries(Guid storageProviderId, int page, int pageSize,
        bool? showDeleted = null,
        string searchText = null,
        int? ofType = null)
    {
        return Get<PageResultSet<StorageEntryViewModel>>(BuildApi("GetStorageEntries", new
        {
            storageProviderId,
            page,
            pageSize,
            showDeleted,
            searchText,
            ofType
        }));
    }

    public ValueTask<ApiResult<IEnumerable<string>>> TestProvider(string storageKey, IDictionary<string, object> fields)
    {
        return Post<IDictionary<string, object>, IEnumerable<string>>(BuildApi("TestProvider", new
        {
            storageKey
        }), fields);
    }

    public ValueTask<ApiResult<IObjectSchemaInfo>> GetStorageProviderDataStructure()
    {
        return Get<IObjectSchemaInfo>(BuildApi("GetStorageProviderDataStructure"));
    }

    public string DownloadUrl(Guid storageId, string token)
    {
        return BuildApi("Download", new
        {
            token,
            storageEntryId = storageId
        });
    }

    public string ThumbnailUrl(Guid storageId, string token, string size)
    {
        return BuildApi("GetThumbnail", new
        {
            token,
            size,
            storageId = storageId,
            //rand = DateTimeOffset.UtcNow.Ticks
        });
    }
}