using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using Microsoft.AspNetCore.Components;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Client.Pages.PaymentInfo;

public partial class PaymentInfoEditView
{
    [Inject]
    public HttpService HttpService { get; set; }

    [Parameter]
    public Guid? Id { get; set; }

    public EntityState<PaymentInfoModel> PaymentInfo { get; set; }
    public IFutureTrackedList<PaymentInfoContentModel> PaymentFields { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/PaymentInfos"));

        if (!Id.HasValue)
        {
            PaymentInfo = new EntityState<PaymentInfoModel>(new PaymentInfoModel(), EntityListState.Added);
            PaymentFields = new TrackedList<PaymentInfoContentModel>();
        }
        else
        {
            PaymentFields = new FutureTrackedList<PaymentInfoContentModel>(() => ServerErrorManager.Eval(HttpService.PaymentInfoApiAccess.GetFields(Id.Value).AsTask()));
            PaymentInfo = ServerErrorManager.Eval(await HttpService.PaymentInfoApiAccess.Get(Id.Value));
        }

        TrackWhen()
            .Changed(PaymentFields)
            .ThenRefresh(this);
    }

    public async Task Save()
    {
        using (WaiterService.WhenDisposed())
        {
            if (PaymentInfo.ListState == EntityListState.Added)
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.PaymentInfoApiAccess.Create(
                    new UpdatePaymentInfoModel()
                    {
                        PaymentInfoModel = PaymentInfo.Entity,
                        ContentModels = PaymentFields.GetStates().Select(f => f.AsApiEntity()).ToArray()
                    }));
                ServerErrorManager.DisplayStatus();
                if (apiResult.Success)
                {
                    NavigationService.NavigateTo("/PaymentInfo/" + apiResult.Object.PaymentInfoId, true);
                    return;
                }
            }
            else
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.PaymentInfoApiAccess.Update(
                    new UpdatePaymentInfoModel()
                    {
                        PaymentInfoModel = PaymentInfo.Entity,
                        ContentModels = PaymentFields
                            .GetStates()
                            .Where(e => e.IsObjectDirty)
                            .Select(f => f.AsApiEntity())
                            .ToArray()
                    }));
                if (apiResult.Success)
                {
                    PaymentInfo = apiResult;
                    PaymentFields.Reset();
                }
                ServerErrorManager.DisplayStatus();
            }
        }
    }
}