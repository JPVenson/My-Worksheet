using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public interface IServerSettingsModel<TSettingsObject> : IServerSettingsModel where TSettingsObject : ISettingsObject
{
    new IDictionary<string, TSettingsObject> SettingsObjects { get; }
}

public interface IServerSettingsModel
{
    string Name { get; set; }
    IDictionary<string, ISettingsObject> SettingsObjects { get; }
    IDictionary<string, IServerSettingsModel> Childs { get; }
    string Path { get; set; }
    IServerSettingsModel AddChild(IServerSettingsModel value);

    IEnumerable<KeyValuePair<string, object>> CompileFlat();

    object this[string path]
    {
        get;
    }
}