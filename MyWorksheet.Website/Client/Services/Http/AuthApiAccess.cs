using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.Login;

namespace MyWorksheet.Website.Client.Services.Http;

public class AuthApiAccess : HttpAccessBase
{
    public AuthApiAccess(HttpService httpService)
        : base(httpService, "AuthorizeApi")
    {
    }

    public async ValueTask<ApiResult<LoginResult>> Login(string username, string password, string recaptcha)
    {
        SkipUnauthorisedCheck = true;
        var challange = await Post<ChallangeRequestBody, string>(BuildApi("Challange"), new ChallangeRequestBody()
        {
            Username = username,
            Recaptcha = recaptcha
        });

        if (!challange.Success)
        {
            return new ApiResult<LoginResult>(challange.StatusCode, false, challange.StatusMessage, challange.Exception);
        }

        var passwordHash = ChallangeUtil.HashPassword(password, username);
        var response = ChallangeUtil.ShiftString(challange.Object, passwordHash.ToHexDecByHexDec());

        SkipUnauthorisedCheck = true;
        var logon = await Post<LoginFormModel, LoginResult>(BuildApi("Login"), new LoginFormModel()
        {
            Account = username,
            Password = response,
            RedictUrl = ""
        });
        return logon;
    }
}