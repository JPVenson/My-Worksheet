using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Reporting;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Reporting;

public partial class ReportListView
{
    public ReportListView()
    {
        Reports = new FutureList<NEngineTemplateLookup>(() => ServerErrorManager.Eval(HttpService.TemplateManagementApiAccess.GetTemplates().AsTask()));
    }

    [Inject]
    public HttpService HttpService { get; set; }

    public IFutureList<NEngineTemplateLookup> Reports { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        await Reports.Load();
        AddDisposable(Reports.WhenLoaded(Render));
    }
}