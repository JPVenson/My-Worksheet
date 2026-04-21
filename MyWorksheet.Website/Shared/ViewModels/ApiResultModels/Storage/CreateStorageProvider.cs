using System.Collections.Generic;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;

public class CreateStorageProvider : ViewModelBase
{
    private IDictionary<string, object> _fields;

    private GetStorageProvider _providerInfo;

    public GetStorageProvider ProviderInfo
    {
        get { return _providerInfo; }
        set { SetProperty(ref _providerInfo, value); }
    }

    public IDictionary<string, object> Fields
    {
        get { return _fields; }
        set { SetProperty(ref _fields, value); }
    }
}