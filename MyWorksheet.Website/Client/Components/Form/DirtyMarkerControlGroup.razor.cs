using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class DirtyMarkerControlGroup
{
    public DirtyMarkerControlGroup()
    {
        DirtyMarkerControls = new HashSet<DirtyMarkerControl>();
    }

    public HashSet<DirtyMarkerControl> DirtyMarkerControls { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }
}