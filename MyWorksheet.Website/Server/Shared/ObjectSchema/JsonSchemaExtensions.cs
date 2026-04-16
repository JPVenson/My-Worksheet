using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Shared.ObjectSchema;

public static class JsonSchemaExtensions
{
    public static IDictionary<string, SchemaValidationError> Validate(this IObjectSchema schema, object toType)
    {
        var result = new Dictionary<string, SchemaValidationError>();
        foreach (var schemaPart in schema.Schema)
        {
            var pathInfo = WalkPath(toType, schemaPart.Key, Private.Models.ObjectSchema.JsonSchema.Delimiter);
            if (!pathInfo.Item1 || pathInfo.Item2 == null)
            {
                if (Private.Models.ObjectSchema.JsonSchema.IsOptional(schemaPart.Key))
                {
                    continue;
                }

                result.Add(schemaPart.Key, SchemaValidationError.Missing);
                continue;
            }

            if (schemaPart.Value is string)
            {
                var mustType = JsonHelper.TypeCsToTsMapping.FirstOrDefault(e => e.Value == schemaPart.Value.ToString());
                var tsType = JsonHelper.TranslateCsToTs(pathInfo.Item2.GetType());
                var isTsType = JsonHelper.TypeCsToTsMapping.FirstOrDefault(e => e.Value == tsType);

                if (!mustType.Value.Split('|').Any(e => isTsType.Value.Equals(e.Trim())) && !mustType.Value.Equals(isTsType.Value))
                {
                    result.Add(schemaPart.Key, SchemaValidationError.WrongType);
                }
            }
        }
        foreach (var unexpectedValues in schema.CheckForUnexpected(null, toType))
        {
            result.Add(unexpectedValues, SchemaValidationError.Unexpected);
        }

        return result;
    }

    public static Tuple<bool, object> WalkPath(object source, string path, string delimiter)
    {
        var pathParts = path.Split(new string[] { delimiter }, StringSplitOptions.None);
        var nPath = pathParts.First();
        object hasPropInfo;
        if (source is IDictionary)
        {
            var sourceDic = source as IDictionary;
            if (!sourceDic.Contains(nPath))
            {
                return new Tuple<bool, object>(false, null);
            }
            hasPropInfo = sourceDic[nPath];
        }
        else
        {
            var property = source.GetType()
                .GetProperty(nPath);
            if (property == null)
            {
                return new Tuple<bool, object>(false, null);
            }
            hasPropInfo = property.GetMethod.Invoke(source, null);
        }

        if (pathParts.Length == 1)
        {
            return new Tuple<bool, object>(true, hasPropInfo);
        }
        if (hasPropInfo == null && pathParts.Length > 1)
        {
            return new Tuple<bool, object>(false, null);
        }
        return WalkPath(hasPropInfo, pathParts.Skip(1).Aggregate((e, f) => e + delimiter + f), delimiter);
    }

    public static IDictionary<string, object> MapToSchema(IDictionary<string, string> values, IObjectSchema schema,
        string delimiter)
    {
        var convertedData = new Dictionary<string, object>();
        foreach (var flatSchemaItem in values)
        {
            var infoAt = WalkPath(schema.Schema, flatSchemaItem.Key, delimiter);
            if (!infoAt.Item1)
            {
                continue;
            }
            object changeType;

            if (string.IsNullOrWhiteSpace(flatSchemaItem.Value))
            {
                changeType = null;
            }
            else
            {
                if (infoAt.Item2.ToString() != "string")
                {
                    var translateTsToCs = JsonHelper.TranslateTsToCs(infoAt.Item2.ToString());
                    if (translateTsToCs == typeof(Guid))
                    {
                        changeType = Guid.Parse(flatSchemaItem.Value);
                    }
                    else
                    {
                        changeType = Convert.ChangeType(flatSchemaItem.Value, translateTsToCs);
                    }
                }
                else
                {
                    changeType = flatSchemaItem.Value;
                }
            }

            convertedData.Add(flatSchemaItem.Key, changeType);
        }

        return convertedData;
    }

    public static JsonSchema JsonSchema(object orignal, bool withFunctions = false)
    {
        if (orignal is Type)
        {
            return JsonSchema(orignal as Type, (object)null, withFunctions);
        }
        return JsonSchema(orignal.GetType(), orignal, withFunctions);
    }

    private class PropDescriptor
    {
        private readonly Func<object, object> _getValue;

        public PropDescriptor(string name,
            Func<object, object> getValue,
            Type type,
            string dbName,
            bool optionalByAttribute,
            string displayName,
            ValueValidator validator = null,
            string comment = null)
        {
            _getValue = getValue;
            Type = type;
            DbName = dbName;
            OptionalByAttribute = optionalByAttribute;
            Comment = comment;
            Name = name;
            ValueValidator = validator;
            DisplayName = displayName;
        }

        public string Name { get; private set; }
        public string DbName { get; private set; }
        public Type Type { get; private set; }
        public string Comment { get; private set; }
        public bool OptionalByAttribute { get; private set; }
        public ValueValidator ValueValidator { get; private set; }
        public string DisplayName { get; private set; }

        public object GetValue(object source)
        {
            return _getValue(source);
        }
    }

    private class MethodPropDescriptor
    {
        public string Name { get; set; }
        public Type ReturnType { get; set; }
        public ParameterInfo[] ArgumentTypes { get; set; }
        public string Comment { get; set; }
    }

    public static JsonSchema JsonSchema(Type orignal, object objValue, bool withFunctions = false)
    {
        var map = new JsonSchema();
        var propsInOrginal = new List<object>();

        if (typeof(IDictionary<string, object>).IsAssignableFrom(orignal))
        {
            foreach (var propPart in (objValue as IDictionary<string, object>))
            {
                propsInOrginal.Add(new PropDescriptor(propPart.Key, o => propPart.Value, propPart.Value.GetType(),
                    propPart.Key, false, propPart.Key));
            }
        }
        else
        {
            propsInOrginal.AddRange(orignal
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(e => e.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                .Where(e => e.GetMethod != null)
                .Select(e => new PropDescriptor(e.Name, o => e.GetValue(o), e.PropertyType,
                    e.Name,
                    e.GetCustomAttribute<JsonCanBeNullAttribute>() != null,
                    e.GetCustomAttribute<JsonDisplayKeyAttribute>()?.DisplayKey,
                    e.GetCustomAttribute<ValidationAttribute>()?.BuildValidator(),
                    e.GetCustomAttributes<JsonCommentAttribute>().FirstOrDefault()?.CodeComment))
                .ToList());
            if (withFunctions)
            {
                foreach (var methodInfo in orignal
                             .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             .Where(e => e.DeclaringType == orignal && !e.IsSpecialName))
                {
                    propsInOrginal.Add(new MethodPropDescriptor()
                    {
                        Name = methodInfo.Name,
                        ReturnType = methodInfo.ReturnType,
                        ArgumentTypes = methodInfo.GetParameters(),
                        Comment = methodInfo
                            .GetCustomAttributes<JsonCommentAttribute>().FirstOrDefault()?.CodeComment
                    });
                }
            }
        }

        foreach (var methodPropDescriptor in propsInOrginal.OfType<MethodPropDescriptor>())
        {
            string funcHeader = "(";
            if (methodPropDescriptor.ArgumentTypes.Any())
            {
                funcHeader += methodPropDescriptor
                    .ArgumentTypes
                    .Select(e => (e.Name + ": " + JsonHelper.TranslateAnyCsToTs(e.ParameterType)))
                    .Aggregate((e, f) => e + ", " + f);

            }

            funcHeader += ") => " + JsonHelper.TranslateAnyCsToTs(methodPropDescriptor.ReturnType);
            map.Add(methodPropDescriptor.Name, funcHeader, methodPropDescriptor.Comment, methodPropDescriptor.Name);
        }

        foreach (var prop in propsInOrginal.OfType<PropDescriptor>())
        {
            var type = prop.Type;
            var nullableType = Nullable.GetUnderlyingType(type);
            var isNullable = nullableType != null || prop.Name.EndsWith("!") || prop.OptionalByAttribute;

            type = nullableType ?? type;
            object val = null;
            if (objValue != null)
            {
                try
                {
                    val = prop.GetValue(objValue);
                }
                catch (Exception e)
                {
                    throw;
                }

            }

            if (objValue != null)
            {
                type = val?.GetType() ?? type;
            }

            if (typeof(IDictionary).IsAssignableFrom(type) || type.Namespace == null)
            {
                map.Add(prop.Name, prop.DisplayName, JsonSchema(type, val, withFunctions));
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
            {
                JsonSchema innerSchema;
                if (type.IsArray)
                {
                    innerSchema = JsonSchema(type.GetElementType(),
                        (objValue as IEnumerable)?.OfType<object>().FirstOrDefault(), withFunctions);
                }
                else
                {
                    innerSchema = JsonSchema(type.GetGenericArguments().First(),
                        (objValue as IEnumerable)?.OfType<object>().FirstOrDefault(), withFunctions);
                }
                map.AddList(prop.Name, prop.DisplayName, innerSchema);
            }
            else
            {
                var ttcs = JsonHelper.TranslateCsToTs(type);
                if (ttcs == null)
                {
                    map.Add(prop.Name, prop.DisplayName, JsonSchema(type, objValue != null ? val : null, withFunctions));
                    map.Comments[prop.Name] = prop.Comment;
                }
                else
                {
                    var name = prop.DbName;
                    if (isNullable)
                    {
                        name = name.TrimEnd('!') + "!";
                    }
                    map.Add(name, ttcs, prop.Comment, prop.DisplayName, val);
                    if (prop.ValueValidator != null)
                    {
                        map.Validators.Add(name, prop.ValueValidator);
                    }
                }
            }
        }
        return map;
    }

    public static IEnumerable<string> CheckForUnexpected(this IObjectSchema schema, string path, object toType)
    {
        if (toType == null)
        {
            yield break;
        }

        if (toType is IDictionary<string, object>)
        {
            foreach (var kv in toType as IDictionary<string, object>)
            {
                var p = path == null ? kv.Key : path + kv.Key;
                if (!schema.Schema.ContainsKey(p) && !schema.Schema.ContainsKey(p + "!"))
                {
                    yield return p;
                }
                foreach (var subErr in schema.CheckForUnexpected(p, kv.Value))
                {
                    yield return subErr;
                }
            }
        }
        else
        {
            var objType = toType.GetType();
            if (objType.IsPrimitive || toType is string || objType.Assembly == typeof(string).Assembly)
            {
                yield break;
            }

            foreach (var prop in toType.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var p = path == null ? prop.Name : path + prop.Name;
                if (!schema.Schema.ContainsKey(p))
                {
                    yield return p;
                }

                foreach (var subErr in schema.CheckForUnexpected(p, prop.GetMethod.Invoke(toType, null)))
                {
                    yield return subErr;
                }
            }
        }
    }
}