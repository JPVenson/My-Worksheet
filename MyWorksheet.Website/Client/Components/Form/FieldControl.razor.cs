using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Text;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class FieldControl<TValue> : InputControl<TValue>
{
    [Inject]
    public TextService TextService { get; set; }

    [Parameter]
    public RenderFragment<string> LabelTemplate { get; set; }
    [Parameter]
    public string Label { get; set; }
    [Parameter]
    public string HelpText { get; set; }
    [Parameter]
    public RenderFragment<FieldControl<TValue>> EditorTemplate { get; set; }

    public RenderFragment EditorTemplateContent { get; set; }

    public FieldControl()
    {
        FieldId = Guid.NewGuid().ToString();
    }

    public string FieldId { get; set; }

    [Parameter]
    public string FormGroupCss { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        EditorId = Guid.NewGuid().ToString();

        if (Placeholder == null)
        {
            TextService.Localize(Label)
                .AsTask().ContinueWith(t =>
                {
                    Placeholder = t.Result;
                });
        }

        if (LabelTemplate == null)
        {
            LabelTemplate = GetDefaultLabelTemplate();
        }

        var template = EditorTemplate;
        EditorTemplateContent = __renderer =>
        {
            if (template == null || (FieldType == FieldTypes.ServerText && ReadOnly))
            {
                GetEditor()(__renderer);
            }
            else
            {
                template(this)(__renderer);
            }
        };
    }
}