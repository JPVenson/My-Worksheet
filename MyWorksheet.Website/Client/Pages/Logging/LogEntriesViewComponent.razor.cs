using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Logging;

[Authorize(Roles = "Administrator")]
public partial class LogEntriesViewComponent
{
    [Parameter]
    public ICollection<AppLoggerLogViewModel> LoggerEntries { get; set; }
}