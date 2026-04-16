using System.IO;

namespace MyWorksheet.Website.Server.Services.Templating.Pdf;

public class HtmlPdfTemplate : IPdfTemplate
{
    private readonly Stream _htmlTemplate;

    public HtmlPdfTemplate(Stream htmlTemplate)
    {
        _htmlTemplate = htmlTemplate;
    }

    public Stream RenderTemplate()
    {
        _htmlTemplate.Seek(0, SeekOrigin.Begin);
        return _htmlTemplate;
    }
}