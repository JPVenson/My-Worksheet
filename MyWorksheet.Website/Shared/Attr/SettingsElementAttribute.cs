using System;

namespace MyWorksheet.Public.Models.Attr;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SettingsElementAttribute : Attribute
{
    public string Name { get; }

    public SettingsElementAttribute(string name)
    {
        Name = name;
    }
}