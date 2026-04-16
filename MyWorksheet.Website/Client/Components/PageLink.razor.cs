using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Module;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class PageLink
{
    [Parameter]
    public string Name { get; set; }

    [Inject]
    public ModuleService ModuleService { get; set; }

    [Parameter]
    public string Link { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Arguments { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Link != null)
        {
            return;
        }

        var module = ModuleService.Modules.FirstOrDefault(e => e.LocName == Name);

        if (module == null)
        {
            return;
        }

        Link = module.Url;
        foreach (var argument in Arguments)
        {
            Link = Link.Replace($"{argument.Key}", argument.Value.ToString());
        }


        ChildContent = ChildContent ?? GetDefaultLinkContent(module);
    }
}