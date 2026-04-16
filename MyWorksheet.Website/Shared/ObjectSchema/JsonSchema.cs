using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Private.Models.ObjectSchema
{
    public class JsonSchema : IObjectSchema
    {
        public JsonSchema()
        {
            Schema = new Dictionary<string, object>();
            Defaults = new Dictionary<string, object>();
            Functions = new Dictionary<string, IFunctionInfo>();
            Comments = new Dictionary<string, string>();
            Names = new Dictionary<string, string>();
            Validators = new Dictionary<string, ValueValidator>();
            AllowedValues = new Dictionary<string, IDictionary<string, object>>();
        }

        private JsonSchema(bool readOnly)
        {
            Schema = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
            Defaults = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
            Comments = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
            Names = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
            Validators = new ReadOnlyDictionary<string, ValueValidator>(new Dictionary<string, ValueValidator>());
            AllowedValues = new ReadOnlyDictionary<string, IDictionary<string, object>>(new Dictionary<string, IDictionary<string, object>>());
        }

        public static readonly JsonSchema EmptyNotNull = new JsonSchema(true);

        public static readonly string Delimiter = ".";

        public IDictionary<string, string> Names { get; set; }
        public IDictionary<string, IFunctionInfo> Functions { get; set; }
        public IDictionary<string, object> Schema { get; set; }
        public IDictionary<string, object> Defaults { get; set; }
        public IDictionary<string, IDictionary<string, object>> AllowedValues { get; set; }
        public IDictionary<string, string> Comments { get; set; }
        public IDictionary<string, ValueValidator> Validators { get; set; }

        public static bool IsOptional(string name)
        {
            return name.EndsWith("!");
        }

        public IObjectSchema ExtendComments(IDictionary<string, string> commentsForPath)
        {
            foreach (var commentInPath in commentsForPath)
            {
                ExtendComments(commentInPath.Key, commentInPath.Value);
            }

            return this;
        }

        public JsonSchema ExtendComments(string path, string comment)
        {
            if (Comments.ContainsKey(path + "!"))
            {
                path = path + "!";
            }
            Comments[path] = comment;
            return this;
        }

        public JsonSchema ExtendAllowedValues(IDictionary<string, IDictionary<string, object>> allowedValues)
        {
            foreach (var commentInPath in allowedValues)
            {
                ExtendAllowedValues(commentInPath.Key, commentInPath.Value);
            }

            return this;
        }

        public IObjectSchema ExtendAllowedValues(string path, IDictionary<string, object> value)
        {
            if (Comments.ContainsKey(path + "!"))
            {
                path = path + "!";
            }
            AllowedValues[path] = value;
            return this;
        }

        public JsonSchema ExtendDefault(IDictionary<string, object> allowedValues)
        {
            foreach (var commentInPath in allowedValues)
            {
                ExtendDefault(commentInPath.Key, commentInPath.Value);
            }

            return this;
        }

        public JsonSchema ExtendDefault(string path, object value)
        {
            if (Comments.ContainsKey(path + "!"))
            {
                path = path + "!";
            }
            Defaults[path] = value;
            return this;
        }

        public void Add(string propKey, string displayName, IObjectSchema innerSchema)
        {
            Names[propKey] = displayName;
            foreach (var schemaPart in innerSchema.Comments)
            {
                Comments[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }

            foreach (var schemaPart in innerSchema.Names)
            {
                Names[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }

            foreach (var schemaPart in innerSchema.Defaults)
            {
                Defaults[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }

            foreach (var schemaPart in innerSchema.AllowedValues)
            {
                AllowedValues[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }

            var schema = Schema;
            var pathParts = propKey.Split(new[] { Delimiter }, StringSplitOptions.None);

            foreach (var pathPart in pathParts.Take(pathParts.Length - 1))
            {
                if (!schema.TryGetValue(pathPart, out var subSchema))
                {
                    schema[pathPart] = subSchema = new Dictionary<string, object>();
                }

                if (!(subSchema is IDictionary<string, object>))
                {
                    throw new InvalidOperationException($"You cannot set the schema for type {propKey} because in level {pathPart} is already a non object located");
                }

                schema = subSchema as IDictionary<string, object>;
            }

            schema[pathParts.Last()] = innerSchema.Schema;
        }

        public void Add(string name, string schemaType, string comment, string displayName, object defaultValue = null)
        {
            if (defaultValue != null)
            {
                Defaults[name] = defaultValue;
            }
            Schema[name] = schemaType;
            Comments[name] = comment;
            Names[name] = displayName;
        }

        public void AddList(string propKey, string displayName, IObjectSchema innerSchema)
        {
            Names[propKey] = displayName;
            foreach (var schemaPart in innerSchema.Comments)
            {
                Comments[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }
            foreach (var schemaPart in innerSchema.Defaults)
            {
                Defaults[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }
            foreach (var schemaPart in innerSchema.Names)
            {
                Names[propKey + Delimiter + schemaPart.Key] = schemaPart.Value;
            }
            Schema[propKey] = new List<IDictionary<string, object>>() { innerSchema.Schema };
        }
    }
}