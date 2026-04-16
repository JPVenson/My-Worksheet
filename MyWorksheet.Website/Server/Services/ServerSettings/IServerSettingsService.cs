using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public interface IServerSettingsService
{
    IServerSettingsModel Root { get; set; }

    object GetSetting(string path);
    T GetSetting<T>(string path);
    void SetSetting<T>(string path, T value);
    IList<string> Delimiter { get; }
    ISettingsObject GetSettingObject(string path);
    IServerSettingsModel GetSettings(string path);
}