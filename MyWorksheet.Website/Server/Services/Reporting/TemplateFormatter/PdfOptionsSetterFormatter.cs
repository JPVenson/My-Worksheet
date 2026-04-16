using System.Drawing;
using ExCSS;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class PdfOptionsSetterFormatter
{
    //[MorestachioFormatter("pdf paper size", "Only for ToPdf Option: Sets the paper size. Can be for example: A2, A3, A4 or A4Extra")]
    //[MorestachioFormatterInput("Anything")]
    //public static object PaperSize(object any, string paperSize)
    //{
    //	if (Enum.TryParse<PaperKind>(paperSize, out var size))
    //	{
    //		var valuePaperSize = new PaperSize()
    //		{
    //			RawKind = (int) size
    //		};
    //		UserContext.CurrentContext.Value.PaperSize = new Size(valuePaperSize.Width, valuePaperSize.Height);
    //	}
    //	return any;
    //}

    [MorestachioFormatter("PdfPaperSize", "Only for ToPdf Option: Sets the paper size")]
    [MorestachioFormatterInput("Anything")]
    public static void PaperSize(object any, string width, string height,
        [ExternalData] UserContext userContext)
    {
        if (!Length.TryParse(width, out var widthUnit))
        {
            return;
        }

        if (!Length.TryParse(height, out var heightUnit))
        {
            return;
        }

        userContext.PaperSize = new SizeF(widthUnit.To(Length.Unit.In), heightUnit.To(Length.Unit.In));
        return;
    }

    [MorestachioFormatter("PdfGrayscale", "Only for ToPdf Option: Sets the Grayscale Option")]
    [MorestachioFormatterInput("Anything")]
    public static void Grayscale(object any, string grayscale,
        [ExternalData] UserContext userContext)
    {
        userContext.Grayscale = GlobalFormatter.FormatBoolean(grayscale);
    }

    [MorestachioFormatter("PdfTitle", "Only for ToPdf Option: Sets the Title Option")]
    [MorestachioFormatterInput("Anything")]
    public static void SetTitle(object any, string title,
        [ExternalData] UserContext userContext)
    {
        userContext.Title = title;
    }
}