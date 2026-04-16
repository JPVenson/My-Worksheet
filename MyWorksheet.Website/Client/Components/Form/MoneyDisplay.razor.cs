using System;
using Microsoft.AspNetCore.Components;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class MoneyDisplay : LocalizationAwareSelf
{
    public MoneyDisplay()
    {

    }

    [Parameter]
    public Money Value { get; set; }
}
