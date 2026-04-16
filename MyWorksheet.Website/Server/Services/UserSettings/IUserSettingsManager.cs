using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.UserSettings;

public interface IUserSettingsManager
{
    object GetSettingFromStore(string name, string key, Guid userId);
    void UpdateSetting(object setting, string name, string key, Guid userId);
    IDictionary<string, Type> SettingsTypes { get; }
}