using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ExternalDomainValidator;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class HtmlFormatter
{
    [MorestachioFormatter("ExternalHttp", "Can be applied to any object. The Original value has no effect on the Formatter. Must have one argument of a valid Url. The content will be taken and written to this location.")]
    [MorestachioFormatterInput("Must be a valid url")]
    public static async Task<string> HttpCall(object any, string url,
        [ExternalData] IExternalDomainValidator externalDomainValidator,
        [ExternalData] UserContext userContext)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var cssUrl) || !externalDomainValidator.ValidateUrl(url, "http", "https").IsValid)
        {
            return null;
        }

        if (!await externalDomainValidator.TryCallDomain(cssUrl.Host, userContext.UserId))
        {
            return null;
        }

        using (var httpClient = new HttpClient())
        {
            using (var httpResponseMessage = await httpClient.GetAsync(url))
            {
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }
    }
}