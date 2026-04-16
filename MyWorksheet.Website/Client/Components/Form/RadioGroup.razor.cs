using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class RadioGroup
{
    public RadioGroup()
    {
        Fields = new Dictionary<string, IList<InputControl<bool>>>();
    }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    public IDictionary<string, IList<InputControl<bool>>> Fields { get; set; }

    public void RegisterInGroup(InputControl<bool> control, string name)
    {
        if (!Fields.TryGetValue(name, out var list))
        {
            list = new List<InputControl<bool>>();
            Fields[name] = list;
        }

        list.Add(control);
    }

    public void DisableAll(string name)
    {
        if (Fields.TryGetValue(name, out var fields))
        {
            foreach (var fieldControl in fields)
            {
                fieldControl.Value = false;
                //fieldControl.Render();
            }
        }
    }

    public void DisableAll(string name, InputControl<bool> others)
    {
        if (Fields.TryGetValue(name, out var fields))
        {
            foreach (var fieldControl in fields)
            {
                fieldControl.Value = fieldControl != others;
                //fieldControl.Render();
            }
        }
    }
}