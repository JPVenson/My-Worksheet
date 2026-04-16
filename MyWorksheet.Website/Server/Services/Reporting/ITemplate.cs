using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;

namespace MyWorksheet.Website.Server.Services.Reporting;

public interface ITemplate
{
    Task<string> RenderTemplateAsync(IDictionary<string, object> arguments, UserContext userContext);
    Task<Stream> RenderTemplateStreamAsync(IDictionary<string, object> arguments, UserContext userContext);
}