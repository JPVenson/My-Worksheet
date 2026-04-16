using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Services.ExecuteLater;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.ExecuteLater.Actions;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Server.Services.Reporting.TemplateProvider;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using MyWorksheet.Website.Server.Services.Templating.Pdf;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Server.Services.UserCounter;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.ReportService.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class GenerateReport : IPriorityQueueAction
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IBlobManagerService _blobManagerService;
    private readonly ITemplateProviderService _templateProviderService;
    private readonly ITextService _textService;
    private readonly ITextTemplateManager _textTemplateManager;
    private readonly IPdfTemplateEngine _pdfTemplateEngine;
    private readonly IUserQuotaService _userQuotaService;
    public const string ActionKey = ExternalSchedulableTasks.CREATE_REPORT;
    public string Name => "Creates an Report";
    public string Key => ActionKey;
    public Version Version { get; set; }

    public GenerateReport(IDbContextFactory<MyworksheetContext> dbContextFactory,
        IBlobManagerService blobManagerService,
        ITemplateProviderService templateProviderService,
        ITextService textService,
        ITextTemplateManager textTemplateManager,
        IPdfTemplateEngine pdfTemplateEngine,
        IUserQuotaService userQuotaService)
    {
        _dbContextFactory = dbContextFactory;
        _blobManagerService = blobManagerService;
        _templateProviderService = templateProviderService;
        _textService = textService;
        _textTemplateManager = textTemplateManager;
        _pdfTemplateEngine = pdfTemplateEngine;
        _userQuotaService = userQuotaService;
    }

    public class ReportArguments : ArgumentsBase
    {
        public static ReportArguments Parse(IDictionary<string, object> arguments)
        {
            var ra = new ReportArguments();
            ra.SetOrAbort<Guid>(arguments, e => ra.TemplateId = e, "templateId");
            ra.Set<TextTemplateDataQuery>(arguments, e => ra.TextTemplateDataQuery = e, "TextTemplateDataQuery");
            ra.SetOrAbort<Guid>(arguments, e => ra.StorageProviderId = e, "storageProviderId");
            ra.SetOrAbort<Guid>(arguments, e => ra.NEngineRunningTaskId = e, "NEngineRunningTaskId");
            ra.Set<bool>(arguments, e => ra.Preview = e, "Preview");
            return ra.GetIfValid() as ReportArguments;
        }

        public Guid TemplateId { get; set; }
        public TextTemplateDataQuery TextTemplateDataQuery { get; set; }
        public Guid StorageProviderId { get; set; }
        public Guid NEngineRunningTaskId { get; set; }
        public bool Preview { get; set; }
    }

    /// <inheritdoc />
    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return new DictionaryElementsValidator<string, object>(arguments)
            .OfType<Guid>("templateId")
            .OfType<Guid>("storageProviderId")
            .OfType<Guid>("NEngineRunningTaskId")
            .Is<TextTemplateDataQuery>("TextTemplateDataQuery")
            .Result;
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        using var dbEntities = _dbContextFactory.CreateDbContext();
        var reportArguments = ReportArguments.Parse(queueElement.Arguments);
        try
        {
            var storageProvider = _blobManagerService;
            var maxReportSize = await storageProvider.GetMaxReportSize(reportArguments.StorageProviderId, queueElement.UserId, StorageEntityType.Report);

            //var nEngineTemplate = dbEntities.Select<NEngineTemplate>(reportArguments.TemplateId);
            var template = await _templateProviderService
                .FindTemplate(dbEntities, reportArguments.TemplateId);

            string templateContents;
            using (template.Item2)
            {
                using (var reader = new StreamReader(template.Item2, Encoding.Default))
                {
                    templateContents = await reader.ReadToEndAsync();
                }
            }

            long templateLength;
            BlobManagerSetOperationResult storageEntry;

            var context = new UserContext(queueElement.UserId);
            var templateEngines = _textTemplateManager.KnownTextFormatter;

            var templateEngine = templateEngines.FirstOrDefault(e => e.Key == template.Item1.UsedFormattingEngine);

            if (templateEngine.Value == null)
            {
                throw new InvalidOperationException($"The selected Formatting engine: '{template.Item1.UsedFormattingEngine}' could not be found");
            }

            var generateTemplate = templateEngine.Value.GenerateTemplate(templateContents, maxReportSize, queueElement.UserId);
            var templateInfo = dbEntities.NengineTemplates.Find(reportArguments.TemplateId);

            using (var renderTemplate = await generateTemplate.RenderTemplateStreamAsync(reportArguments.TextTemplateDataQuery.Values, context))
            {
                templateLength = renderTemplate.Length;
                renderTemplate.Seek(0, SeekOrigin.Begin);

                var fileNameResult = reportArguments.TextTemplateDataQuery.DataSourceName + "-Report";

                if (!string.IsNullOrWhiteSpace(templateInfo.FileNameTemplate))
                {
                    fileNameResult = await templateEngine
                        .Value
                        .GenerateTemplate(templateInfo.FileNameTemplate, queueElement.UserId)
                        .RenderTemplateAsync(reportArguments.TextTemplateDataQuery.Values, context);
                }

                if (!Path.HasExtension(fileNameResult))
                {
                    fileNameResult = Path.ChangeExtension(fileNameResult, templateInfo.FileExtention);
                }

                using (var reportBlob = new BlobData(renderTemplate, fileNameResult))
                {
                    if (templateInfo.FileExtention == "pdf")
                    {
                        var pdfTemplate = await _pdfTemplateEngine
                            .GenerateTemplate(renderTemplate, new PDfGenerationOptions()
                            {
                                Dimentions = context.PaperSize,
                                Grayscale = context.Grayscale,
                                Title = context.Title,
                                Footer = context.Footer,
                                Header = context.Header
                            });
                        await reportBlob.DataStream.DisposeAsync();
                        reportBlob.DataStream = pdfTemplate.RenderTemplate();
                    }

                    storageEntry = await storageProvider.SetData(reportBlob, reportArguments.StorageProviderId,
                        queueElement.UserId, StorageEntityType.Report,
                        !reportArguments.Preview);
                }
            }
            if (!storageEntry.Success)
            {
                throw new InvalidOperationException(
                    $"The selected Storage Provider has denied the new Report because: {storageEntry.Error}");
            }

            dbEntities.NengineRunningTasks.Where(e => e.NengineRunningTaskId == reportArguments.NEngineRunningTaskId)
                .ExecuteUpdate(f => f
                    .SetProperty(e => e.DateRun, DateTime.UtcNow)
                    .SetProperty(e => e.IsDone, true)
                    .SetProperty(e => e.IdStoreageEntry, storageEntry.Object.StorageEntryId));

            if (templateLength > maxReportSize)
            {
                dbEntities.NengineRunningTasks.Where(e => e.NengineRunningTaskId == reportArguments.NEngineRunningTaskId)
                    .ExecuteUpdate(f => f
                        .SetProperty(e => e.ErrorText, $"Data Truncated. The data Limit is Limited to {LocalFileStorageProvider.FormatBytes(maxReportSize)} per Report"));
            }
        }
        catch (Exception e)
        {
            var exMessage = e.Message;

            if (e is AggregateException agg)
            {
                foreach (var innerException in agg.Flatten().InnerExceptions)
                {
                    exMessage += Environment.NewLine + "---------------------------------" + Environment.NewLine +
                                 innerException.Message;
                }
            }

            dbEntities.NengineRunningTasks.Where(e => e.NengineRunningTaskId == reportArguments.NEngineRunningTaskId)
                .ExecuteUpdate(f => f
                    .SetProperty(e => e.DateRun, DateTime.UtcNow)
                    .SetProperty(e => e.IsDone, true)
                    .SetProperty(e => e.IsFaulted, true)
                    .SetProperty(e => e.ErrorText, exMessage));
        }
        finally
        {
            await _userQuotaService.Subtract(queueElement.UserId, 1, Quotas.ConurrentReports);
            _textTemplateManager.OnTemplateProcessed(reportArguments.TemplateId);
        }
    }
}