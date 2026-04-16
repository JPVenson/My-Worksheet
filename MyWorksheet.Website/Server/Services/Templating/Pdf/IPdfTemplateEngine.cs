using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ServerManager;

namespace MyWorksheet.Website.Server.Services.Templating.Pdf;

public interface IPdfTemplateEngine : IReportCapability
{
    Task<IPdfTemplate> GenerateTemplate(string template, PDfGenerationOptions options);
    Task<IPdfTemplate> GenerateTemplate(Stream template, PDfGenerationOptions options);
}

public class PDfGenerationOptions
{
    public SizeF? Dimentions { get; set; }
    public bool Grayscale { get; set; }
    public string Title { get; set; }

    public PdfAddon Header { get; set; }
    public PdfAddon Footer { get; set; }
}

public class PdfAddon
{
    public string Html { get; set; }
    public int Height { get; set; }
}