using System;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public class DynamicSettingsObject : SettingsObject
{
    private readonly Func<object> _generateSetting;
    private readonly bool _allowOverwriting;
    private object _overwrittenValue;

    public bool Overwritten { get; set; }

    public DynamicSettingsObject(Func<object> generateSetting, bool allowOverwriting)
    {
        _generateSetting = generateSetting;
        _allowOverwriting = allowOverwriting;
    }

    public override bool ReadOnly { get; set; } = false;

    public override object Value
    {
        get { return Overwritten ? _overwrittenValue : _generateSetting(); }
        set
        {
            if (!_allowOverwriting)
            {
                throw new InvalidOperationException("This setting is ReadOnly");
            }
            _overwrittenValue = value;
        }
    }
}
