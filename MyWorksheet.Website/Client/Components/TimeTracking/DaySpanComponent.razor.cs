using System;
using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.TimeTracking;

public partial class DaySpanComponent
{
    public DaySpanComponent()
    {

    }

    [Parameter]
    public IList<WorksheetItemModel> WorksheetItemModels { get; set; }

    [Parameter]
    public DateTimeOffset Date { get; set; }
}