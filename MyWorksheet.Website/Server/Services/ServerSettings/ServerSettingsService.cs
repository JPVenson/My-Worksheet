using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.Extensions.Configuration;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public static class ServerSettingsExtentions
{
    public static T AsReadOnly<T>(this T model) where T : IServerSettingsModel
    {
        foreach (var modelSettingsObject in model.SettingsObjects)
        {
            modelSettingsObject.Value.ReadOnly = true;
        }

        return model;
    }

    public static T ChildsAsReadOnly<T>(this T model) where T : IServerSettingsModel
    {
        foreach (var modelSettingsObject in model.Childs)
        {
            modelSettingsObject.Value.AsReadOnly();
            modelSettingsObject.Value.ChildsAsReadOnly();
        }

        return model;
    }

    public static IEnumerable<T> AsReadOnly<T>(this IEnumerable<T> model) where T : IServerSettingsModel
    {
        foreach (var modelSettingsObject in model)
        {
            modelSettingsObject.AsReadOnly();
        }

        return model;
    }

    public static IEnumerable<T> ChildsAsReadOnly<T>(this IEnumerable<T> model) where T : IServerSettingsModel
    {
        foreach (var modelSettingsObject in model)
        {
            modelSettingsObject.ChildsAsReadOnly();
        }

        return model;
    }
}

[SingletonService(typeof(IServerSettingsService))]
public class ServerSettingsService : IServerSettingsService
{
    private readonly IAppLogger _logger;

    public ServerSettingsService(IAppLogger logger)
    {
        _logger = logger;
        Root = new ServerSettingsModel("root");
        Delimiter = [".", ":"];
    }

    public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;

    public IServerSettingsModel Root { get; set; }

    public IList<string> Delimiter { get; }

    public IServerSettingsModel AddSetting(IServerSettingsModel model)
    {
        var rootModel = Root.Childs.FirstOrDefault(f => f.Key == model.Name).Value;
        if (rootModel != null)
        {
            foreach (var serverSettingsModel in model.Childs)
            {
                rootModel.AddChild(serverSettingsModel.Value);
            }

            foreach (var modelSettingsObject in model.SettingsObjects)
            {
                rootModel.SettingsObjects.Add(modelSettingsObject.Key, modelSettingsObject.Value);
            }

            return rootModel;
        }

        Root.Childs.Add(model.Name, model);
        return model;
    }

    private Tuple<IServerSettingsModel, ISettingsObject> Traverse(string path)
    {
        var pathElements = path.Split(Delimiter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        var nextModel = Root
            .Childs.FirstOrDefault(f => string.Equals(f.Key, pathElements.FirstOrDefault(), StringComparison))
            .Value;
        if (nextModel == null)
        {
            return null;
        }

        foreach (var pathPart in pathElements.Skip(1).Take(pathElements.Length - 2))
        {
            nextModel = nextModel.Childs.FirstOrDefault(f => f.Key.Equals(pathPart, StringComparison)).Value;
            if (nextModel == null)
            {
                _logger?.LogWarning($"The Child with the Name {path} was not found. The missing part is {pathPart}",
                    "ServerSettingsService");
                return null;
            }
        }

        var last = pathElements.LastOrDefault();
        var setting = nextModel.SettingsObjects.FirstOrDefault(e => string.Equals(e.Key, last, StringComparison));
        if (setting.Value != null)
        {
            return new Tuple<IServerSettingsModel, ISettingsObject>(null, setting.Value);
        }

        var child = nextModel.Childs.FirstOrDefault(e => string.Equals(e.Key, last, StringComparison)).Value;
        if (child != null)
        {
            return new Tuple<IServerSettingsModel, ISettingsObject>(child, null);
        }

        return null;
    }

    public ISettingsObject GetSettingObject(string path)
    {
        var traversePath = Traverse(path);
        if (traversePath == null || traversePath.Item2 == null)
        {
            _logger?.LogWarning($"The Setting with the Name {path} was not found.");
            return null;
        }

        return traversePath.Item2;
    }

    public IServerSettingsModel GetSettings(string path)
    {
        var traversePath = Traverse(path);
        if (traversePath == null || traversePath.Item1 == null)
        {
            _logger?.LogWarning($"The Child with the Name {path} was not found.");
            return null;
        }

        return traversePath.Item1;
    }

    public ISettingsObject CreateFromPath(string path)
    {
        return CreateFromPath<ServerSettingsModel, SettingsObject>(path).Item2;
    }

    public Tuple<IServerSettingsModel, TSetting> CreateFromPath<TSettingsModel, TSetting>(string path) where TSetting : ISettingsObject, new()
        where TSettingsModel :
        IServerSettingsModel, new()
    {
        var elements = path.Split(Delimiter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        var pre = Root;
        var pathToObject = pre.Path;
        foreach (var element in elements.Take(elements.Length - 1))
        {
            var n = new TSettingsModel
            {
                Name = element
            };
            pre = pre.AddChild(n);
            //pathToObject += Delimiter.First() + element;
            //pre.Path = pathToObject;
        }

        var setting = new TSetting();
        setting.Path = path;
        setting.Name = elements.Last();
        setting.Value = null;
        setting.ReadOnly = false;
        pre.SettingsObjects.Add(setting.Name, setting);
        return new Tuple<IServerSettingsModel, TSetting>(pre, setting);
    }

    //public IEnumerable<IServerSettingsModel> FromKeyValueMap(NameValueCollection collection, string name)
    //{
    //	foreach (string appSetting in collection)
    //	{
    //		var serverSettingsModel = CreateFromPath<ServerSettingsModel, SettingsObject>(name + Delimiter.First() + appSetting);
    //		serverSettingsModel.Item2.Value = WebConfigurationManager.AppSettings[appSetting];
    //		yield return serverSettingsModel.Item1;
    //	}
    //}

    public IEnumerable<IServerSettingsModel> FromConfiguration(IConfiguration configuration, string name)
    {
        foreach (var keyValuePair in configuration.AsEnumerable())
        {
            var serverSettingsModel = CreateFromPath<ServerSettingsModel, SettingsObject>(name + Delimiter.First() + keyValuePair.Key);
            serverSettingsModel.Item2.Value = keyValuePair.Value;
            yield return serverSettingsModel.Item1;
        }
    }

    public IEnumerable<IServerSettingsModel> FromEnvironment(string rootName = "Environment")
    {
        if (Environment.GetEnvironmentVariables() is Hashtable environmentVariables)
        {
            foreach (var environmentVariablesKey in environmentVariables.Keys)
            {
                var settingsObject = CreateFromPath<ServerSettingsModel, EnvironmentSettingObject>("Environment" + Delimiter.First() + environmentVariablesKey);
                settingsObject.Item2.Value = environmentVariables[environmentVariablesKey];
                settingsObject.Item2.EnvKey = environmentVariablesKey.ToString();
                yield return settingsObject.Item1;
            }
        }
    }

    public IEnumerable<IServerSettingsModel> FromConnectionSettings(IConfiguration configuration, string name = "ConnectionStrings")
    {
        foreach (var connection in configuration.GetSection(name).AsEnumerable())
        {
            var serverSettingsModel = CreateFromPath<ServerSettingsModel, SettingsObject>(name + Delimiter.First() + connection.Key);
            serverSettingsModel.Item2.Value = connection.Value;
            yield return serverSettingsModel.Item1;
        }
    }

    public object GetSetting(string path)
    {
        return GetSettingObject(path)?.Value;
    }

    public T GetSetting<T>(string path)
    {
        var val = GetSettingObject(path);
        if (val == null)
        {
            return default(T);
        }

        return val.As<T>();
    }

    public void SetSetting<T>(string path, T value)
    {
        var val = GetSettingObject(path);
        if (val == null)
        {
            return;
        }

        if (val.ReadOnly)
        {
            return;
        }

        val.Value = value;
    }
}