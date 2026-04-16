using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Client.Services.UserSettings;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class TimeDisplay<TValue> : ComponentViewBase
{
    private string _editorValue;

    public virtual string EditorValue
    {
        get { return _editorValue; }
        set { SetProperty(ref _editorValue, value); }
    }

    [Parameter]
    public TimeFormatTypes Format { get; set; }

    [Inject]
    public TextService TextService { get; set; }

    private TValue _value;

    [Parameter]
    public TValue Value
    {
        get { return _value; }
        set
        {
            if (SetProperty(ref _value, value))
            {
                EditorValue = FormatMinutesToDateOnly((value as IConvertible ?? 0F).ToInt32(TextService.CurrentSystemUiCulture), Format, UserSettingsService.UiSettings.UseDecimalTimes);
            }
        }
    }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Inject]
    public UserSettingsService UserSettingsService { get; set; }

    public string FormatMinutesToDateOnly(int minutes, TimeFormatTypes format, bool decimalFormat)
    {
        switch (format)
        {
            case TimeFormatTypes.Minutes:
                return minutes.ToString("F2", TextService.CurrentSystemUiCulture);
            case TimeFormatTypes.Hours:
                return (Math.Ceiling(minutes / 60D)).ToString(TextService.CurrentSystemUiCulture);
            case TimeFormatTypes.Default:
            case TimeFormatTypes.MinutesAndHours:

                var timespan = TimeSpan.FromMinutes(minutes);
                var hours = timespan.TotalHours;
                if (decimalFormat)
                {
                    var decimalMinutes = (100D / 60D) * timespan.Minutes;
                    if (decimalMinutes < 0)
                    {
                        decimalMinutes = decimalMinutes * -1;
                    }
                    return $"{(int)hours:D2}{TextService.CurrentSystemUiCulture.NumberFormat.CurrencyDecimalSeparator}{(int)decimalMinutes:D2}";
                }
                return $"{(int)hours:D2}{TextService.CurrentSystemUiCulture.DateTimeFormat.TimeSeparator}{(timespan.Minutes < 0 ? timespan.Minutes * -1 : timespan.Minutes):D2}";
        }

        return string.Empty;
    }
}