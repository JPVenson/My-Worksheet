using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.PaymentInfo;

public partial class PaymentInfoListView
{
    public PaymentInfoListView()
    {

    }

    [Inject]
    public HttpService HttpService { get; set; }

    public IFutureList<PaymentInfoModel> PaymentInfos { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        PaymentInfos = new FutureList<PaymentInfoModel>(() =>
            ServerErrorManager.Eval(HttpService.PaymentInfoApiAccess.Get().AsTask()));
        TrackWhen()
            .Changed(PaymentInfos)
            .ThenRefresh(this);
    }
}