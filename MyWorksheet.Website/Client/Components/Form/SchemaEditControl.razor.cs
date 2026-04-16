using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class SchemaEditControl
{
    public SchemaEditControl()
    {
        SchemaItems = new List<SchemaInfoValue>();
    }

    private IObjectSchemaInfo _schemaInfo;

    private ValueBag _values;

    [Parameter]
    public ValueBag Values
    {
        get { return _values; }
        set
        {
            SetProperty(ref _values, value, ValuesChanged);
        }
    }

    [Parameter]
    public EventCallback<ValueBag> ValuesChanged { get; set; }

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

    public List<SchemaInfoValue> SchemaItems { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Values = Values ?? new ValueBag();
        WhenChanged(Values).Then(() =>
        {
            RebuildValues(Values);
        });
        RebuildSchema();
    }

    private void RebuildValues(ValueBag values)
    {
        foreach (var valuesValue in values.Values)
        {
            var pathParts = valuesValue.Key.Split('.');
            SchemaInfoValue schemaItem = null;
            foreach (var pathPart in pathParts)
            {
                schemaItem = schemaItem?.Children.Find(e => e.ValueName == pathPart) ??
                             SchemaItems.Find(e => e.ValueName == pathPart);
            }

            if (schemaItem == null)
            {
                continue;
            }

            if (Equals(schemaItem.Value, valuesValue.Value))
            {
                continue;
            }
            Render();
            break;
        }
    }

    private void RebuildSchema()
    {
        if (SchemaInfo == null)
        {
            return;
        }

        SchemaItems.Clear();

        var fromObjectSchema = SchemaInfoValue
            .FromObjectSchema(SchemaInfo, SchemaInfo.Schema, "", () => Values ?? new ValueBag())
            .ToArray();

        var items = new List<SchemaInfoValue>();
        foreach (var schemaInfoValue in fromObjectSchema)
        {
            items.Add(schemaInfoValue);
        }

        Render();
        OnNextRender(() =>
        {
            SchemaItems = items;
            Render();
        });
    }
}

public class ValueBag : ViewModelBase
{
    public ValueBag() : this(new Dictionary<string, object>())
    {

    }

    public ValueBag(IDictionary<string, object> values)
    {
        Values = values;
        Id = Guid.NewGuid().ToString("N");
    }

    public string Id { get; set; }

    public IDictionary<string, object> Values { get; }

    public object this[string index]
    {
        get
        {
            return Values.GetOrDefault(index, null);
        }
        set
        {
            Values[index] = value;
            SendPropertyChanged();
        }
    }

    public void Clear()
    {
        Values.Clear();
        Id = Guid.NewGuid().ToString("N");
    }

    public void LoadWith(IDictionary<string, object> values)
    {
        foreach (var value in values)
        {
            this[value.Key] = value.Value;
        }
    }
}

public class SchemaInfoValue
{
    public SchemaInfoValue()
    {
        Children = new List<SchemaInfoValue>();
    }

    private Func<ValueBag> _valueStore;
    public string Name { get; set; }
    public string ValueName { get; set; }

    public string DisplayName { get; set; }
    public string Comment { get; set; }
    public object DefaultValue { get; set; }
    public bool Optional { get; set; }
    public object SchemaType { get; set; }

    public bool BooleanValue
    {
        get
        {
            return Equals(Value, true);
        }
        set
        {
            Value = value;
        }
    }

    public object Value
    {
        get
        {
            return _valueStore()[ValueName];
        }
        set
        {
            _valueStore()[ValueName] = value;
        }
    }

    public string ValueAsString
    {
        get
        {
            return Value?.ToString();
        }
        set
        {
            object val = value;
            if (Optional && value == null)
            {
                val = null;
            }
            else if (val == null)
            {
                switch (GetHtmlInputType())
                {
                    case "number":
                        val = default(int);
                        break;
                    case "checkbox":
                        val = default(bool);
                        break;
                    case "date":
                        val = default(DateTime);
                        break;
                }
            }
            else
            {
                switch (GetHtmlInputType())
                {
                    case "number":
                        val = int.Parse(value);
                        break;
                    case "checkbox":
                        val = value == bool.TrueString;
                        break;
                    case "date":
                        val = DateTime.Parse(value);
                        break;
                }
            }
            Value = val;
        }
    }

    public List<SchemaInfoValue> Children { get; set; }

    public IDictionary<string, object> AllowedValues { get; set; }
    public SchemaEditDisplayType DisplayType { get; set; }

    public static IEnumerable<SchemaInfoValue> FromObjectSchema(IObjectSchemaInfo schema,
        IDictionary<string, object> valueObject,
        string name,
        Func<ValueBag> valueStore)
    {
        var values = new List<SchemaInfoValue>();
        foreach (var prop in valueObject)
        {
            var schemaValue = new SchemaInfoValue();
            schemaValue._valueStore = valueStore;
            var valueName = name + "" + prop.Key;
            var value = prop.Value;


            schemaValue.Optional = valueName.EndsWith("!") || value?.ToString() == "boolean";
            schemaValue.ValueName = valueName.TrimEnd('!');
            schemaValue.Name = valueName;
            schemaValue.DisplayName = schema.Names[valueName] ?? schemaValue.ValueName.Split('.').LastOrDefault();

            schemaValue.SchemaType = value;
            //schemaValue.Value = value;
            if (schemaValue.Value == null)
            {
                schemaValue.Value = schema.Defaults.GetOrDefault(valueName, null);
                schemaValue.ValueAsString = schemaValue.Value?.ToString();
            }
            schemaValue.Comment = schema.Comments.GetOrDefault(valueName, null);
            schemaValue.AllowedValues = schema.AllowedValues.GetOrDefault(valueName, null);

            if (value is string) // this is a primitive value like "string" "boolean" "string | Guid"
            {
                schemaValue.DisplayType = SchemaEditDisplayType.Value;

                if (value is "string | Guid" && schemaValue.AllowedValues?.Any() == true) // Guids are special as they are send as strings in the allowed values but needs conversion
                {
                    schemaValue.AllowedValues = schemaValue.AllowedValues.ToDictionary(e => e.Key, e => Guid.Parse(e.Value.ToString()) as object);
                }
            }
            else if (value is JArray jArr)
            {
                schemaValue.DisplayType = SchemaEditDisplayType.List;
                schemaValue.Children.AddRange(FromObjectSchema(schema, jArr.ElementAt(0).ToObject<IDictionary<string, object>>(), valueName + ".", valueStore));
            }
            else if (value is JObject jobj)
            {
                schemaValue.DisplayType = SchemaEditDisplayType.Object;
                schemaValue.Children.AddRange(FromObjectSchema(schema, jobj.ToObject<IDictionary<string, object>>(), valueName + ".", valueStore));
            }
            values.Add(schemaValue);
        }

        return values;
    }

    public string GetHtmlInputType()
    {
        var schemaType = SchemaType;
        if (schemaType is JObject || schemaType is JArray)
        {
            return "object";
        }

        if (schemaType is JValue jValue)
        {
            schemaType = jValue.ToString();
        }

        switch (schemaType.ToString().ToLower())
        {
            case "string":
                return "text";
            case "number":
                return "number";
            case "boolean":
                return "checkbox";
            case "string | date":
                return "date";
            case "string | password":
                return "password";
        }

        return "object";
    }
}

public enum SchemaEditDisplayType
{
    Value,
    List,
    Object
}