using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.NumberRange;

public partial class NumberRangesListView
{
    public NumberRangesListView()
    {
        NumberRanges = new List<NumberRangeModel>();
    }

    [Inject]
    public HttpService HttpService { get; set; }

    public IList<NumberRangeModel> NumberRanges { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        var apiResult = ServerErrorManager.Eval(await HttpService.NumberRangeApiAccess.Get());
        if (!apiResult.Success)
        {
            return;
        }

        NumberRanges = apiResult.Object;
    }
}