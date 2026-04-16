using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Pages.Home.Widgets;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Pages.Home;

public partial class Dashboard : NavigationPageBase, IAsyncDisposable
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    public bool IsEditMode { get => field; set => SetProperty(ref field, value); }
    public bool IsAddPanelOpen { get; set; }

    public List<DashboardWidgetInstance> ActiveWidgets { get; set; } = new();

    public IEnumerable<DashboardWidgetDefinition> FilteredAvailableWidgets =>
        DashboardWidgetDefinition.All.Where(d =>
            !d.DisplayOnce || ActiveWidgets.All(w => w.WidgetKey != d.Key));

    private DotNetObjectReference<Dashboard> _dotNetRef;
    private bool _gridInitialized;
    private readonly HashSet<string> _registeredWidgetIds = new();

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        var savedPlugins = ServerErrorManager.EvalAndUnbox(
            await HttpService.DashboardApiAccess.GetDashboardPluginInfos());

        if (savedPlugins?.Length > 0)
        {
            ActiveWidgets = savedPlugins
                .OrderBy(p => p.GridY).ThenBy(p => p.GridX)
                .Select(p => new DashboardWidgetInstance(p.ArgumentsQuery, p)
                {
                    X = p.GridX,
                    Y = p.GridY,
                    W = Math.Max(1, p.GridWidth),
                    H = Math.Max(1, p.GridHeight),
                })
                .ToList();
        }
        else
        {
            ActiveWidgets = new List<DashboardWidgetInstance>
            {
                new DashboardWidgetInstance(DashboardWidgetDefinition.KeyWelcome) { X = 0, Y = 0, W = 2, H = 2 },
                new DashboardWidgetInstance(DashboardWidgetDefinition.KeyActiveWorksheets) { X = 0, Y = 2, W = 4, H = 3 },
                new DashboardWidgetInstance(DashboardWidgetDefinition.KeyEarnings) { X = 4, Y = 2, W = 4, H = 3 },
                new DashboardWidgetInstance(DashboardWidgetDefinition.KeyPublicReports) { X = 8, Y = 2, W = 4, H = 3 },
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!_gridInitialized && ActiveWidgets.Any())
        {
            _dotNetRef ??= DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("GridStackInterop.init", _dotNetRef, 12);
            _gridInitialized = true;
            foreach (var w in ActiveWidgets)
                _registeredWidgetIds.Add(w.Id);
        }
        else if (_gridInitialized)
        {
            // Register widgets that were added after initial render
            foreach (var w in ActiveWidgets.Where(w => !_registeredWidgetIds.Contains(w.Id)).ToList())
            {
                await JS.InvokeVoidAsync("GridStackInterop.addWidget", w.Id, w.X, w.Y, w.W, w.H);
                _registeredWidgetIds.Add(w.Id);
            }
        }
    }

    public RenderFragment BuildWidget(DashboardWidgetInstance instance)
    {
        var metadata = DashboardWidgetDefinition.All.FirstOrDefault(e => e.Key == instance.WidgetKey);

        if (metadata is null)
        {
            return DefaultWidget()(instance);
        }

        return (context) =>
        {
            context.OpenComponent(0, metadata.BlazorComponentType);
            context.AddAttribute(1, nameof(WidgetViewBase.WidgetInstance), instance);
            context.CloseComponent();
        };
    }

    public async Task StartEdit()
    {
        IsEditMode = true;
        IsAddPanelOpen = true;
        if (_gridInitialized)
            await JS.InvokeVoidAsync("GridStackInterop.setEditMode", true);
    }

    public async Task StopEdit()
    {
        IsEditMode = false;
        IsAddPanelOpen = false;
        if (_gridInitialized)
            await JS.InvokeVoidAsync("GridStackInterop.setEditMode", false);
        await SaveLayout();
    }

    private async Task SaveLayout()
    {
        var pluginInfos = ActiveWidgets.Select(inst => new DashboardPluginInfoViewModel
        {
            ArgumentsQuery = inst.ToArgumentsQuery(),
            GridX = inst.X,
            GridY = inst.Y,
            GridWidth = inst.W,
            GridHeight = inst.H,
        }).ToArray();

        using (WaiterService.WhenDisposed())
        {
            ServerErrorManager.Eval(await HttpService.DashboardApiAccess.UpdateDashboardPluginInfos(pluginInfos));
        }
    }

    [JSInvokable]
    public void OnGridChanged(GridStackItem[] items)
    {
        foreach (var item in items)
        {
            var inst = ActiveWidgets.FirstOrDefault(w => w.Id == item.Id);
            if (inst != null)
            {
                inst.X = item.X;
                inst.Y = item.Y;
                inst.W = item.W;
                inst.H = item.H;
            }
        }
    }

    public async Task AddWidget(DashboardWidgetDefinition definition)
    {
        var inst = new DashboardWidgetInstance(definition.Key)
        {
            X = 0,
            Y = 0,
            W = DefaultWidgetWidth(definition.Key),
            H = DefaultWidgetHeight(definition.Key),
        };
        ActiveWidgets.Add(inst);
        if (definition.DisplayOnce && !FilteredAvailableWidgets.Any())
            IsAddPanelOpen = false;
        StateHasChanged(); // OnAfterRenderAsync will register the new widget with GridStack
    }

    public async Task RemoveWidget(DashboardWidgetInstance widget)
    {
        if (_gridInitialized)
            await JS.InvokeVoidAsync("GridStackInterop.removeWidget", widget.Id);
        _registeredWidgetIds.Remove(widget.Id);
        ActiveWidgets.Remove(widget);
        StateHasChanged();
    }

    private static int DefaultWidgetWidth(string key) => key switch
    {
        DashboardWidgetDefinition.KeyProjectChart => 4,
        DashboardWidgetDefinition.KeyWelcome => 4,
        _ => 4,
    };

    private static int DefaultWidgetHeight(string key) => key switch
    {
        DashboardWidgetDefinition.KeyProjectChart => 3,
        DashboardWidgetDefinition.KeyWelcome => 2,
        _ => 3,
    };

    public string GetWidgetTitle(string widgetKey)
        => DashboardWidgetDefinition.All.FirstOrDefault(d => d.Key == widgetKey)?.Name ?? "Dashboard/Widget.Unknown";

    public string GetWidgetIcon(string widgetKey)
        => DashboardWidgetDefinition.All.FirstOrDefault(d => d.Key == widgetKey)?.Icon ?? "fas fa-puzzle-piece";

    public new async ValueTask DisposeAsync()
    {
        if (_gridInitialized)
        {
            try { await JS.InvokeVoidAsync("GridStackInterop.destroy"); } catch { /* page may have unloaded */ }
        }
        _dotNetRef?.Dispose();
        await base.DisposeAsync();
    }
}

public record GridStackItem(string Id, int X, int Y, int W, int H);

