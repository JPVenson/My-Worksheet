using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class FieldLayout
{
    [Parameter]
    public RenderFragment LabelContent { get; set; }
    [Parameter]
    public RenderFragment EditorContent { get; set; }
    [Parameter]
    public RenderFragment HelpTextContent { get; set; }

    [Parameter]
    public LocalizableString HelpText { get; set; }
    [Parameter]
    public string EditorCss { get; set; }
    [Parameter]
    public string LabelCss { get; set; }

    [Parameter]
    public string EditorId { get; set; }

    [Parameter]
    public string FormGroupCss { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }
}