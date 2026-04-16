using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Auth;

public partial class NotLoadedComponent : IDisposable
{
    private PubSubEvent.TrackableDisposable _isWaitingChangedHandle;

    [Inject]
    public WaiterService WaiterService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _isWaitingChangedHandle = WaiterService.IsWaitingChanged.Register(StateHasChanged);
    }

    public void Dispose()
    {
        _isWaitingChangedHandle?.Dispose();
    }
}