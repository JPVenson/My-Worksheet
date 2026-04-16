using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyWorksheet.Website.Client;

public class LocalizationAware : LocalizationAwareSelf
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override Task OnParametersSetAsync()
    {
        base.OnInitialized();

        TextService.ResourcesUpdated -= TextService_ResourcesUpdated;
        TextService.ResourcesUpdated += TextService_ResourcesUpdated;

        return Task.CompletedTask;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.AddContent(0, ChildContent);
    }
}

public class LocalizationAwareSelf : ComponentViewBase, IDisposable
{
    [Inject]
    public TextService TextService { get; set; }

    protected override Task OnParametersSetAsync()
    {
        base.OnInitialized();

        TextService.ResourcesUpdated -= TextService_ResourcesUpdated;
        TextService.ResourcesUpdated += TextService_ResourcesUpdated;

        return Task.CompletedTask;
    }

    protected virtual void TextService_ResourcesUpdated(object sender, string e)
    {
        StateHasChanged();
    }

    public override void OnDispose()
    {
        TextService.ResourcesUpdated -= TextService_ResourcesUpdated;
    }
}
