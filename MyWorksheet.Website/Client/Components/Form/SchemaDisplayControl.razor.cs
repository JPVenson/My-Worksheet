using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using Microsoft.AspNetCore.Components;

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

    public IList<SchemaInfoValue> SchemaItems { get; set; }

    [Parameter]
    public bool EmbeddedDisplay { get; set; }

    private void RebuildSchema()
    {
        if (SchemaInfo == null)
        {
            return;
        }
        SchemaItems.Clear();
        //Values.Clear();

        var valueStore = new ValueBag();
        foreach (var schemaInfoValue in SchemaInfoValue.FromObjectSchema(SchemaInfo, SchemaInfo.Schema, "", () => valueStore))
        {
            SchemaItems.Add(schemaInfoValue);
        }
    }
}