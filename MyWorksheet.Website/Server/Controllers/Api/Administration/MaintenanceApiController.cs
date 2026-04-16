using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.Maintainace;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[Route("api/MaintenanceApi")]
public class MaintenanceApiController : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _contextFactory;
    private readonly IMapperService _mapperService;
    private readonly ILocalFileProvider _fileProvider;
    private readonly ITemplateEngine _templateEngine;
    private readonly IMaintenanceService _maintenanceService;
    private readonly IOptions<MaintenanceAppSettings> _maintenanceSettings;

    public MaintenanceApiController(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapperService, ILocalFileProvider fileProvider, ITemplateEngine templateEngine, IMaintenanceService maintenanceService,
        IOptions<MaintenanceAppSettings> maintenanceSettings)
    {
        _contextFactory = dbContextFactory;
        _mapperService = mapperService;
        _fileProvider = fileProvider;
        _templateEngine = templateEngine;
        _maintenanceService = maintenanceService;
        _maintenanceSettings = maintenanceSettings;
    }

    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/Maintaince")]
    public IActionResult GetMaintance()
    {
        return Data(_maintenanceService.CurrentMode);
    }

    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [Route("Admin/UpdateMaintaince")]
    public async Task<IActionResult> UpdateMaintance([FromBody] MaintainceModeUpdateViewModel modeUpdateViewModel)
    {
        using var db = _contextFactory.CreateDbContext();
        if (modeUpdateViewModel == null)
        {
            db.Maintainaces.ExecuteUpdate(e => e.SetProperty(f => f.Completed, true));
            db.SaveChanges();
            return Data();
        }

        modeUpdateViewModel.From = modeUpdateViewModel.From.ToUniversalTime();
        modeUpdateViewModel.Until = modeUpdateViewModel.Until.ToUniversalTime();
        modeUpdateViewModel.CallerIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();

        var maintainace = _mapperService.ViewModelMapper.Map<MaintainceModeUpdateViewModel, Models.Maintainace>(modeUpdateViewModel);
        var templateUrl = "";

        if (!modeUpdateViewModel.Scheduled)
        {
            templateUrl =
                _maintenanceSettings.Value.Templates.Scheduled;
        }
        else
        {
            templateUrl =
                _maintenanceSettings.Value.Templates.UnScheduled;
        }

        var templateContent = await _fileProvider.ReadAllAsync(templateUrl);
        var generateTemplate = _templateEngine.GenerateTemplate(templateContent.Stringify(true, Encoding.UTF8),
            User.GetUserId());

        var appOfflineMessage = await generateTemplate.RenderTemplateAsync(new Dictionary<string, object>()
        {
            {"Maintainace", modeUpdateViewModel}
        }, null);
        maintainace.CompiledView = appOfflineMessage;

        db.Add(maintainace);
        db.SaveChanges();
        _maintenanceService.CurrentMode = maintainace;
        return Data(_maintenanceService.CurrentMode);
    }
}