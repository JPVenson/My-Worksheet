using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MyWorksheet.Website.Client.Services.OverlayDraw;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class WaiterIndicatorComponent : IDisposable, IAsyncDisposable
{
    private PubSubEvent.TrackableDisposable _isWaitingChangedHandle;
    private OverlayHandler _overlayHandle;
    private int _currentIndex;

    [Parameter]
    [Inject]
    public WaiterService WaiterService { get; set; }

    [Inject]
    public OverlayDrawOrderService OverlayDrawOrderService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _isWaitingChangedHandle = WaiterService.IsWaitingChanged.Register(StateHasChanged);
        _overlayHandle = OverlayDrawOrderService.Reserve();
        _overlayHandle.OrderItem.Register((idx) =>
        {
            _currentIndex = idx;
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        _isWaitingChangedHandle?.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _overlayHandle.DisposeAsync();
    }
}