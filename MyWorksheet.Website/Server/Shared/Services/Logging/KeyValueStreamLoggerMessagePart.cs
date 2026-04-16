namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class KeyValueStreamLoggerMessagePart : StreamLoggerMessageParts
{
    public KeyValueStreamLoggerMessagePart(string name, string keyName, string valueName)
    {
        KeyName = keyName;
        ValueName = valueName;
        Name = name;
    }

    public string KeyName { get; private set; }
    public string ValueName { get; private set; }

    public override string ToString()
    {
        return "{" + KeyName + ", " + ValueName + "}";
    }
}
