using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJs.Blazor.BarChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Pages.TimeTracking.Projects;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Services.ResLoaded;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Statistics;

public partial class StatisticsPage
{
    public StatisticsPage()
    {
        StatisticViewModels = new List<StatisticsViewModel>();
        ProjectsSwitch = new Dictionary<Guid, (string Name, bool State)>();
    }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ResourceLoaderService ResourceLoaderService { get; set; }

    [Inject]
    public ICacheRepository<GetProjectModel> ProjectsRepository { get; set; }

    public IDictionary<Guid, (string Name, bool State)> ProjectsSwitch { get; set; }

    public IList<StatisticsViewModel> StatisticViewModels { get; set; }

    public StatisticsViewModel SelectedStatistics { get; set; }

    public ConfigBase ChartConfig { get; set; }

    /// <inheritdoc />
    public override async Task LoadDataAsync()
    {
        await ResourceLoaderService.AddResource(new ScriptLinkResource("https://cdn.jsdelivr.net/npm/chart.js@2.9.4/dist/Chart.min.js"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/ChartJs.Blazor.Fork/ChartJsBlazorInterop.js"));
        await ProjectsRepository.Cache.LoadAll();
        ProjectsSwitch = ProjectsRepository.Cache.ToDictionary(e => e.ProjectId, e => (e.Name, false));

        await base.LoadDataAsync();
        var statisticsProvider = ServerErrorManager.EvalAndUnbox(await HttpService.StatisticsApiAccess.GetStatisticsProvider());
        foreach (var statisticsDataPresenterViewModel in statisticsProvider)
        {
            StatisticViewModels.Add(new StatisticsViewModel()
            {
                DataPresenterViewModel = statisticsDataPresenterViewModel
            });
        }
    }

    public void ToggleProject(Guid projectId)
    {
        var state = ProjectsSwitch[projectId];
        ProjectsSwitch[projectId] = (state.Name, !state.State);
    }

    public void SetChart(DataDimensionModel statisticsViewModel)
    {
        ChartConfig = null;
        OnNextRender(() =>
        {
            ChartConfig = statisticsViewModel.LoadData();
        });
        StateHasChanged();
    }

    public async Task LoadData(StatisticsViewModel statisticsViewModel)
    {
        var data = HttpService.StatisticsApiAccess.GetData(statisticsViewModel.DataPresenterViewModel.Name,
            ProjectsSwitch.Where(e => e.Value.State).Select(e => e.Key).ToArray(),
            statisticsViewModel.ArgumentValues.Values,
            AggregationStrategy.AddWhereExistsAndDuplicateWhereNot);

        statisticsViewModel.DataExport = ServerErrorManager.EvalAndUnbox(await data);

        if (!statisticsViewModel.DataExport.DataDimensions.Any())
        {
            return;
        }
        statisticsViewModel.SelectedDimension = statisticsViewModel.DataExport.DataDimensions.First();
        SetChart(statisticsViewModel.SelectedDimension);
    }

    public async Task LoadChart(StatisticsViewModel statisticsViewModel)
    {
        SelectedStatistics = statisticsViewModel;
        using (WaiterService.WhenDisposed())
        {
            if (statisticsViewModel.ArgumentSchema == null)
            {
                statisticsViewModel.ArgumentSchema = ServerErrorManager.EvalAndUnbox(await HttpService.StatisticsApiAccess.GetArgumentSchema(statisticsViewModel.DataPresenterViewModel.Name));
            }
        }
    }
}

public class StatisticsViewModel
{
    public StatisticsViewModel()
    {
        ArgumentValues = new ValueBag();
    }

    public StatisticsDataPresenterViewModel DataPresenterViewModel { get; set; }
    public DataExportModel DataExport { get; set; }
    public IObjectSchema ArgumentSchema { get; set; }
    public ValueBag ArgumentValues { get; set; }

    public DataDimensionModel SelectedDimension { get; set; }
}

public static class ChartLoaderExtensions
{
    public static ConfigBase LoadData(this DataDimensionModel data)
    {
        if (data.DisplayAs == DisplayTypes.Bar.TypeName)
        {
            return data.LoadDataAsBar();
        }
        if (data.DisplayAs == DisplayTypes.Pie.TypeName)
        {
            return data.LoadDataAsPie();
        }
        if (data.DisplayAs == DisplayTypes.Line.TypeName)
        {
            return data.LoadDataAsLine();
        }

        return null;
    }

    private static string[] GetColors()
    {
        return new[]
        {
            ColorUtil.ColorHexString(255, 99, 132), // Slice 1 aka "Red"
			ColorUtil.ColorHexString(255, 205, 86), // Slice 2 aka "Yellow"
			ColorUtil.ColorHexString(75, 192, 192), // Slice 3 aka "Green"
			ColorUtil.ColorHexString(54, 162, 235), // Slice 4 aka "Blue"
		};
    }

    private static ConfigBase LoadDataAsPie(this DataDimensionModel data)
    {
        var config = new PieConfig();

        foreach (var dataDataSample in data.DataSamples)
        {
            var dataPoint = dataDataSample.DataLines.FirstOrDefault();
            if (dataPoint == null) continue;
            config.Data.Labels.Add(dataPoint.Label.Name);
        }
        config.Data.Datasets.Add(new PieDataset(data.DataSamples
            .Where(s => s.DataLines?.Count > 0)
            .SelectMany(f => f.DataLines.Select(e => e.DataPoint.DataValue.GetValueOrDefault(0))))
        {
            BackgroundColor = GetColors()
        });

        return config;
    }

    private static ConfigBase LoadDataAsLine(this DataDimensionModel data)
    {
        var config = new LineConfig();

        foreach (var dataDataSample in data.DataSamples.SelectMany(f => f.DataLines))
        {
            config.Data.Labels.Add(dataDataSample.Label.Name);
        }

        foreach (var dataDataSample in data.DataSamples)
        {
            config.Data.Datasets.Add(new LineDataset<double>(dataDataSample.DataLines.Select(f => f.DataPoint.DataValue.GetValueOrDefault(0)))
            {
                Label = dataDataSample.Label.Name,
                BackgroundColor = GetColors().First()
            });
        }


        return config;
    }

    private static ConfigBase LoadDataAsBar(this DataDimensionModel data)
    {
        var config = new LineConfig();

        foreach (var dataDataSample in data.DataSamples)
        {
            var dataPoint = dataDataSample.DataLines.FirstOrDefault();
            if (dataPoint == null) continue;
            config.Data.Labels.Add(dataPoint.Label.Name);
        }
        config.Data.Datasets.Add(new LineDataset<double>(data.DataSamples
            .Where(s => s.DataLines?.Count > 0)
            .SelectMany(f => f.DataLines.Select(e => e.DataPoint.DataValue.GetValueOrDefault(0))))
        {
            BackgroundColor = GetColors().First(),
        });

        return config;
    }
}