using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.LocalStorage;
using MyWorksheet.Website.Client.Services.LocalStorage.Entities;
using MyWorksheet.Website.Client.Services.Presentation;
using MyWorksheet.Website.Client.Util;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class PresentationStateAwareComponent : IDisposable
{
    private PubSubEvent.TrackableDisposable _presentationStateChangedHandle;

    [Inject]
    public PresentationModeService PresentationModeService { get; set; }

    [Parameter]
    public RenderFragment Enabled { get; set; }
    [Parameter]
    public RenderFragment Disabled { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _presentationStateChangedHandle = PresentationModeService.PresentationStateChanged.Register(StateHasChanged);
        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        _presentationStateChangedHandle?.Dispose();
    }
}