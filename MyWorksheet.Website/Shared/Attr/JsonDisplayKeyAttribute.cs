using System;

namespace MyWorksheet.Public.Models.ObjectSchema;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class JsonDisplayKeyAttribute : Attribute
{
    public JsonDisplayKeyAttribute(string displayKey)
    {
        DisplayKey = displayKey;
    }

    public string DisplayKey { get; private set; }
}