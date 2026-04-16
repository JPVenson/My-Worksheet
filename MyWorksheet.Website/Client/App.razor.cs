using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ThemeManager;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client;

public partial class App
{
    [Inject]
    public ThemeManagerService ThemeManagerService { get; set; }
}