using System;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public class InputDescription
{
    public InputDescription(string description, Type outputType, string example)
    {
        Description = description;
        OutputType = outputType;
        Example = example;
    }

    public string Description { get; private set; }
    public string Example { get; private set; }
    public Type OutputType { get; private set; }
}