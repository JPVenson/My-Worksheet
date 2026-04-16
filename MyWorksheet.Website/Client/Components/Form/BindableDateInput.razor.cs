using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Client.Services.UserSettings;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace MyWorksheet.Website.Client.Components.Form;

public class BindableDateInputX<TValue> : InputDate<TValue>
{
    [Inject]
    public UserSettingsService UserSettingsService { get; set; }
    [Inject]
    public TextService TextService { get; set; }

    private CultureInfo GetFromUserSettingsOrDefault()
    {
        //if (UserSettingsService.UiSettings.TimeLocale != null)
        //{
        //	return CultureInfo.GetCultureInfo(UserSettingsService.UiSettings.TimeLocale);
        //}
        return TextService.CurrentSystemUiCulture;
    }

    [Parameter]
    public BindingMode ChangeTrigger { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(3, "class", "form-control " + CssClass);
        var culture = GetFromUserSettingsOrDefault();
        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString, culture));
        if (ChangeTrigger == BindingMode.Default || ChangeTrigger == BindingMode.LostFocus)
        {
            builder.AddAttribute(5, "onchange",
                EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString, culture));
        }
        else
        {
            builder.AddAttribute(5, "oninput",
                EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString, culture));
        }
        if (AdditionalAttributes != null && !AdditionalAttributes.TryGetValue("type", out var type))
        {
            builder.AddAttribute(7, "type", "date");
        }
        else
        {
            switch (Type)
            {
                case InputDateType.Date:
                    builder.AddAttribute(7, "type", "date");
                    break;
                case InputDateType.DateTimeLocal:
                case InputDateType.Month:
                case InputDateType.Time:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        builder.CloseElement();
    }
}