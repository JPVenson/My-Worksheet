using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class ToggleEditorControl
{
    public ToggleEditorControl()
    {
        OnText = "Common/Positive";
        OffText = "Common/Negative";
        ChildContent = GetDefaultDisplay();
        EditorId = Guid.NewGuid().ToString("N");
    }

    public string EditorId { get; set; }

    private bool _value;

    [Parameter]
    public bool Value
    {
        get { return _value; }
        set
        {
            if (SetProperty(ref _value, value, ValueChanged))
            {
                OnValueChanged.Raise(value);
            }
        }
    }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<bool> OnValueChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    public RenderFragment<bool> ChildContent { get; set; }

    [Parameter]
    public LocalizableString ContentText { get; set; }

    [Parameter]
    public LocalizableString OnText { get; set; }

    [Parameter]
    public LocalizableString OffText { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private void ToggleState()
    {
        if (!ReadOnly)
        {
            Value = !Value;
        }
    }
}