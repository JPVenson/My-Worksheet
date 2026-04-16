using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Services.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class StatusMail : TemplateMail
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IOptions<WebsiteInstanceSettings> _websiteInstanceSettings;
    private readonly ITemplateEngine _templateEngine;

    public StatusMail(ILocalFileProvider localFileProvider,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IOptions<WebsiteInstanceSettings> websiteInstanceSettings,
        ITemplateEngine templateEngine)
        : base(localFileProvider)
    {
        _dbContextFactory = dbContextFactory;
        _websiteInstanceSettings = websiteInstanceSettings;
        _templateEngine = templateEngine;
    }

    public override async Task Init()
    {
        Subject = "Your daily Status mail";
        using var db = _dbContextFactory.CreateDbContext();
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var events = db.AppLoggerLogs
            .Where(f => f.DateCreated >= yesterday)
            .ToArray();

        Values.Add("LoggerExceptions", events);
        var statusMailReportAttachment = new StatusMailReportAttachment(events, LocalFileProvider, _websiteInstanceSettings, _templateEngine, _dbContextFactory)
        {
            Name = "Report.pdf",
            FileType = "application/pdf"
        };
        await statusMailReportAttachment.Init();
        Body = statusMailReportAttachment.TemplateContent;
        await base.Init();
    }
}