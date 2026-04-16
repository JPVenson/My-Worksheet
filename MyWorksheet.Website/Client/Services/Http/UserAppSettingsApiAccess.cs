using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;

namespace MyWorksheet.Website.Client.Services.Http;

public class UserAppSettingsApiAccess : HttpAccessBase
{
    public UserAppSettingsApiAccess(HttpService httpService) : base(httpService, "UserAppSettingsApi")
    {
    }

    public ValueTask<ApiResult<object>> GetSetting(string name, string key)
    {
        return Get<object>(BuildApi("Get", new
        {
            name,
            key
        }));
    }

    public ValueTask<ApiResult> UpdateSetting(string name, string key, object value)
    {
        return Post(BuildApi("Update", new
        {
            name,
            key,
        }), value);
    }
}