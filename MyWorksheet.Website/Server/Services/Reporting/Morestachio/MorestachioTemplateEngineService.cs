using System;
using System.IO;
using System.Text;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting.CustomDocuments;
using MyWorksheet.Website.Server.Services.StreamPool;
using Microsoft.EntityFrameworkCore;
using Morestachio;
using Morestachio.Helper.Localization;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.Morestachio;

[ScopedService(typeof(ITemplateEngine))]
public class MorestachioTemplateEngineService : ITemplateEngine
{
    private readonly MustachioFormatterService _mustachioFormatterService;
    private readonly ILocalFileStreamPoolService _localFileStreamPoolService;
    private readonly IMorestachioLocalizationService _morestachioLocalizationService;

    public MorestachioTemplateEngineService(MustachioFormatterService mustachioFormatterService,
        ILocalFileStreamPoolService localFileStreamPoolService,
        IMorestachioLocalizationService morestachioLocalizationService)
    {
        _mustachioFormatterService = mustachioFormatterService;
        _localFileStreamPoolService = localFileStreamPoolService;
        _morestachioLocalizationService = morestachioLocalizationService;
    }

    public ITemplate GenerateTemplate(string template, Guid userId)
    {
        return GenerateTemplate(template, long.MaxValue, userId);
    }

    public IObjectSchema GetFrameworkAddons(IDbContextFactory<MyworksheetContext> db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }

    public static string PrgKey { get; } = "Morestachio";
    public string Key { get; } = PrgKey;
    public string DisplayKey { get; } = null;
    public string DescriptionKey { get; } = null;

    public ITemplate GenerateTemplate(string template, long maxSize, Guid userId)
    {
        Stream SourceStream() => _localFileStreamPoolService
            .GetLocalStream(LocalFileStreamPoolPoolService.OPKEY_GENERATE_TEMPLATE, userId, 0).CreateTempStream();

        var parsingOptions = ParserOptionsBuilder.New()
            .WithTemplate(template)
            .WithTargetStream(SourceStream)
            .WithEncoding(Encoding.UTF8)
            .WithMaxSize(maxSize)
            .WithDisableContentEscaping(false)
            .WithTimeout(TimeSpan.FromSeconds(30))
            .WithFormatterService(_mustachioFormatterService)
            .WithLocalizationService(() => _morestachioLocalizationService)
            .AddCustomDocument(PdfAddonDocumentItemProvider.Header())
            .AddCustomDocument(PdfAddonDocumentItemProvider.Footer())
            .Build();

        return new MorestachioTemplate(Parser.ParseWithOptions(parsingOptions));
    }
}