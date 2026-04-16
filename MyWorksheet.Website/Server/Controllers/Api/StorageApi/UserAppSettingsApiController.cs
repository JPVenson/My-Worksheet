using System;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Services.UserSettings;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MyWorksheet.Website.Server.Controllers.Api.StorageApi;

[Route("api/UserAppSettingsApi")]
public class UserAppSettingsApiController : ApiControllerBase
{
    private readonly IUserSettingsManager _settingsManager;

    public UserAppSettingsApiController(IUserSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    [Route("Update")]
    [RevokableAuthorize]
    [HttpPost]
    public IActionResult SetSettings(string name, [FromBody] JObject localSettings, string key = null)
    {
        if (!_settingsManager.SettingsTypes.ContainsKey(name))
        {
            return BadRequest("Invalid name");
        }

        var settingsManagerSettingsType = _settingsManager.SettingsTypes[name];
        var settingsObject = localSettings.ToObject(settingsManagerSettingsType);

        if (settingsObject == null)
        {
            return BadRequest("Invalid object");
        }
        //var bodyModelValidator = Configuration.Services.GetBodyModelValidator();
        //if (bodyModelValidator == null)
        //{
        //	return InternalServerError();
        //}
        //var metadataProvider = Configuration.Services.GetModelMetadataProvider();

        //if (!bodyModelValidator.Validate(settingsObject, settingsManagerSettingsType, metadataProvider, ActionContext, ""))
        //{
        //	return BadRequest();
        //}

        _settingsManager.UpdateSetting(settingsObject, name, key, User.GetUserId());
        return Data();
    }

    [Route("Get")]
    [RevokableAuthorize]
    [HttpGet]
    public IActionResult GetSettings(string name, string key = null)
    {
        return Data(_settingsManager.GetSettingFromStore(name, key, User?.GetUserId() ?? Guid.Empty));
    }
}