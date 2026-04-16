using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.Promise;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class DirtyMarkerControl
{
    [Parameter]
    public IEntityState EntityState { get; set; }

    [CascadingParameter(Name = "DirtyMarkerControlGroup")]
    public DirtyMarkerControlGroup Group { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Group != null)
        {
            Group.DirtyMarkerControls.Add(this);
        }
    }
}