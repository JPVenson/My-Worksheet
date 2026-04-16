using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.Values;
using MyWorksheet.Website.Client.Components;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MyWorksheet.Website.Client.Components;

public partial class TimeValueEditor<TValue> : ComponentViewBase
{
    public TimeValueEditor()
    {
    }


    private string _editorValueAsString;

    [Parameter]
    public string EditorValueAsString
    {
        get { return _editorValueAsString; }
        set
        {
            if (TryParseHourString(value, out var val))
            {
                if (SetProperty(ref _editorValueAsString, value, EditorValueAsStringChanged))
                {
                    SetValue(val);
                }
            }
        }
    }

    private static bool TryParseHourString(string hours, out TimeSpan value)
    {
        if (hours.IndexOf(':') == -1)
        {
            if (int.TryParse(hours, out var totalHours))
            {
                value = TimeSpan.FromHours(totalHours);
                return true;
            }
            value = TimeSpan.Zero;
            return false;
        }

        var values = hours.Split(':');
        if (int.TryParse(values[0], out var hoursValue) && int.TryParse(values[1], out var minutesValue))
        {
            value = TimeSpan.FromMinutes(hoursValue * 60 + minutesValue);
            return true;
        }
        value = TimeSpan.Zero;
        return false;
    }

    [Parameter]
    public EventCallback<string> EditorValueAsStringChanged { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private DateTime? _editorValue;

    public DateTime? EditorValue
    {
        get { return _editorValue; }
        set
        {
            if (SetProperty(ref _editorValue, value, EditorValueChanged))
            {
                if (!value.HasValue)
                {
                    SetValue(TimeSpan.Zero);
                }
                else
                {
                    SetValue(value.Value - DateTime.Today);
                }
            }
        }
    }

    public EventCallback<DateTime?> EditorValueChanged { get; set; }

    private void SetValue(TimeSpan? timespan)
    {
        if (this is TimeValueEditor<TimeSpan> tsEditor)
        {
            tsEditor.SetProperty(ref tsEditor._value, timespan.Value, tsEditor.ValueChanged);
        }
        if (this is TimeValueEditor<TimeSpan?> tsNEditor)
        {
            tsNEditor.SetProperty(ref tsNEditor._value, timespan, tsNEditor.ValueChanged);
        }
        else if (Converter != null)
        {
            var val = Converter.ConvertTo(timespan, typeof(TimeSpan));
            SetProperty(ref _value, val, ValueChanged);
        }

        if (timespan.HasValue)
        {
            var timespanValue = $"{((int)timespan.Value.TotalHours).ToString("D2")}:{timespan.Value.Minutes.ToString("D2")}";
            //var timespanValue = timespan.Value.ToString("hh\\:mm");
            SetProperty(ref _editorValueAsString, timespanValue, EditorValueAsStringChanged);
        }
    }

    private TValue _value;

    [Parameter]
    public TValue Value
    {
        get { return _value; }
        set
        {
            if (SetProperty(ref _value, value, ValueChanged))
            {
                if (Converter == null)
                {
                    throw new InvalidOperationException("Converter must be set");
                }

                EditorValue = DateTime.Today.Add(Converter.ConvertFrom(value, typeof(TimeSpan)) ?? TimeSpan.Zero);
            }
        }
    }

    [Parameter]
    public IBindingConverter<TimeSpan?, TValue> Converter { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }
    [Parameter]
    public Expression<Func<TValue>> ValueExpression { get; set; }

    [Parameter]
    public TimeFormatTypes Format { get; set; }

    [Parameter]
    public DisplayModeTypes DisplayMode { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    public string GetEditorFormat()
    {
        switch (Format)
        {
            case TimeFormatTypes.Hours:
            case TimeFormatTypes.Minutes:
                return "[0-9]*";
            case TimeFormatTypes.Default:
            case TimeFormatTypes.MinutesAndHours:
                return "^([0-9]+)?[:]?([0-5]?[0-9]?)?$";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}