using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util;

namespace MyWorksheet.Website.Client.Components.Fullscreen;

public class FullscreenController
{
    public FullscreenController(FullscreenComponent component)
    {
        _component = component;
        OnFullscreenChanged = new PubSubEvent();
    }

    private FullscreenComponent _component;

    public PubSubEvent OnFullscreenChanged { get; private set; }

    public async Task RequestFullscreen()
    {
        await _component.RequestFullscreen();
    }

    public async Task ExitFullscreen()
    {
        await _component.ExitFullscreen();
    }

    public Task<bool> HasFullscreen()
    {
        return _component.HasFullscreen();
    }

    public Task<string> GetFullscreenElement()
    {
        return _component.GetFullscreenElement();
    }

    public ValueTask<ViewportInfo> GetViewportDimensions()
    {
        return _component.JsRuntime.InvokeAsync<ViewportInfo>("fullscreenBlazorApi.getViewportDimensions", null);
    }

    public async Task FullscreenChanged()
    {
        await OnFullscreenChanged.Raise();
    }
}


public class ViewportInfo
{
    public int Height { get; set; }
    public int Width { get; set; }

    public override string ToString()
    {
        return $"Height: {Height}; Width: {Width}";
    }
}