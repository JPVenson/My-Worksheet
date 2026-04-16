using System;

namespace MyWorksheet.Webpage.Services.Templating.Text.FormatterFramework;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class MorestachioFormatterInputAttribute : Attribute
{
    public string Description { get; }
    public string Example { get; set; }
    public Type OutputType { get; set; }
    public string Output { get; set; }

    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public MorestachioFormatterInputAttribute(string description)
    {
        Description = description;
    }
}