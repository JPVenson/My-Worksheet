using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ResLoaded;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Shared;

public partial class ResourcesListComponent
{
    [Inject]
    public ResourceLoaderService ResourceLoaderService { get; set; }

    protected override void OnInitialized()
    {
        ResourceLoaderService.ResourceAdded.Register(StateHasChanged);
        ResourceLoaderService.ResourceRemoved.Register(StateHasChanged);
        base.OnInitialized();
    }
}