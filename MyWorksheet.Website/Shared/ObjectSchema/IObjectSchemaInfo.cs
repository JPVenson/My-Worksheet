using System.Collections.Generic;

namespace MyWorksheet.Public.Models.ObjectSchema
{
    public interface IObjectSchemaInfo
    {
        IDictionary<string, string> Comments { get; set; }
        IDictionary<string, string> Names { get; set; }
        IDictionary<string, IFunctionInfo> Functions { get; set; }
        IDictionary<string, object> Schema { get; set; }
        IDictionary<string, object> Defaults { get; set; }
        IDictionary<string, IDictionary<string, object>> AllowedValues { get; set; }
    }
}