using System;

namespace MyWorksheet.Webpage.Services.Templating.Text.FormatterFramework;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class MorestachioFormatterAttribute : Attribute
{
    public MorestachioFormatterAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ReturnHint { get; set; }
    public Type OutputType { get; set; }
}