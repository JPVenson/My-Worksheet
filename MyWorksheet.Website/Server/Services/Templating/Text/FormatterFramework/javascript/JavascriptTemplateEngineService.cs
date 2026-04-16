using System;
using System.Collections.Generic;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.StreamPool;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.EntityFrameworkCore;
using Morestachio.Formatter.Framework;
using ServiceLocator.Attributes;

namespace MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;

[SingletonService(typeof(ITemplateEngine))]
public class JavascriptTemplateEngineService : ITemplateEngine
{
    private readonly ILocalFileStreamPoolService _localFileStreamPoolService;
    private readonly ActivatorService _activatorService;

    public JavascriptTemplateEngineService(ILocalFileStreamPoolService localFileStreamPoolService,
        ActivatorService activatorService)
    {
        _localFileStreamPoolService = localFileStreamPoolService;
        _activatorService = activatorService;
    }

    public ITemplate GenerateTemplate(string template, long maxSize, Guid userId)
    {
        var sourceStream = _localFileStreamPoolService
            .GetLocalStream(LocalFileStreamPoolPoolService.OPKEY_GENERATE_TEMPLATE, userId, 0)
            .CreateTempStream();
        //var mustachioFormatterService = IoC.Resolve<MustachioFormatterService>();
        //var formatter = mustachioFormatterService.GetFormatterForUser(userId);
        return _activatorService.ActivateType<JavascriptTemplate>(template, new Dictionary<string, object>(), sourceStream, new List<MorestachioFormatterModel>());
    }

    public ITemplate GenerateTemplate(string template, Guid userId)
    {
        return GenerateTemplate(template, long.MaxValue, userId);
    }

    public IObjectSchema GetFrameworkAddons(IDbContextFactory<MyworksheetContext> db, Guid userId)
    {
        return JsonSchemaExtensions.JsonSchema(typeof(JavascriptGlobalObject), true);
    }

    private class JavascriptGlobalObject
    {
        [JsonComment("Defines the stream that must be written to to produce a report")]
        public JsDocumentStreamWrapper Out { get; set; }
        [JsonComment("Allows to call Format(name, arguments) to invoke a formatter")]
        public FormatterWrapper Formatter { get; set; }
    }

    public string Key { get; } = PrgKey;
    public string DisplayKey { get; } = "";
    public string DescriptionKey { get; } = "";
    public static string PrgKey { get; set; } = "javascript";
}