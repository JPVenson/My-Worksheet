using MyWorksheet.Private.Models.ObjectSchema;
using System;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Reporting;

public interface ITemplateEngine
{
    ITemplate GenerateTemplate(string template, long maxSize, Guid userId);
    ITemplate GenerateTemplate(string template, Guid userId);
    IObjectSchema GetFrameworkAddons(IDbContextFactory<MyworksheetContext> db, Guid userId);
    string Key { get; }
    string DisplayKey { get; }
    string DescriptionKey { get; }
}