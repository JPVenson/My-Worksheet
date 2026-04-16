namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class StreamLoggerMessageParts
{
    public StreamLoggerMessageParts(string name, string templateValue)
    {
        Name = name;
        TemplateValue = templateValue;
    }

    protected StreamLoggerMessageParts()
    {

    }

    public string Name { get; protected set; }
    public string TemplateValue { get; protected set; }

    public override string ToString()
    {
        return "{" + TemplateValue + "}";
    }
}
