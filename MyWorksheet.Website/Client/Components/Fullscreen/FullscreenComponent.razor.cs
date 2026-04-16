using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Components.Fullscreen;

public partial class FullscreenComponent
{
    public FullscreenComponent()
    {
        ContainerId = Guid.NewGuid().ToString("N");
    }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    public ElementReference Container { get; set; }

    [Parameter]
    public string CollapsedContainerClass { get; set; }

    [Parameter]
    public string ExpandedContainerClass { get; set; }

    public bool HasFullscreenValue { get; private set; }

    private FullscreenController _fullscreenController;

    [Parameter]
    public FullscreenController FullscreenController
    {
        get { return _fullscreenController; }
        set { SetProperty(ref _fullscreenController, value, FullscreenControllerChanged); }
    }

    [Parameter]
    public EventCallback<FullscreenController> FullscreenControllerChanged { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    public string ContainerId { get; set; }

    [Parameter]
    public EventCallback OnFullscreenChanged { get; set; }

    public string EventKey { get; set; }
    protected override async Task OnInitializedAsync()
    {
        EventKey = await JsRuntime.InvokeAsync<string>("fullscreenBlazorApi.subscribeToFullscreenChangedEvent", DotNetObjectReference.Create(this));
        WhenDisposed(() => JsRuntime.InvokeVoidAsync("fullscreenBlazorApi.unsubscribeToFullscreenChangedEvent", EventKey));
        await base.OnInitializedAsync();
        FullscreenController = new FullscreenController(this);
    }

    [JSInvokable("onFullscreenChangedCallback")]
    public async void OnFullscreenChangedCallback()
    {
        OnFullscreenChanged.Raise();
        await FullscreenController.FullscreenChanged();
        await HasFullscreen();
    }

    public async Task RequestFullscreen()
    {
        await JsRuntime.InvokeVoidAsync("fullscreenBlazorApi.requestFullscreen", Container);
    }

    public async Task ExitFullscreen()
    {
        await JsRuntime.InvokeVoidAsync("fullscreenBlazorApi.exitFullscreen");
    }

    public async Task<bool> HasFullscreen()
    {
        return HasFullscreenValue = await JsRuntime.InvokeAsync<bool>("fullscreenBlazorApi.hasFullscreen", ContainerId);
    }

    public async Task<string> GetFullscreenElement()
    {
        return await JsRuntime.InvokeAsync<string>("fullscreenBlazorApi.getFullscreenElement");
    }
}