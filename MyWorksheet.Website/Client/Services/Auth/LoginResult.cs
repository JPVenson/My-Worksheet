using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;

namespace MyWorksheet.Website.Client.Services.Auth;

public class LoginResult
{
    public string Token { get; set; }
    public AccountApiUserGet UserData { get; set; }
}