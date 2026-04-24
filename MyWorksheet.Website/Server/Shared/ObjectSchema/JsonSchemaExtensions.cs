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
        // foreach (var schemaPart in schema.Schema)
        // {
        //     var pathInfo = WalkPath(toType, schemaPart.Key, Private.Models.ObjectSchema.JsonSchema.Delimiter);
        //     if (!pathInfo.Item1 || pathInfo.Item2 == null)
        //     {
        //         if (Private.Models.ObjectSchema.JsonSchema.IsOptional(schemaPart.Key))
        //         {
        //             continue;
        //         }

        //         result.Add(schemaPart.Key, SchemaValidationError.Missing);
        //         continue;
        //     }

        //     if (schemaPart.Value is string)
        //     {
        //         var mustType = JsonHelper.TypeCsToTsMapping.FirstOrDefault(e => e.Value == schemaPart.Value.ToString());
        //         var tsType = JsonHelper.TranslateCsToTs(pathInfo.Item2.GetType());
        //         var isTsType = JsonHelper.TypeCsToTsMapping.FirstOrDefault(e => e.Value == tsType);

        //         if (!mustType.Value.Split('|').Any(e => isTsType.Value.Equals(e.Trim())) && !mustType.Value.Equals(isTsType.Value))
        //         {
        //             result.Add(schemaPart.Key, SchemaValidationError.WrongType);
        //         }
        //     }
        // }
        // foreach (var unexpectedValues in schema.CheckForUnexpected(null, toType))
        // {
        //     result.Add(unexpectedValues, SchemaValidationError.Unexpected);
        // }

        return result;
    }

    public static IDictionary<string, object> MapToSchema(IDictionary<string, string> values, IObjectSchema schema,
        string delimiter)
    {
        var convertedData = new Dictionary<string, object>();
        foreach (var flatSchemaItem in values)
        {
            var infoAt = Private.Models.ObjectSchema.JsonSchema.WalkPath(schema, flatSchemaItem.Key, delimiter);
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
                if (infoAt.Item2.Type.TypeName != "string")
                {
                    var translateTsToCs = JsonHelper.TranslateTsToCs(infoAt.Item2.Type.TypeName);
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

    private static JsonSchema BuildObjectSchema(JsonSchema map,
        JsonSchema rootSchema,
        Type type,
        object typeValue,
        IDictionary<Type, (JsonSchema, object)> buildStack,
        bool withFunctions = false)
    {
        var propsInOrginal = new List<object>();
        if (typeof(IDictionary<string, object>).IsAssignableFrom(type))
        {
            foreach (var propPart in (typeValue as IDictionary<string, object>))
            {
                propsInOrginal.Add(new PropDescriptor(propPart.Key, o => propPart.Value, propPart.Value.GetType(),
                    propPart.Key, false, propPart.Key));
            }
        }
        else
        {
            propsInOrginal.AddRange(type
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
                foreach (var methodInfo in type
                             .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             .Where(e => e.DeclaringType == type && !e.IsSpecialName))
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

        // foreach (var methodPropDescriptor in propsInOrginal.OfType<MethodPropDescriptor>())
        // {
        //     string funcHeader = "(";
        //     if (methodPropDescriptor.ArgumentTypes.Any())
        //     {
        //         funcHeader += methodPropDescriptor
        //             .ArgumentTypes
        //             .Select(e => (e.Name + ": " + JsonHelper.TranslateAnyCsToTs(e.ParameterType)))
        //             .Aggregate((e, f) => e + ", " + f);

        //     }

        //     funcHeader += ") => " + JsonHelper.TranslateAnyCsToTs(methodPropDescriptor.ReturnType);
        //     funcHeader, methodPropDescriptor.Comment, methodPropDescriptor.Name
        //     map.Functions.Add(methodPropDescriptor.Name, new()
        //     {

        //     });
        // }

        foreach (var prop in propsInOrginal.OfType<PropDescriptor>())
        {
            var propType = prop.Type;
            var nullableType = Nullable.GetUnderlyingType(propType);
            var isNullable = nullableType != null || prop.Name.EndsWith("!") || prop.OptionalByAttribute;

            propType = nullableType ?? propType;
            object val = null;
            if (typeValue != null)
            {
                val = prop.GetValue(typeValue);
            }

            if (typeValue != null)
            {
                propType = val?.GetType() ?? propType;
            }

            if (typeof(IDictionary).IsAssignableFrom(propType) || propType.Namespace == null)
            {
                var dynName = "dynamicType_" + map.References.Count;
                map.References[dynName] = BuildObjectSchema(new(), rootSchema, propType, val, buildStack, withFunctions);
                map.Properties.Add(prop.Name, new()
                {
                    Name = prop.DisplayName,
                    Type = new()
                    {
                        IsAnonymousType = true,
                        TypeName = dynName
                    },
                    IsOptional = isNullable,
                    Validator = prop.ValueValidator
                });
            }
            else if (typeof(IEnumerable).IsAssignableFrom(propType) && propType != typeof(string))
            {
                Type innerType;
                if (propType.IsArray)
                {
                    innerType = propType.GetElementType();
                }
                else
                {
                    innerType = propType.GetGenericArguments().First();
                }
                var primitiveTypeName = JsonHelper.TranslateCsToTs(innerType);

                if (primitiveTypeName is null && !rootSchema.References.TryGetValue(innerType.Name, out var jsonSchema) && !buildStack.ContainsKey(innerType))
                {
                    buildStack.Add(innerType, (new JsonSchema(), (typeValue as IEnumerable)?.OfType<object>().FirstOrDefault()));
                }
                map.Properties.Add(prop.Name, new()
                {
                    Name = prop.DisplayName,
                    Type = new()
                    {
                        TypeName = primitiveTypeName ?? innerType.Name,
                        IsListType = true
                    },
                    IsOptional = isNullable,
                    Validator = prop.ValueValidator
                });
            }
            else
            {
                var ttcs = JsonHelper.TranslateCsToTs(propType);
                if (ttcs == null)
                {
                    if (!rootSchema.References.TryGetValue(propType.Name, out var jsonSchema) && !buildStack.ContainsKey(propType))
                    {
                        buildStack.Add(propType, (new JsonSchema(), typeValue != null ? val : null));
                    }
                    map.Properties.Add(prop.Name, new()
                    {
                        Name = prop.DisplayName,
                        Type = new()
                        {
                            TypeName = propType.Name,
                        },
                        Comment = prop.Comment,
                        IsOptional = isNullable,
                        Validator = prop.ValueValidator
                    });
                }
                else
                {
                    var name = prop.DbName;
                    map.Properties.Add(prop.Name, new()
                    {
                        Name = prop.DisplayName,
                        Type = new()
                        {
                            TypeName = ttcs,
                            IsValueType = true,
                            IsStaticCompositType = propType.IsEnum
                        },
                        Comment = prop.Comment,
                        IsOptional = isNullable || propType == typeof(bool),
                        Validator = prop.ValueValidator,
                        Default = val
                    });
                }
            }
        }
        return map;
    }

    public static JsonSchema JsonSchema(Type orignal, object objValue, bool withFunctions = false)
    {
        var rootSchema = new JsonSchema();
        var buildStack = new Dictionary<Type, (JsonSchema, object)>();
        buildStack.Add(orignal, (rootSchema, objValue));
        while (buildStack.Count > 0)
        {
            var item = buildStack.First();
            buildStack.Remove(item.Key);

            rootSchema.References[item.Key.Name] = item.Value.Item1;
            BuildObjectSchema(item.Value.Item1, rootSchema, item.Key, item.Value.Item2, buildStack, withFunctions);
        }

        return rootSchema;
    }

    public static IEnumerable<string> CheckForUnexpected(this IObjectSchema schema, string path, object toType)
    {
        yield break;
        // if (toType == null)
        // {
        //     yield break;
        // }

        // if (toType is IDictionary<string, object>)
        // {
        //     foreach (var kv in toType as IDictionary<string, object>)
        //     {
        //         var p = path == null ? kv.Key : path + kv.Key;
        //         if (!schema.Schema.ContainsKey(p) && !schema.Schema.ContainsKey(p + "!"))
        //         {
        //             yield return p;
        //         }
        //         foreach (var subErr in schema.CheckForUnexpected(p, kv.Value))
        //         {
        //             yield return subErr;
        //         }
        //     }
        // }
        // else
        // {
        //     var objType = toType.GetType();
        //     if (objType.IsPrimitive || toType is string || objType.Assembly == typeof(string).Assembly)
        //     {
        //         yield break;
        //     }

        //     foreach (var prop in toType.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        //     {
        //         var p = path == null ? prop.Name : path + prop.Name;
        //         if (!schema.Schema.ContainsKey(p))
        //         {
        //             yield return p;
        //         }

        //         foreach (var subErr in schema.CheckForUnexpected(p, prop.GetMethod.Invoke(toType, null)))
        //         {
        //             yield return subErr;
        //         }
        //     }
        // }
    }
}