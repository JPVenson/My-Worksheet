using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Text;

namespace MyWorksheet.Website.Client.Services.Http;

public class TextApiAccess : HttpAccessBase
{
    public TextApiAccess(HttpService httpService)
        : base(httpService, "TextApi")
    {
    }

    public ValueTask<ApiResult<TextResourceViewModel[]>> GetTranslationsInGroup(string key, string culture)
    {
        return base.Get<TextResourceViewModel[]>(BuildApi("GetForPage", new
        {
            groupName = key,
            locale = culture
        }));
    }

    public ValueTask<ApiResult<UiResourceState[]>> GetState()
    {
        return Get<UiResourceState[]>(BuildApi("GetCacheStatus"));
    }

    public ValueTask<ApiResult<TextResourceViewModel[]>> GetAll(string culture)
    {
        return Get<TextResourceViewModel[]>(BuildApi("GetAll", new
        {
            culture
        }));
    }
}