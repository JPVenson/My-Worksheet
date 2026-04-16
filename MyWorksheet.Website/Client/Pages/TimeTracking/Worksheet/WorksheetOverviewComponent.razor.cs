using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChargeRate;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.AspNetCore.Components;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetOverviewComponent
{
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }

    [Inject]
    public UserWorkloadService UserWorkloadService { get; set; }
    [Inject]
    public ChargeRateService ChargeRateService { get; set; }

    public override Task LoadDataAsync()
    {
        WhenChanged(Model.WorksheetItems).ThenRefresh(this);
        WhenChanged(ChargeRateService).ThenRefresh(this);
        Model.WorksheetItems.Load();
        ChargeRateService.Load();
        return base.LoadDataAsync();
    }

    public Tuple<Money, ProjectItemRateViewModel>[] GetPayedTime()
    {
        var rates = new List<Tuple<Money, ProjectItemRateViewModel>>();
        foreach (var modelWorksheetItem in Model.WorksheetItems.GroupBy(e => e.IdProjectItemRate))
        {
            var chargeRate = Model.ChargeRates.FirstOrDefault(e => e.ProjectItemRateId == modelWorksheetItem.Key);
            if (chargeRate == null)
            {
                return rates.ToArray();
            }

            rates.Add(Tuple.Create(ChargeRateService.CalculateByTime(chargeRate, modelWorksheetItem), chargeRate));
        }

        return rates.ToArray();
    }
}