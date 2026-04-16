using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

namespace MyWorksheet.Website.Client.Services.Http;

public class RegionApiAccess : RestHttpAccessBase<FeatureRegionViewModel>
{
    public RegionApiAccess(HttpService httpService) : base(httpService, "RegionApi")
    {

    }

    public ValueTask<ApiResult<CurrencyLookup[]>> GetStatisticsProvider(string filter)
    {
        return Get<CurrencyLookup[]>(BuildApi("AllCurrencys", new
        {
            filter
        }));
    }

    public ValueTask<ApiResult<FeatureRegionViewModel>> GetUsersRegion()
    {
        return Get<FeatureRegionViewModel>(BuildApi("GetUsersRegion"));
    }

    public ValueTask<ApiResult<FeatureRegionViewModel>> GetAllRegion()
    {
        return Get<FeatureRegionViewModel>(BuildApi("All"));
    }
    public ValueTask<ApiResult<FeatureRegionViewModel>> GetRegion(Guid regionId)
    {
        return Get<FeatureRegionViewModel>(BuildApi("Single"));
    }
}
