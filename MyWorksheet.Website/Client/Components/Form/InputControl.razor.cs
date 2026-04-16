using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Components.Form;

public interface IFieldControl
{
    bool ReadOnly { get; set; }
    string EditorId { get; set; }
}

public enum BindingMode
{
    Default,
    LostFocus,
    PropertyChanged
}

public class ValueChangedEventArgs<TValue>
{
    public ValueChangedEventArgs(TValue oldValue, TValue value)
    {
        OldValue = oldValue;
        NewValue = value;
        Same = Equals(OldValue, NewValue);
    }

    public TValue OldValue { get; private set; }
    public TValue NewValue { get; private set; }
    public bool Same { get; set; }
}

public partial class InputControl<TValue> : ComponentViewBase, IFieldControl
{
    public InputControl()
    {
        EditorId = Guid.NewGuid().ToString("N");
    }

    private TValue _value;

    [Parameter]
    public TValue Value
    {
        get
        {
            return _value;
        }
        set
        {
            var valueChangedEventArgs = new ValueChangedEventArgs<TValue>(_value, value);
            if (valueChangedEventArgs.Same)
            {
                return;
            }

            if (SetProperty(ref _value, value, ValueChanged))
            {
                OnValueChanged.Raise(valueChangedEventArgs);
            }
        }
    }

    [Parameter]
    public BindingMode BindingMode { get; set; }

    [Parameter]
    public Expression<Func<TValue>> ValueExpression { get; set; }

    [Parameter]
    public EventCallback<FocusEventArgs> OnFocusLost { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<ValueChangedEventArgs<TValue>> OnValueChanged { get; set; }

    [Parameter]
    public Expression<Func<object>> Validate { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public string DisplayFormat { get; set; }

    [Parameter]
    public string Title { get; set; }
    [Parameter]
    public string Placeholder { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Arguments { get; set; }

    [Parameter]
    [Required]
    public FieldTypes FieldType { get; set; }
    [Parameter]
    public string EditorId { get; set; }
    [Parameter]
    public RenderFragment DataListSource { get; set; }

    public static FieldTypes GetFieldTypeFromCsType(Type type)
    {
        if (Nullable.GetUnderlyingType(type) != null)
        {
            type = Nullable.GetUnderlyingType(type);
        }

        if (type == typeof(string))
        {
            return FieldTypes.Text;
        }
        else if (type == typeof(bool))
        {
            return FieldTypes.Checkbox;
        }
        else if (type == typeof(Money))
        {
            return FieldTypes.Money;
        }
        else if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
        {
            return FieldTypes.Date;
        }
        else if (new Type[]
                 {
                     typeof(int),
                     typeof(long),

                     typeof(decimal),
                     typeof(double),
                     typeof(float),
                 }.Contains(type))
        {
            return FieldTypes.Number;
        }
        else
        {
            return FieldTypes.Display;
        }
    }
}

public enum FieldTypes
{
    Auto,
    Display,
    Text,
    ServerText,
    MultiLineText,
    Number,
    Checkbox,
    RadioBox,
    Date,
    Time,
    Password,
    EMail,
    Money
}