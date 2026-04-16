using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Services;
using MyWorksheet.Website.Client.Services.Breadcrumb;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Services.Plugins;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Shared.Layout;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Pages.Base;

public abstract class NavigationPageBase : EntityTrackingComponent
{
    protected NavigationPageBase()
    {
        ViewState = ViewInitState.Uninitialized;
    }

    public void AddPlugin(string name, RenderFragment content, PluginLocation pluginLocation)
    {
        AddDisposable(PluginService.AddPlugin(name, content, pluginLocation));
    }

    public void AddPlugin(string name, RenderFragment content, PluginLocation pluginLocation, PluginOrientation alignment)
    {
        AddDisposable(PluginService.AddPlugin(name, content, pluginLocation, alignment));
    }

    public virtual void OnSetPlugins()
    {

    }

    public virtual Task OnSetPluginsAsync()
    {
        return Task.CompletedTask;
    }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    [Inject]
    public TextService TextService { get; set; }

    [Inject]
    public PluginService PluginService { get; set; }
    [Inject]
    public NavigationService NavigationService { get; set; }
    [Inject]
    public ModuleService ModuleService { get; set; }
    [Inject]
    public WaiterService WaiterService { get; set; }
    [Inject]
    public DialogService DialogService { get; set; }
    [Inject]
    public BreadcrumbService BreadcrumbService { get; set; }

    [CascadingParameter]
    public LayoutController LayoutController { get; set; }

    public ServerErrorManager ServerErrorManager { get; set; }

    public void ChangeLayout(Action<LayoutData> layoutAction)
    {
        AddDisposable(LayoutController.Modifier(layoutAction));
    }

    public void TrackBreadcrumb(BreadcrumbPart part)
    {
        AddDisposable(new Disposable(() =>
        {
            BreadcrumbService.Remove(part);
        }));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OnSetPlugins();
        ServerErrorManager = ServerErrorManager ?? new ServerErrorManager(WaiterService);
    }

    protected virtual async Task SetTitleAsync(LocalizableString title)
    {
        ModuleService.CurrentTitle = title;
        var localize = await TextService.Localize(title.Text, title.Arguments);
        await JsRuntime.InvokeVoidAsync("MyWorksheet.Blazor.Window.SetTitle", localize);
    }

    public virtual Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }

    private ViewInitState _viewState;

    public ViewInitState ViewState
    {
        get { return _viewState; }
        set { SetProperty(ref _viewState, value); }
    }

    private int _parameterState = 0;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        try
        {
            var param = parameters.ToDictionary();
            int newState = 0;
            if (param.Count > 0)
            {
                newState = param.Select((e) => e.Key + e.Value?.ToString()).Select(f => f.GetHashCode()).Aggregate((e, f) => e ^ f);
            }

            if (newState != _parameterState)
            {
                _parameterState = newState;
                ViewState = ViewInitState.Unknown;

                await SoftDispose();
                OnPropertyChanged.Dispose();
            }

            await base.SetParametersAsync(parameters);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    protected override async Task OnInitializedAsync()
    {
        if (ViewState == ViewInitState.Uninitialized)
        {
            return;
        }

        ViewState = ViewInitState.Initializing;

        using (WaiterService.WhenDisposed())
        {
            Render();
            try
            {
                var pageAttribute = GetType().GetCustomAttribute<PageAttribute>();
                if (pageAttribute != null)
                {
                    ModuleService.SetCurrent(pageAttribute.GetName());
                    await SetTitleAsync(pageAttribute.LocName);
                }

                OnPropertyChanged.Register(() => PluginService.PluginsChanged.Raise());

                await LoadDataAsync();
                await OnSetPluginsAsync();
                if (ServerErrorManager.ServerErrors.Any())
                {
                    ViewState = ViewInitState.Error;
                    ServerErrorManager.DisplayStatus();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine("ERROR WHILE LOADING: " + e);
#endif
                ViewState = ViewInitState.Error;
            }
            finally
            {
                if (ViewState != ViewInitState.Error)
                {
                    ViewState = ViewInitState.Initialized;
                }
            }
        }

    }
}