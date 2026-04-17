using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Shared.Helper;
using MyWorksheet.Shared.Services.PriorityQueue;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using MyWorksheet.Website.Server.Services.ServerManager;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.ReportService.Services.Templating.Text;

[SingletonService(typeof(ITextTemplateManager), typeof(IReportCapability))]
public class TextTemplateManager : ITextTemplateManager
{
    private readonly IServerPriorityQueueManager _mgr;
    private readonly ActivatorService _activatorService;
    private readonly ILogger<TextTemplateManager> _appLogger;
    private readonly IBlobManagerService _blobManagerService;
    private readonly IUserQuotaService _userQuotaService;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IServerManagerService _serverManagerService;

    public TextTemplateManager(IServerPriorityQueueManager mgr,
        ActivatorService activatorService,
        ILogger<TextTemplateManager> appLogger,
        IBlobManagerService blobManagerService,
        IUserQuotaService userQuotaService,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IServerManagerService serverManagerService)
    {
        _mgr = mgr;
        _activatorService = activatorService;
        _appLogger = appLogger;
        _blobManagerService = blobManagerService;
        _userQuotaService = userQuotaService;
        _dbContextFactory = dbContextFactory;
        _serverManagerService = serverManagerService;
        RunningTemplates = new ConcurrentHashSet<PositionalTemplate>();
        KnownTextFormatter = new Dictionary<string, ITemplateEngine>
        {
            { MorestachioTemplateEngineService.PrgKey, _activatorService.ActivateType<MorestachioTemplateEngineService>() },
            { JavascriptTemplateEngineService.PrgKey, _activatorService.ActivateType<JavascriptTemplateEngineService>() }
        };
    }

    public ConcurrentHashSet<PositionalTemplate> RunningTemplates { get; set; }

    public ProcessorCapability[] ReportCapabilities()
    {
        var capabilities = new List<ProcessorCapability>
        {
            new ProcessorCapability
            {
                Name = "Feature_Reporting_Engines_Morestachio",
                IsEnabled = true,
                Value = "true"
            },
            new ProcessorCapability
            {
                Name = "Feature_Reporting_Engines_javascript",
                IsEnabled = true,
                Value = "true"
            }
        };

        foreach (var reportingDataSource in ReportDataSources.Yield())
        {
            capabilities.Add(new ProcessorCapability
            {
                Name = "Feature_Reporting_DataSource_" + reportingDataSource.Key,
                Value = reportingDataSource.Id.ToString(),
                IsEnabled = true
            });
        }

        return capabilities.ToArray();
    }

    public IDictionary<string, ITemplateEngine> KnownTextFormatter { get; set; }

    public event OnReportPositionChanged ReportPositionChanged;

    public int GetPositionOfTemplate(Guid templateId, Guid creatorId)
    {
        var template =
            RunningTemplates.FirstOrDefault(e => e.TemplateId == templateId && e.ExecutingUser == creatorId);
        return template?.Position ?? 0;
    }

    public async Task<StandardOperationResultBase<Guid>> RunReport(MyworksheetContext db,
        ScheduleReportModel model, Guid appUserId)
    {
        var dataSourceElements = new Dictionary<string, object>();
        var prepaireReporting = PrepareReport(db, model, appUserId, dataSourceElements);
        if (!prepaireReporting.Success)
        {
            return new StandardOperationResultBase<Guid>(prepaireReporting.Error);
        }

        var findTemplate = prepaireReporting.Object;

        var run = await GenerateTemplate(findTemplate.NengineTemplateId, findTemplate.Template,
            new TextTemplateDataQuery
            {
                Values = new SerializableObjectDictionary<string, object>(dataSourceElements),
                DataSourceName = findTemplate.UsedDataSource,
                GeneratedQuery = DataSourceHelper.CreateQueryFromArguments(model)
            }, PriorityManagerLevel.FireAndForget, appUserId, model.StorageProvider.Value, model.Preview);
        return run;
    }

    public StandardOperationResultBase<NengineTemplate> PrepareReport(MyworksheetContext db,
        ScheduleReportModel model,
        Guid appUserId,
        IDictionary<string, object> dataSourceElements)
    {
        if (model.Arguments == null || model.ParameterValues == null)
        {
            return new StandardOperationResultBase<NengineTemplate>("Invalid Id");
        }

        var findTemplate = db.NengineTemplates.Find(model.TemplateId);
        if (findTemplate.IdCreator.HasValue && findTemplate.IdCreator.Value != appUserId)
        {
            return new StandardOperationResultBase<NengineTemplate>("Invalid Id");
        }

        if (!model.StorageProvider.HasValue)
        {
            model.StorageProvider = db.StorageProviders
                .Where(f => (f.IdAppUser == appUserId && f.IsDefaultProvider) || (f.IdAppUser == null && f.IsDefaultProvider))
                .OrderBy(e => e.IdAppUser != null)
                .First()
                .StorageProviderId;
        }

        try
        {
            var source = ReportDataSources.Yield().FirstOrDefault(e => e.Key == findTemplate.UsedDataSource);
            if (source == null)
            {
                return new StandardOperationResultBase<NengineTemplate>("Invalid Datasource");
            }

            var compairsonResults = DataSourceHelper.CompareIncommingArguments(
                source.GetParameterInfos(db, appUserId),
                model.ParameterValues).ToArray();
            if (compairsonResults.Any())
            {
                return new StandardOperationResultBase<NengineTemplate>(
                    "Invalid Arguments: \r\n" + compairsonResults
                        .Select(f => f.Name + " | " + f.Text)
                        .Aggregate((e, f) => e + "\r\n" + f));
            }

            var dataSource = source.GetData(db, appUserId, model.ParameterValues, model.Arguments);
            if (dataSource == null)
            {
                return new StandardOperationResultBase<NengineTemplate>(
                    "There was an Internal error while loading the data");
            }

            foreach (var o in dataSource)
            {
                dataSourceElements.Add(o);
            }

            if (model.StorageProvider == Guid.Empty || model.StorageProvider == null)
            {
                var storageProviders = db.StorageProviders
                    .Where(e => (e.IdAppUser == appUserId && e.IsDefaultProvider == true) || e.IdAppUser == null)
                    .ToArray();
                model.StorageProvider = storageProviders.FirstOrDefault(e => e.IdAppUser == appUserId)?.StorageProviderId
                                        ?? storageProviders.First(e => e.IdAppUser is null).StorageProviderId;
            }
        }
        catch (Exception e)
        {
            _appLogger.LogWarning($"Invalid Query detected for user '{appUserId}'",
                LoggerCategories.Reporting.ToString(), new Dictionary<string, string>
                {
                    {"Arguments", JsonConvert.SerializeObject(model)},
                    {"Exception", JsonConvert.SerializeObject(e)}
                });
            return new StandardOperationResultBase<NengineTemplate>("Something Unexpected happend.");
        }

        return new StandardOperationResultBase<NengineTemplate>(findTemplate);
    }

    public async Task<StandardOperationResultBase<Guid>> GenerateTemplate(Guid templateId,
        string template,
        TextTemplateDataQuery values,
        PriorityManagerLevel level,
        Guid creatorId,
        Guid storageProviderId,
        bool preview,
        IDictionary<string, object> additonalReportInfos = null)
    {
        var db = _dbContextFactory.CreateDbContext();
        if (!(await _userQuotaService.Add(creatorId, 1, Quotas.ConurrentReports)))
        {
            return new StandardOperationResultBase<Guid>(
                "There are currently running more reports or awaiting scheduling then allowed for your account");
        }

        try
        {
            if (preview)
            {
                var lastPreviews = db.NengineRunningTasks
                    .Where(f => f.IdNengineTemplate == templateId)
                    .Where(f => f.IsPreview == true)
                    .ToArray();
                foreach (var lastPreview in lastPreviews)
                {
                    db.NengineRunningTasks.Where(e => e.NengineRunningTaskId == lastPreview.NengineRunningTaskId).ExecuteDelete();
                    if (lastPreview != null && lastPreview.IdStoreageEntry.HasValue)
                    {
                        await _blobManagerService.DeleteData(lastPreview.IdStoreageEntry.Value, creatorId);
                        db.StorageEntries.Where(e => e.StorageEntryId == lastPreview.IdStoreageEntry.Value).ExecuteDelete();
                    }
                }
            }

            var nEngineRunningTask = new NengineRunningTask
            {
                IdNengineTemplate = templateId,
                IsDone = false,
                DateCreated = DateTime.UtcNow,
                IsFaulted = false,
                ArgumentsRepresentation = values.GeneratedQuery,
                IdCreator = creatorId,
                IsPreview = preview,
                IdProcessor = _serverManagerService.Self.InstanceId
            };
            db.Add(nEngineRunningTask);
            db.SaveChanges();

            if (!RunningTemplates.TryAdd(new PositionalTemplate(RunningTemplates.Count, creatorId, templateId)))
            {
                return new StandardOperationResultBase<Guid>("Internal Error. Could not enqueue the Report");
            }

            OnReportPositionChanged(templateId, RunningTemplates.Count, creatorId);
            var queueData = new Dictionary<string, object>
            {
                {"templateId", templateId},
                {"TextTemplateDataQuery", values},
                {"storageProviderId", storageProviderId},
                {"NEngineRunningTaskId", nEngineRunningTask.NengineRunningTaskId},
                {"preview", preview}
            };

            additonalReportInfos = additonalReportInfos ?? new Dictionary<string, object>();
            foreach (var o in queueData)
            {
                additonalReportInfos.Add(o);
            }

            var queueItem = new PriorityQueueElement(level, creatorId, ExternalSchedulableTasks.CREATE_REPORT,
                additonalReportInfos,
                values.FollowUpAction);
            _mgr.Enqueue(queueItem).AttachAsyncHandler();

            return new StandardOperationResultBase<Guid>(nEngineRunningTask.NengineRunningTaskId);
        }
        catch
        {
            await _userQuotaService.Subtract(creatorId, 1, Quotas.ConurrentReports);
            throw;
        }
    }

    public void OnTemplateProcessed(Guid templateId)
    {
        PositionalTemplate waiter;

        if ((waiter = RunningTemplates.Get(e => e.TemplateId == templateId)) != null)
        {
            RunningTemplates.Remove(waiter);
            OnReportPositionChanged(templateId, -1, waiter.ExecutingUser);
            foreach (var positionalTemplate in RunningTemplates.Select((item, index) => new
            {
                index,
                item
            }))
            {
                positionalTemplate.item.Position = positionalTemplate.index + 1;
                OnReportPositionChanged(positionalTemplate.item.TemplateId, positionalTemplate.item.Position,
                    positionalTemplate.item.ExecutingUser);
            }
        }
    }

    protected virtual void OnReportPositionChanged(Guid reportId, int newPosition, Guid executingUser)
    {
        ReportPositionChanged?.Invoke(reportId, newPosition, executingUser);
    }

    public class PositionalTemplate
    {
        public PositionalTemplate(int position, Guid executingUser, Guid templateId)
        {
            Position = position;
            ExecutingUser = executingUser;
            TemplateId = templateId;
        }

        public Guid TemplateId { get; set; }
        public int Position { get; set; }
        public Guid ExecutingUser { get; }
    }
}