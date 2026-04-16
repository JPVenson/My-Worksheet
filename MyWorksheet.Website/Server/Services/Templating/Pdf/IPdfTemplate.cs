using System.IO;

namespace MyWorksheet.Website.Server.Services.Templating.Pdf;

public interface IPdfTemplate
{
    Stream RenderTemplate();
}