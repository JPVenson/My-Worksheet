namespace MyWorksheet.Website.Server.Services.Statistics;

public class StatisticsDataPresenter
{
    public StatisticsDataPresenter(string name, string defaultChart, params string[] allowedCharts)
    {
        Name = name;
        AllowedCharts = allowedCharts;
        DefaultChart = defaultChart;
    }

    public string Name { get; set; }
    public string[] AllowedCharts { get; }
    public string DefaultChart { get; }
}