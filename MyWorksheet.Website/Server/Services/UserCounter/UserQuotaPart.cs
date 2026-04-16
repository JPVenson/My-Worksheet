namespace MyWorksheet.Website.Server.Services.UserCounter;

public class UserQuotaPart
{
    public UserQuotaPart(string name, int typeKey, string configName = null)
    {
        Name = name;
        TypeKey = typeKey;
        ConfigName = configName ?? name;
    }

    public string Name { get; private set; }
    public int TypeKey { get; private set; }
    public string ConfigName { get; private set; }
}