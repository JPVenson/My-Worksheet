using System.Collections.Generic;
using MyWorksheet.Private.Models.ObjectSchema;

namespace MyWorksheet.Public.Models.ObjectSchema;

public interface IObjectSchemaInfo
{
    IDictionary<string, JsonSchemaProperty> Properties { get; set; }
    IDictionary<string, IFunctionInfo> Functions { get; set; }
    IDictionary<string, JsonSchema> References { get; set; }
}


public class JsonSchemaProperty
{
    public string Type { get; set; }

    public string Name { get; set; }

    public IDictionary<string, object> AllowedValues { get; set; }

    public string Comment { get; set; }

    public object Default { get; set; }

    public ValueValidator Validator { get; set; }

    public bool IsAnonymousType { get; set; }

    public bool IsListType { get; set; }

    public bool IsOptional { get; set; }
}
