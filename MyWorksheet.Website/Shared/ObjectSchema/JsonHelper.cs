using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWorksheet.Private.Models.ObjectSchema;

public class JsonHelper
{
    static JsonHelper()
    {
        TypeCsToTsMapping = new Dictionary<string, string>();
        TypeCsToTsMapping.Add("String", "string");
        TypeCsToTsMapping.Add("Guid", "string | Guid");
        TypeCsToTsMapping.Add("SecureString", "string | password");
        TypeCsToTsMapping.Add(nameof(DateTime), "string | Date");
        TypeCsToTsMapping.Add(nameof(DateTimeOffset), "string | Date");
        TypeCsToTsMapping.Add("Int64", "number");
        TypeCsToTsMapping.Add("Int32", "number");
        TypeCsToTsMapping.Add("Int16", "number");
        TypeCsToTsMapping.Add("Single", "number");
        TypeCsToTsMapping.Add("Decimal", "number");
        TypeCsToTsMapping.Add("Byte", "number");
        TypeCsToTsMapping.Add("Double", "number");
        TypeCsToTsMapping.Add("Boolean", "boolean");
        TypeCsToTsMapping.Add("Void", "void");
    }

    public static IDictionary<string, object> Jsonify(IDictionary<string, object> values)
    {
        var expandedValues = new Dictionary<string, object>();
        foreach (var value in values)
        {
            var pathParts = value.Key.Split('.');
            var currentValue = expandedValues;
            foreach (var pathPart in pathParts.Take(pathParts.Length - 1))
            {
                if (currentValue.ContainsKey(pathPart))
                {
                    currentValue = currentValue[pathPart] as Dictionary<string, object>;
                    continue;
                }

                currentValue = (currentValue[pathPart] = new Dictionary<string, object>()) as Dictionary<string, object>;
            }

            currentValue[pathParts.Last()] = value.Value;
        }

        return expandedValues;
    }



    public static string ParseJsonSchema(IDictionary<string, object> schema, int level = 1)
    {
        var sb = new StringBuilder();
        var intention = "";
        for (int i = 0; i < level; i++)
        {
            intention += "\t";
        }
        sb.AppendLine("{");
        foreach (var schemaPart in schema)
        {
            sb.Append(intention);
            sb.Append(schemaPart.Key);
            sb.Append(": ");
            if (schemaPart.Value is IDictionary<string, object>)
            {
                sb.Append(ParseJsonSchema((IDictionary<string, object>)schemaPart.Value, level + 1));
            }
            else
            {
                sb.Append(schemaPart.Value.ToString().Replace("\r\n", "\r\n" + intention));
            }

            sb.Append(",");
            sb.AppendLine();
        }
        sb.AppendLine("}");
        return sb.ToString();
    }



    public static IDictionary<string, string> TypeCsToTsMapping { get; set; }

    public static string TranslateCsToTs(Type type)
    {
        if (type.IsEnum)
        {
            return Enum.GetNames(type).Aggregate((e, f) => e + " | " + f);
        }

        if (TypeCsToTsMapping.ContainsKey(type.Name))
        {
            return TypeCsToTsMapping[type.Name];
        }

        return null;
    }

    public static string TranslateAnyCsToTs(Type type)
    {
        var jsLike = TranslateCsToTs(type);

        if (jsLike != null)
        {
            return jsLike;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            var hasGenericArgument = type.GetGenericArguments().FirstOrDefault();
            if (hasGenericArgument != null)
            {
                return "Array<" + TranslateAnyCsToTs(hasGenericArgument) + ">";
            }

            if (type.IsArray)
            {
                hasGenericArgument = type.GetElementType();
                return "Array<" + TranslateAnyCsToTs(hasGenericArgument) + ">";
            }

            return "Array";
        }

        return type.Name;
    }

    public static Type TranslateTsToCs(string jsType)
    {
        var firstOrDefault = TypeCsToTsMapping.FirstOrDefault(e => e.Value.Equals(jsType)).Key;

        if (firstOrDefault == null)
        {
            return null;
        }

        return Type.GetType("System." + firstOrDefault);
    }
}