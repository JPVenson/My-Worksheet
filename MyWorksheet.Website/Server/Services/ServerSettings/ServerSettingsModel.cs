using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public class ServerSettingsModel : IServerSettingsModel
{
    public ServerSettingsModel(string name) : this()
    {
        Name = name;
    }

    public ServerSettingsModel()
    {
        SettingsObjects = new Dictionary<string, ISettingsObject>();
        Childs = new Dictionary<string, IServerSettingsModel>();
    }

    public string Name { get; set; }
    public IDictionary<string, ISettingsObject> SettingsObjects { get; private set; }
    public IDictionary<string, IServerSettingsModel> Childs { get; private set; }
    public string Path { get; set; }

    public ServerSettingsModel CreateChild(ServerSettingsModel value)
    {
        Childs.Add(value.Name, value);
        value.Path = Path + "." + value.Name;
        return value;
    }

    public IServerSettingsModel AddChild(IServerSettingsModel value)
    {
        var child = Childs.FirstOrDefault(f => f.Key == value.Name).Value;

        if (child != null)
        {
            foreach (var settingsModel in value.Childs)
            {
                child.AddChild(settingsModel.Value);
            }

            foreach (var valueSettingsObject in value.SettingsObjects)
            {
                child.SettingsObjects.Add(valueSettingsObject.Key, valueSettingsObject.Value);
            }
        }
        else
        {
            child = value;
            Childs.Add(child.Name, child);
        }
        child.Path = Path + "." + child.Name;
        return child;
    }

    public IEnumerable<KeyValuePair<string, object>> CompileFlat()
    {
        var settings = new List<KeyValuePair<string, object>>();

        foreach (var settingsObject in SettingsObjects)
        {
            settings.Add(new KeyValuePair<string, object>(settingsObject.Key, settingsObject.Value.Value));
        }

        foreach (var serverSettingsModel in Childs)
        {
            foreach (var settingsModel in serverSettingsModel.Value.CompileFlat())
            {
                settings.Add(new KeyValuePair<string, object>(serverSettingsModel.Key + "." + settingsModel.Key, settingsModel.Value));
            }
        }

        return settings;
    }

    public object this[string path]
    {
        get
        {
            IServerSettingsModel instance = this;
            var pathParts = path.Split(".");
            foreach (var pathPart in pathParts.Take(pathParts.Length - 1))
            {
                instance = instance?.Childs[pathPart];
            }

            return instance?.SettingsObjects[pathParts.Last()]?.Value;
        }
    }

    public ServerSettingsModel AddSetting(ISettingsObject child)
    {
        SettingsObjects.Add(child.Name, child);
        return this;
    }

    public ServerSettingsModel AddSetting(string name, object values)
    {
        SettingsObjects.Add(name, new SettingsObject() { Name = name, Value = values, Path = name });
        return this;
    }

    public ServerSettingsModel AddSetting(string name, Func<object> values)
    {
        SettingsObjects.Add(name, new DynamicSettingsObject(values, true) { Name = name, Path = name });
        return this;
    }
}

public interface ISettingsObject<TValue> : ISettingsObject
{
    new TValue Value { get; set; }
}