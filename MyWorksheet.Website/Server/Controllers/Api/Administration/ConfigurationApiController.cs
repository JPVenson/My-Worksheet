using System;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.ServerManager;
using MyWorksheet.Website.Server.Services.ServerSettings;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

public static class StringExtensions
{
    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source?.IndexOf(toCheck, comp) >= 0;
    }
}

[ApiController]
[Route("api/ConfigurationApi")]
[RevokableAuthorize(Roles = Roles.AdminRoleName)]
public class ConfigurationApiControllerBase : ApiControllerBase
{
    private readonly IServerSettingsService _settingsService;
    private readonly IMapperService _mapper;
    private readonly IServerManagerService _serverManager;

    public ConfigurationApiControllerBase(IServerSettingsService settingsService, IMapperService mapper, IServerManagerService serverManager)
    {
        _settingsService = settingsService;
        _mapper = mapper;
        _serverManager = serverManager;
    }

    [Route("KnownProcessors")]
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetKnownProcessors()
    {
        var serverManagerService = _serverManager;
        return Data(_mapper.ViewModelMapper.Map<KnownServerViewModel[]>(serverManagerService.GetKnownServers()));
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetDelimiter()
    {
        return Data(_settingsService.Delimiter);
    }

    [Route("Get")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAll()
    {
        return Data(_mapper.ViewModelMapper.Map<ConfigurationModelGet>(_settingsService.Root));
    }

    [Route("Flat")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAllFlat(string search)
    {
        return Data(_mapper.ViewModelMapper.Map<ConfigurationModelGetFlat[]>(_settingsService.Root.CompileFlat()
            .Where(e => e.Value != null && ((e.Value == null && search == "NULL") ||
                                            e.Key.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                            e.Value.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)))));
    }


    [Route("Search")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult Search(string search)
    {
        var keyValuePairs = _settingsService.Root.CompileFlat()
            .Where(e => e.Value != null && ((e.Value == null && search == "NULL") ||
                                            e.Key.Contains(search) ||
                                            e.Value.ToString().Contains(search)));

        var searchResult = keyValuePairs.Select(e =>
            _settingsService.GetSettingObject(e.Key));
        var mapResult = _mapper.ViewModelMapper
            .Map<ConfigurationSettingModelGet[]>(searchResult).ToArray();
        return Data(mapResult);
    }

    [Route("Update")]
    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult Update(string path, string value)
    {
        var serverSettingsModel = _settingsService.GetSettingObject(path);
        if (serverSettingsModel == null)
        {
            return BadRequest("Invalid Path");
        }

        if (serverSettingsModel.ReadOnly)
        {
            return BadRequest("ReadOnly");
        }

        serverSettingsModel.Value = value;
        return Data();
    }
}