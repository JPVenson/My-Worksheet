using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.CdnFallback;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Morestachio;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Controllers.Api.Reporting;

[Route("api/ReportManagementApi")]
[RevokableAuthorize]
public class TemplateReportingApiController : ApiControllerBase
{
    private readonly IMapperService _mapperService;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly ILogger<TemplateReportingApiController> _logger;
    private readonly ICdnFallbackService _cdnFallbackService;
    private readonly ILocalFileProvider _localFileProvider;
    private readonly ITextTemplateManager _textTemplateManager;

    public TemplateReportingApiController(IMapperService mapperService,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        ILogger<TemplateReportingApiController> logger,
        ICdnFallbackService cdnFallbackService,
        ILocalFileProvider localFileProvider,
        ITextTemplateManager textTemplateManager)
    {
        _mapperService = mapperService;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _cdnFallbackService = cdnFallbackService;
        _localFileProvider = localFileProvider;
        _textTemplateManager = textTemplateManager;
    }

    [HttpGet]
    [Route("GetJavascriptBaseLib")]
    public async Task<IActionResult> GetJavascriptBaseLib()
    {
        var textPath = _cdnFallbackService.TextPaths["es5.ts"];
        var localPath = _cdnFallbackService.ConvertToLocalPath(textPath);
        return Data((await _localFileProvider.ReadAllAsync(localPath)).Stringify(true, Encoding.UTF8));
    }

    [HttpGet]
    [Route("GetTemplateFormatter")]
    public IActionResult GetTemplateFormatter()
    {
        return Ok(_textTemplateManager
            .KnownTextFormatter
            .ToDictionary(e => e.Key, e => e.Value.Key)
            .ToArray());
    }

    [HttpGet]
    [Route("TokenizeTemplate")]
    public IActionResult TokenizeTemplate(string template)
    {
        var morestachioDocumentInfo = Parser.ParseWithOptions(ParserOptionsBuilder.New()
            .WithTemplate(template)
            .Build());
        return Ok(new MorestachioParsedTemplateViewModel()
        {
            Document = morestachioDocumentInfo.Document,
            Errors = morestachioDocumentInfo.Errors
        });
    }

    [HttpGet]
    [Route("GetFormatterForUser")]
    public IActionResult GetFormatter()
    {
        //var mustachioFormatterModels = IoC.Resolve<MustachioFormatterService>()
        //	.GetFormatterForUser(User.Identity.GetUserId<int>());
        return Ok(_mapperService.ViewModelMapper.Map<MustachioFormatterViewModel[]>(null));
    }

    [HttpGet]
    [Route("GetSchemaForRds")]
    public IActionResult GetSchema(string dataSource)
    {
        var rds = ReportDataSources.Yield().FirstOrDefault(f => f.Key == dataSource);
        if (rds == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        using var db = _dbContextFactory.CreateDbContext();
        var scheduleReportSchemaModel = new ScheduleReportSchemaModel()
        {
            QuerySchema = rds.QuerySchema() as JsonSchema,
            ArgumentSchema = rds.ArgumentSchema(db, User.GetUserId()) as JsonSchema
        };
        return Ok(scheduleReportSchemaModel);
    }

    [HttpGet]
    [Route("GetEngineAddons")]
    public IActionResult GetReportingEngineAddons(string reportingEngine)
    {
        var firstOrDefault = _textTemplateManager.KnownTextFormatter.FirstOrDefault(e => e.Key == reportingEngine);

        if (firstOrDefault.Value == null)
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Ok(firstOrDefault.Value.GetFrameworkAddons(_dbContextFactory, User.GetUserId()));
    }

    [HttpGet]
    [Route("Rds")]
    public IActionResult GetRds()
    {
        var rds = _mapperService.ViewModelMapper.Map<ReportingDataSourceViewModel[]>(ReportDataSources.Yield().Select(f => f));
        return Ok(rds);
    }

    [HttpGet]
    [Route("RdsArguments")]
    public IActionResult GetRdsArguments(string key)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Ok(_mapperService.ViewModelMapper.Map<NEngineParameterInfo[]>(ReportDataSources.Yield().FirstOrDefault(e => e.Key == key)?.GetParameterInfos(db, User.GetUserId())));
    }

    [HttpGet]
    [Route("RdsOperators")]
    public IActionResult GetOperators()
    {
        return Ok(ReportingOperators.Enumerate());
    }

    [HttpPost]
    [Route("ScheduleReport")]
    public async Task<IActionResult> RunReport([FromBody] ScheduleReportModel model)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var run = await _textTemplateManager.RunReport(db, model, User.GetUserId());
        if (!run.Success)
        {
            return BadRequest(run.Error);
        }
        return Ok(run.Object);
    }
}