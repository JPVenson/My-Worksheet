using MyWorksheet.Website.Server.Services.Text;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class LocProvider
{
    [MorestachioFormatter("Loc", "Translates a key")]
    public static object Loc([SourceObject] string locKey, [ExternalData] ITextService textService)
    {
        return textService.Compile(locKey);
    }
    [MorestachioFormatter("Loc", "Translates a key")]
    public static object Loc([SourceObject] object any, string locKey, [ExternalData] ITextService textService)
    {
        return textService.Compile(locKey);
    }
}