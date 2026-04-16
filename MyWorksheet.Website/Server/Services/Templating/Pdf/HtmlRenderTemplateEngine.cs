using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ServerManager;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.Playwright;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Templating.Pdf;

[SingletonService(typeof(IPdfTemplateEngine), typeof(IReportCapability))]
public class HtmlRenderTemplateEngine : RequireInit, IPdfTemplateEngine
{
    private readonly IAppLogger _appLogger;

    static HtmlRenderTemplateEngine()
    {
        var provider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(provider);
    }

    public HtmlRenderTemplateEngine(IAppLogger appLogger)
    {
        _appLogger = appLogger;
    }

    public async Task<IPdfTemplate> GenerateTemplate(string template, PDfGenerationOptions options)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Headless = true,
        });

        var page = await browser.NewPageAsync(new BrowserNewPageOptions()
        {
            BypassCSP = true
        });
        await page.SetContentAsync(template);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions());

        await page.EmulateMediaAsync(new PageEmulateMediaOptions()
        {
            Media = Media.Print,
        });

        var pdfOptions = new PagePdfOptions()
        {
            Format = PaperFormat.A4,
            FooterTemplate = options.Footer?.Html ?? "",
            HeaderTemplate = options.Header?.Html ?? "",
            DisplayHeaderFooter = options.Header is { } || options.Footer is { },
            Margin = options.Header is { } || options.Footer is { } ? new Margin()
            {
                Top = $"{options.Header?.Height ?? 0}px",
                Bottom = $"{options.Footer?.Height ?? 0}px",
            } : null,
        };
        var bytes = await page.PdfAsync(pdfOptions);
        return new HtmlPdfTemplate(new MemoryStream(bytes));
    }

    public async Task<IPdfTemplate> GenerateTemplate(Stream template, PDfGenerationOptions options)
    {
        using var stream = template;
        using var reader = new StreamReader(template);
        return await GenerateTemplate(await reader.ReadToEndAsync(), options);
    }

    public ProcessorCapability[] ReportCapabilities()
    {
        return new[]
        {
            new ProcessorCapability
            {
                Name = "Feature_Reporting_PostSteps_Pdf",
                Value = "true",
                IsEnabled = true
            }
        };
    }

    //public RevisionInfo ChromeInfo { get; private set; }

    public override async ValueTask InitAsync(IServiceProvider serviceProvider)
    {
        _appLogger.LogVerbose("Check Chrome PDF status");
        Microsoft.Playwright.Program.Main(new string[] { "install" });
        //Microsoft.Playwright.Program.Main(new string[] { "install -deps chromium" });
        //var playwright = await Playwright.CreateAsync();

        //var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions()
        //{
        //	Path = AppDomain.CurrentDomain.BaseDirectory
        //});


        //browserFetcher.DownloadProgressChanged += BrowserFetcher_DownloadProgressChanged;
        //ChromeInfo = await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
    }

    private int _lastPercentage = -1;
    private void BrowserFetcher_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
    {
        if (_lastPercentage == e.ProgressPercentage)
        {
            return;
        }
        _lastPercentage = e.ProgressPercentage;
        _appLogger.LogInformation($"Chrome Download Progress: {e.ProgressPercentage}");
    }
}