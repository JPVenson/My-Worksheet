using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace MyWorksheet.Webpage.Helper;

public static class ObjectToDictionary
{
    public static T FromJsonElements<T>(this IDictionary<string, string> source) where T : class, new()
    {
        var obj = new T();
        foreach (var o in source)
        {
            SetValueOnPath(obj, o.Key, o.Value);
        }
        return obj;
    }

    public static object FromJsonElements(this IDictionary<string, string> source, Type type)
    {
        var obj = Activator.CreateInstance(type);
        foreach (var o in source)
        {
            SetValueOnPath(obj, o.Key, o.Value);
        }
        return obj;
    }

    public static IDictionary<string, object> Expand(this IDictionary<string, object> source)
    {
        var nDict = new Dictionary<string, object>();
        foreach (var sourceElement in source)
        {
            SetValueOnPath(nDict, sourceElement.Key, sourceElement.Value);
        }

        return nDict;
    }

    private static void SetValueOnPath(object source, string path, object value)
    {
        var pathParts = path.Split('.');
        var typeInfo = source.GetType();
        PropertyInfo dbPropertyInfoCache;
        foreach (var pathPart in pathParts.Take(pathParts.Length - 1))
        {
            if (pathPart.StartsWith("[") && pathPart.EndsWith("]"))
            {
                var nPathPart = pathPart.Trim('[', ']');
                if (!(source is IList listSource))
                {
                    throw new InvalidOperationException($"You cannot set the property '{path}' on part '{pathPart}' as the object there is not a list as expected");
                }

                if (!int.TryParse(nPathPart, out var indexOfPart))
                {
                    throw new InvalidOperationException($"You cannot set the property '{path}' on part '{pathPart}' as the indexer expected is not an int");
                }

                source = listSource[indexOfPart];
                typeInfo = source.GetType();
            }
            else if (source is IDictionary<string, object> sourceDict)
            {
                if (sourceDict.TryGetValue(pathPart, out source))
                {
                    if (!(source is IDictionary<string, object>))
                    {
                        throw new InvalidOperationException($"You cannot set the property '{path}' on part '{pathPart}' to both a value and an object");
                    }
                }
                else
                {
                    sourceDict[pathPart] = source = new Dictionary<string, object>();
                }
            }
            else
            {
                dbPropertyInfoCache = typeInfo.GetProperty(pathPart);
                source = dbPropertyInfoCache.GetValue(source);
                if (source != null)
                {
                    typeInfo = source.GetType();
                }
                else
                {
                    source = Activator.CreateInstance(dbPropertyInfoCache.PropertyType);
                    dbPropertyInfoCache.SetValue(source, null);
                }
            }
        }

        var propertyName = pathParts.Last();
        if (source is IList sourceList)
        {
            if (!(propertyName.StartsWith("[") && propertyName.EndsWith("]")))
            {
                throw new InvalidOperationException($"You cannot set the property '{path}' on part '{propertyName}' as the object there is not a list as expected");
            }

            var nPathPart = propertyName.Trim('[', ']');

            if (!int.TryParse(nPathPart, out var indexOfPart))
            {
                throw new InvalidOperationException($"You cannot set the property '{path}' on part '{propertyName}' as the indexer expected is not an int");
            }

            //this can only be the case on the first item as we then need to create a new instance of that type that is in the value
            var type = Type.GetType(value.ToString());
            if (type == null)
            {
                throw new InvalidOperationException($"The type '{value}' could not be found");
            }

            var instance = Activator.CreateInstance(type);
            sourceList.Insert(indexOfPart, instance);
        }
        else if (source is IDictionary<string, object> objects)
        {
            objects[propertyName] = value;
        }
        else
        {
            dbPropertyInfoCache = typeInfo.GetProperty(propertyName);
            if (value.GetType() != dbPropertyInfoCache.PropertyType)
            {
                var type = Nullable.GetUnderlyingType(dbPropertyInfoCache.PropertyType);
                if (type == null)
                {
                    type = dbPropertyInfoCache.PropertyType;
                }

                value = Convert.ChangeType(value, type);
            }
            dbPropertyInfoCache.SetValue(source, value);
        }
    }

    public static IDictionary<string, object> ToJsonElements(this object source)
    {
        var result = new Dictionary<string, object>();

        if (source is JObject jObject)
        {
            IEnumerable<JToken> jTokens = jObject.Descendants().Where(p => p.Count() == 0);
            source = jTokens.Aggregate(new Dictionary<string, object>(), (properties, jToken) =>
            {
                properties.Add(jToken.Path, jToken.ToString());
                return properties;
            });
        }

        if (source is IDictionary<string, object> objects)
        {
            foreach (var keyVal in objects)
            {
                if (keyVal.Value == null)
                {
                    result.Add(keyVal.Key, null);
                    continue;
                }

                var propType = keyVal.Value.GetType();
                if (propType.IsPrimitive || propType == typeof(string))
                {
                    result.Add(keyVal.Key, keyVal.Value);
                }
                else
                {
                    var elements = ToJsonElements(keyVal.Value);
                    foreach (var listItem in elements)
                    {
                        result.Add(keyVal.Key + $"." + listItem.Key, listItem.Value);
                    }
                }
            }
        }
        //list support
        else if (!(source is string) && (source is IEnumerable listOfObjects))
        {
            var counter = 0;
            foreach (var objInEnumerable in listOfObjects)
            {
                var elements = ToJsonElements(objInEnumerable);
                result.Add($"[{counter}]", objInEnumerable.GetType().AssemblyQualifiedName);
                foreach (var listItem in elements)
                {
                    result.Add($"[{counter}]." + listItem.Key, listItem.Value);
                }

                counter++;
            }
        }
        else
        {
            var sourceType = source.GetType();

            foreach (var prop in sourceType.GetProperties())
            {
                var val = prop.GetValue(source);

                if (val == null)
                {
                    result.Add(prop.Name, null);
                    continue;
                }

                var propType = val.GetType();
                if (propType.IsPrimitive || propType == typeof(string))
                {
                    result.Add(prop.Name, val);
                }
                else
                {
                    var elements = ToJsonElements(val);
                    foreach (var listItem in elements)
                    {
                        result.Add(prop.Name + $"." + listItem.Key, listItem.Value);
                    }
                }
            }
        }

        return result;
    }
}