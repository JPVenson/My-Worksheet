using System;
using System.Reflection;
using Morestachio.Formatter.Framework;

namespace MyWorksheet.Webpage.Services.Templating.Text.FormatterFramework;

public class MorestachioFormatterModel
{
    public MorestachioFormatterModel(string name, string description,
        Type inputType,
        InputDescription[] inputDescription,
        string output,
        MethodInfo function)
    {
        Name = name;
        Description = description;
        InputDescription = inputDescription;
        Output = output;
        Function = function;
        InputType = inputType;
    }

    public MorestachioFormatterModel(string name, string description,
        Type inputType,
        Type outputType,
        InputDescription[] inputDescription,
        string output,
        MethodInfo function)
        : this(name, description, inputType, inputDescription, output, function)
    {
        OutputType = outputType;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public InputDescription[] InputDescription { get; private set; }
    public Type InputType { get; private set; }
    public string Output { get; private set; }
    public Type OutputType { get; private set; }
    public MethodInfo Function { get; private set; }
}