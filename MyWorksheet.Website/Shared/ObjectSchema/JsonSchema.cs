using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Private.Models.ObjectSchema;

public class JsonSchema : IObjectSchema
{
    public JsonSchema()
    {
    }

    public static readonly string Delimiter = ".";

    public static readonly JsonSchema EmptyNotNull = new JsonSchema();

    public IDictionary<string, JsonSchemaProperty> Properties { get; set; } = new Dictionary<string, JsonSchemaProperty>();
    public IDictionary<string, IFunctionInfo> Functions { get; set; } = new Dictionary<string, IFunctionInfo>();
    public IDictionary<string, JsonSchema> References { get; set; } = new Dictionary<string, JsonSchema>();


    public static bool IsOptional(string name)
    {
        return name.EndsWith("!");
    }

    public JsonSchema ExtendDefault(string path, object value)
    {
        WalkPath(this, path, Delimiter).Item2.Default = value;
        return this;
    }

    public JsonSchema ExtendAllowedValues(string path, IDictionary<string, object> value)
    {
        WalkPath(this, path, Delimiter).Item2.AllowedValues = value;
        return this;
    }

    public JsonSchema ExtendComments(IDictionary<string, string> comments)
    {
        foreach (var item in comments)
        {
            WalkPath(this, item.Key, Delimiter).Item2.Comment = item.Value;
        }

        return this;
    }

    public static Tuple<bool, JsonSchemaProperty> WalkPath(IObjectSchemaInfo source, string path, string delimiter)
    {
        var pathParts = path.Split(new string[] { delimiter }, StringSplitOptions.None);
        var rootSource = source;
        foreach (var item in pathParts.Take(pathParts.Length - 1))
        {
            if (!source.Properties.TryGetValue(item, out var innerProp))
            {
                return new Tuple<bool, JsonSchemaProperty>(false, null);
            }

            if (!rootSource.References.TryGetValue(innerProp.Type.TypeName, out var innerSchema))
            {
                return new Tuple<bool, JsonSchemaProperty>(false, null);
            }
            source = innerSchema;
        }
        if (!source.Properties.TryGetValue(pathParts[^1], out var prop))
        {
            return new Tuple<bool, JsonSchemaProperty>(false, null);
        }
        return new Tuple<bool, JsonSchemaProperty>(true, prop);
    }

    // public void Add(string propKey, string displayName, IObjectSchema innerSchema)
    // {
    //     Names[propKey] = displayName;
    //     foreach (var schemaPart in innerSchema.Comments.ToArray())
    //     {
    //         Comments[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }

    //     foreach (var schemaPart in innerSchema.Names.ToArray())
    //     {
    //         Names[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }

    //     foreach (var schemaPart in innerSchema.Defaults.ToArray())
    //     {
    //         Defaults[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }

    //     foreach (var schemaPart in innerSchema.AllowedValues.ToArray())
    //     {
    //         AllowedValues[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }

    //     var schema = Schema;
    //     var pathParts = propKey.Split(new[] { Delimiter }, StringSplitOptions.None);

    //     foreach (var pathPart in pathParts.Take(pathParts.Length - 1))
    //     {
    //         if (!schema.TryGetValue(pathPart, out var subSchema))
    //         {
    //             schema[pathPart] = subSchema = new Dictionary<string, object>();
    //         }

    //         if (!(subSchema is IDictionary<string, object>))
    //         {
    //             throw new InvalidOperationException($"You cannot set the schema for type {propKey} because in level {pathPart} is already a non object located");
    //         }

    //         schema = subSchema as IDictionary<string, object>;
    //     }

    //     schema[pathParts.Last()] = innerSchema.Schema;
    // }

    // public void Add(string name, string schemaType, string comment, string displayName, object defaultValue = null)
    // {
    //     if (defaultValue != null)
    //     {
    //         Defaults[name] = defaultValue;
    //     }
    //     Schema[name] = schemaType;
    //     Comments[name] = comment;
    //     Names[name] = displayName;
    // }

    // public void AddList(string propKey, string displayName, IObjectSchema innerSchema)
    // {
    //     Names[propKey] = displayName;
    //     foreach (var schemaPart in innerSchema.Comments.ToArray())
    //     {
    //         Comments[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }
    //     foreach (var schemaPart in innerSchema.Defaults.ToArray())
    //     {
    //         Defaults[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }
    //     foreach (var schemaPart in innerSchema.Names.ToArray())
    //     {
    //         Names[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
    //     }
    //     Schema[propKey] = new List<IDictionary<string, object>>() { innerSchema.Schema };
    // }
}