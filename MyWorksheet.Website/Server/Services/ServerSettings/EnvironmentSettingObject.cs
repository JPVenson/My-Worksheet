using System;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public class EnvironmentSettingObject : SettingsObject
{
    private bool _preset;

    public override object Value
    {
        get
        {
            return Environment.GetEnvironmentVariable(EnvKey, EnvironmentVariableTarget.Process);
        }
        set
        {
            if (!_preset || EnvKey == null)
            {
                _preset = true;
                return;
            }

            if (Value?.ToString() != value?.ToString())
            {
                Environment.SetEnvironmentVariable(EnvKey, Value?.ToString(), EnvironmentVariableTarget.Process);
            }
        }
    }

    public string EnvKey { get; set; }
}
