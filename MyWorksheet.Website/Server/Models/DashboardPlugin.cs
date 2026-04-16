using System;
namespace MyWorksheet.Website.Server.Models;

public partial class DashboardPlugin
{
    public Guid DashboardPluginId { get; set; }

    public string ArgumentsQuery { get; set; }

    public int GridWidth { get; set; }

    public int GridHeight { get; set; }

    public int GridX { get; set; }

    public int GridY { get; set; }

    public Guid IdAppUser { get; set; }
}
