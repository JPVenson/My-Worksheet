using System.Collections.Generic;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Private.Models.ObjectSchema
{
    public interface IObjectSchema : IObjectSchemaInfo
    {
        void Add(string propKey, string displayName, IObjectSchema innerSchema);
        void AddList(string propKey, string displayName, IObjectSchema innerSchema);
        IObjectSchema ExtendComments(IDictionary<string, string> commentsForPath);
        IObjectSchema ExtendAllowedValues(string path, IDictionary<string, object> value);
    }
}