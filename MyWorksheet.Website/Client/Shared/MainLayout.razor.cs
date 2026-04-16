using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Activity;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Breadcrumb;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.LocalStorage;
using MyWorksheet.Website.Client.Services.LocalStorage.Entities;
using MyWorksheet.Website.Client.Services.Module;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Services.Plugins;
using MyWorksheet.Website.Client.Services.ResLoaded;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Client.Services.UI;
using MyWorksheet.Website.Client.Services.UserSettings;
using MyWorksheet.Website.Client.Shared.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MyWorksheet.Website.Client.Shared;

public partial class MainLayout
{
    public MainLayout()
    {
        LayoutController = new LayoutController();
        WhenChanged(LayoutController).ThenRefresh(this);
        HubState = new HubState();
    }

    [Inject]
    public PluginService PluginService { get; set; }
    [Inject]
    public UIService UIService { get; set; }

    [Inject]
    public BreadcrumbService BreadcrumbService { get; set; }

    [Inject]
    public ModuleService ModuleService { get; set; }

    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    [Inject]
    public DialogService DialogService { get; set; }

    [Inject]
    public TextService TextService { get; set; }

    [Inject]
    public ActivityService ActivityService { get; set; }

    [Inject]
    public NavigationService NavigationService { get; set; }

    [Inject]
    public SignalService SignalService { get; set; }

    [Inject]
    public StorageService StorageService { get; set; }

    [Inject]
    public UserSettingsService UserSettingsService { get; set; }


    public LayoutController LayoutController { get; set; }

    private CultureInfo _selectedCulture;

    private static string GetTitleCaseNativeLanguage(CultureInfo culture)
    {
        switch (culture.Name.ToUpper())
        {
            case "EN-US":
                return "English (Simplified)";
            case "EN-GB":
                return "English";
            case "DE-DE":
                return "Deutsch";
        }

        return culture.NativeName;
    }

    public CultureInfo SelectedCulture
    {
        get { return _selectedCulture; }
        set { SetProperty(ref _selectedCulture, value); }
    }

    private async Task UpdateCulture(CultureInfo value)
    {
        if (TextService.CurrentCulture.Culture.ThreeLetterISOLanguageName == value.ThreeLetterISOLanguageName)
        {
            return;
        }

        await TextService.UpdateCulture(value);
    }

    public HubState HubState { get; set; }

    /// <summary>
    ///     Gets the content to be rendered inside the layout.
    /// </summary>
    [Parameter]
    public RenderFragment Body { get; set; }

    public PresentationState PresentationState { get; set; }

    private bool _isInit = false;

    protected override void OnInitialized()
    {
        if (_isInit)
        {
            return;
        }

        _isInit = true;
        PluginService.PluginsChanged = new EventCallback(this, new Action(StateHasChanged));
        AddDisposable(PluginService.AddPlugin("Breadcrumbs", BreadcrumbDisplay(), PluginLocation.Header));
        AddDisposable(PluginService.AddPlugin("Activities", DisplayNotificationsPlugin(), PluginLocation.Header,
            PluginOrientation.Right));
        AddDisposable(PluginService.AddPlugin("Logoff", DisplayLogoutPlugin(), PluginLocation.Header,
            PluginOrientation.Right));
        AddDisposable(PluginService.AddPlugin("LangSelection", LanguageSelectionPlugin(), PluginLocation.Footer,
            PluginOrientation.Right));
        AddDisposable(PluginService.AddPlugin("PresentationSelection", PresentationSelectionPlugin(),
            PluginLocation.Footer, PluginOrientation.Right));
        WhenChanged(BreadcrumbService).ThenRefresh(this);
        WhenChanged(ModuleService).ThenRefresh(this);
        WhenChanged(CurrentUserStore).ThenRefresh(this);
        WhenChanged(ActivityService).ThenRefresh(this);
        SelectedCulture = TextService.CurrentCulture.Culture;
        CurrentUserStore.WhenChanged()
            .UserIsAuthenticated(() => ActivityService.ActivitiesCache.Cache.LoadAll()).Invoke();
        HubState = SignalService.HubState;
        NavigationService.SameLocationChanged += NavigationService_SameLocationChanged;
        SignalService.HubStateChanged += SignalService_HubStateChanged;
        AddDisposable(new Disposable(() =>
        {
            NavigationService.SameLocationChanged -= NavigationService_SameLocationChanged;
            SignalService.HubStateChanged -= SignalService_HubStateChanged;
        }));
        PresentationState = new PresentationState();
        AddDisposable(StorageService.PresentationState.ValueChanged.Register(state =>
        {
            PresentationState = state;
            Render();
        }));
        OnNextRender(() => UIService.UiLoaded.Raise().AsTask());
    }

    protected override async Task OnInitializedAsync()
    {
        PresentationState = (await StorageService.PresentationState.ReadValue());
        await base.OnInitializedAsync();
    }

    private Task SignalService_HubStateChanged(HubState arg)
    {
        HubState = arg ?? new HubState();
        Render();
        return Task.CompletedTask;
    }

    private void NavigationService_SameLocationChanged(object sender, LocationChangedEventArgs e)
    {
        Body = null;
    }

    public void Logoff()
    {
        DialogService.Show("LogoffDialog");
    }

    private Task UpdatePresentationMode(bool arg)
    {
        PresentationState.Enabled = arg;
        return StorageService.PresentationState.WriteValue(PresentationState).AsTask();
    }

    private async Task UpdateDecimalTimesMode(bool arg)
    {
        UserSettingsService.UiSettings.UseDecimalTimes = arg;
        await UserSettingsService.SaveUiSettings();
    }
}