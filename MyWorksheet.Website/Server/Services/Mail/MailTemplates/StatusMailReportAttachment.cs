using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Webpage.Helper.Utlitiys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class StatusMailReportAttachment : MailAttachment
{
    public class Statistics
    {
        public PaymentOrder[] NewOrders { get; set; }
        public AppUser[] NewUsers { get; set; }
        public AppUser[] NewUsersTest { get; set; }
        public decimal Earned { get; set; }
    }

    private readonly AppLoggerLog[] _events;
    private readonly ILocalFileProvider _localFileProvider;
    private readonly IOptions<WebsiteInstanceSettings> _websiteInstanceSettings;
    private readonly ITemplateEngine _templateEngine;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public StatusMailReportAttachment(AppLoggerLog[] events,
        ILocalFileProvider localFileProvider,
        IOptions<WebsiteInstanceSettings> websiteInstanceSettings,
        ITemplateEngine templateEngine,
        IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _events = events;
        _localFileProvider = localFileProvider;
        _websiteInstanceSettings = websiteInstanceSettings;
        _templateEngine = templateEngine;
        _dbContextFactory = dbContextFactory;
    }

    public string TemplateContent { get; set; }

    public override async Task Init()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        var newOrders = db.PaymentOrders
            .Where(f => f.OrderCreatedDate >= yesterday && f.OrderCreatedDate < today)
            .ToArray();

        var featureContents = db.PromisedFeatureContents.ToArray();

        var statistics = new Statistics
        {
            NewOrders = newOrders,
            NewUsers = db.AppUsers
                .Where(f => f.CreateDate >= yesterday && f.CreateDate < today && !f.IsTestUser)
                .ToArray(),
            NewUsersTest = db.AppUsers
                .Where(f => f.CreateDate >= yesterday && f.CreateDate < today && f.IsTestUser)
                .ToArray(),
            Earned = newOrders
                .Where(f => f.IsOrderDone && f.IsOrderSuccess)
                .Select(e => featureContents.FirstOrDefault(f => f.PromisedFeatureContentId == e.IdPromisedFeatureContent))
                .Where(e => e != null)
                .Sum(e => e.Price)
        };

        var renderSelf = new Dictionary<string, object>
        {
            { "LoggerExceptions", _events },
            { "ServerId", _websiteInstanceSettings.Value.Id },
            { "Statistics", statistics }
        };

        using var cssStream = await _localFileProvider.ReadAllAsync("/Content/Themes/theme-day/site-theme.css");
        renderSelf.Add("css", cssStream.Stringify(false, Encoding.UTF8));

        using var templateStream = await _localFileProvider.ReadAllAsync("/StaticViews/EmailTemplates/DailyReportMail - Attachment.html");
        var template = templateStream.Stringify(false, Encoding.UTF8);
        TemplateContent = await _templateEngine.GenerateTemplate(template, Guid.Empty).RenderTemplateAsync(renderSelf, null);
    }
}