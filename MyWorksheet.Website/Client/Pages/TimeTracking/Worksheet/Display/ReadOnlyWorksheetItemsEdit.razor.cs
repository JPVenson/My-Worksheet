using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet.Display;

public partial class ReadOnlyWorksheetItemsEdit
{
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }
}