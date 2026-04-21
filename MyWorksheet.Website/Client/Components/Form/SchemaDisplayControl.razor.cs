using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class SchemaDisplayControl
{
    public SchemaDisplayControl()
    {
        SchemaItems = new List<SchemaInfoValue>();
    }
    private IObjectSchemaInfo _schemaInfo;
    [Parameter]
    public IObjectSchemaInfo SchemaInfo
    {
        get { return _schemaInfo; }
        set
        {
            if (SetProperty(ref _schemaInfo, value, SchemaInfoChanged))
            {
                RebuildSchema();
            }
        }
    }

    [Parameter]
    public EventCallback<IObjectSchemaInfo> SchemaInfoChanged { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    public IList<SchemaInfoValue> SchemaItems { get; set; }

    public IDictionary<string, SchemaInfoValue> ReferenceSchemas { get; set; } = new Dictionary<string, SchemaInfoValue>();

    [Parameter]
    public bool EmbeddedDisplay { get; set; }

    private void RebuildSchema()
    {
        if (SchemaInfo == null)
        {
            return;
        }
        SchemaItems.Clear();
        ReferenceSchemas.Clear();
        //Values.Clear();

        var valueStore = new ValueBag();
        foreach (var schemaInfoValue in SchemaInfoValue.FromObjectSchema(SchemaInfo, SchemaInfo, () => valueStore, ReferenceSchemas))
        {
            SchemaItems.Add(schemaInfoValue);
        }
    }

    private async Task ScrollIntoView(string typeDefName)
    {
        await JSRuntime.InvokeVoidAsync("MyWorksheet.Blazor.Window.BlazorScrollToId", "schema_referece_" + typeDefName);
    }
}