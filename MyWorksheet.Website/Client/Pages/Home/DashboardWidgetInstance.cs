using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;

namespace MyWorksheet.Website.Client.Pages.Home;

public class DashboardWidgetInstance
{
    public DashboardWidgetInstance(string argumentsQuery, DashboardPluginInfoViewModel savedInfo = null)
    {
        SavedInfo = savedInfo;

        // ArgumentsQuery uses a URI-like format: "WidgetKey?param1=value1&param2=value2"
        var qIdx = argumentsQuery?.IndexOf('?') ?? -1;
        if (qIdx >= 0)
        {
            WidgetKey = argumentsQuery.Substring(0, qIdx);
            Arguments = ParseQueryString(argumentsQuery.Substring(qIdx + 1));
        }
        else
        {
            WidgetKey = argumentsQuery ?? string.Empty;
            Arguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static Dictionary<string, string> ParseQueryString(string qs)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in qs.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var eq = pair.IndexOf('=');
            if (eq >= 0)
                dict[Uri.UnescapeDataString(pair.Substring(0, eq))] = Uri.UnescapeDataString(pair.Substring(eq + 1));
        }
        return dict;
    }

    /// <summary>Reconstruct the full URI-like ArgumentsQuery string.</summary>
    public string ToArgumentsQuery()
    {
        if (Arguments.Count == 0)
            return WidgetKey;
        var qs = string.Join("&", Arguments.Select(kv =>
            Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value)));
        return WidgetKey + "?" + qs;
    }

    public void SetArg(string key, string value)
    {
        if (value is null)
            Arguments.Remove(key);
        else
            Arguments[key] = value;
    }

    public string GetArg(string key) =>
        Arguments.TryGetValue(key, out var v) ? v : null;

    public Guid? GetGuidArg(string key) =>
        Arguments.TryGetValue(key, out var v) && Guid.TryParse(v, out var g) ? g : null;

    public string WidgetKey { get; }
    public DashboardPluginInfoViewModel SavedInfo { get; }
    public Dictionary<string, string> Arguments { get; }

    /// <summary>Stable session ID used as the GridStack element identifier.</summary>
    public string Id { get; } = Guid.NewGuid().ToString("N");

    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
}

