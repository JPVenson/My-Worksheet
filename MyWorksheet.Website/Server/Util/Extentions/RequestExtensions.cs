using Microsoft.AspNetCore.Http;

namespace MyWorksheet.Website.Server.Util.Extentions;

public static class RequestExtensions
{
    public static string GetClientIp(this HttpRequest request)
    {
        return request.HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    public static string GetSignalId(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("x-mw-changetrackingid", out var id))
        {
            return id;
        }

        return null;
    }
}