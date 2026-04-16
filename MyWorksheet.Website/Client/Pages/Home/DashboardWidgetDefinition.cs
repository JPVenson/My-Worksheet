using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyWorksheet.Website.Client.Pages.Home.Widgets;

namespace MyWorksheet.Website.Client.Pages.Home;

public class DashboardWidgetDefinition
{
    public const string KeyActiveWorksheets = "CurrentWorksheetsList";
    public const string KeyEarnings = "Earnings";
    public const string KeyTracker = "Tracker";
    public const string KeyTrackerButton = "TrackerButton";
    public const string KeyPublicReports = "PublicReports";
    public const string KeyTodayWorkload = "Today";
    public const string KeyTodayForProject = "TodayForProject";
    public const string KeyProjectChart = "Project Chart";
    public const string KeyWelcome = "Welcome";
    public const string KeyOrgs = "Orgs";

    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public bool DisplayOnce { get; set; }

    public Type BlazorComponentType { get; set; }

    public static IReadOnlyList<DashboardWidgetDefinition> All { get; } =
        typeof(DashboardWidgetAttribute).Assembly
            .GetTypes()
            .Select(t => (type: t, attr: t.GetCustomAttribute<DashboardWidgetAttribute>()))
            .Where(x => x.attr != null)
            .OrderBy(x => x.attr.Order)
            .ThenBy(x => x.attr.Key)
            .Select(x => new DashboardWidgetDefinition
            {
                Key = x.attr.Key,
                Name = x.attr.Name,
                Description = x.attr.Description,
                Icon = x.attr.Icon,
                DisplayOnce = x.attr.DisplayOnce,
                BlazorComponentType = x.type,
            })
            .ToArray();
}

