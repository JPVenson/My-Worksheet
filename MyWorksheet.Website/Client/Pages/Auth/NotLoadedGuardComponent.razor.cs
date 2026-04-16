using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Auth;

public partial class NotLoadedGuardComponent
{
    private ViewInitState _viewState;

    [Parameter]
    public ViewInitState ViewState
    {
        get { return _viewState; }
        set { SetProperty(ref _viewState, value, ViewStateChanged); }
    }

    [Parameter] public EventCallback<ViewInitState> ViewStateChanged { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    public WaiterService WaiterService { get; set; }

    protected override Task OnInitializedAsync()
    {
        AddDisposable(WaiterService.IsWaitingChanged.Register(Render));
        return base.OnInitializedAsync();
    }
}