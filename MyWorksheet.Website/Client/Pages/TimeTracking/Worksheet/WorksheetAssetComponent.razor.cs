using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Shared;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.UserWorkload;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.Worksheet;

public partial class WorksheetAssetComponent
{
    [Parameter]
    public WorksheetEditViewModel Model { get; set; }
    [Inject]
    public HttpService HttpService { get; set; }

    public IFutureList<WorksheetAssertViewModel> Asserts { get; set; }

    public override Task LoadDataAsync()
    {
        WhenChanged(Model.Worksheet).ThenRefresh(this);
        var changeAdapter = TrackWhen();
        Asserts = new FutureList<WorksheetAssertViewModel>(async () =>
        {
            changeAdapter.Unregister();
            return ServerErrorManager
                .Eval(await HttpService.WorksheetAssertApiAccess.GetAssertsFromWorksheet(Model.Worksheet.WorksheetId).AsTask())
                .With(f =>
                {
                    return f.Select(e =>
                    {
                        var model = new WorksheetAssertViewModel();
                        model.AssertFiles = new FutureList<FileModel>(async () =>
                        {
                            return ServerErrorManager
                                .Eval(await HttpService.WorksheetAssertApiAccess.GetStorageEntries(e.WorksheetAssertId).AsTask())
                                .With(g => g.Select(w => new FileModel(w)).ToArray());
                        });
                        changeAdapter.Changed(model.AssertFiles);
                        model.Assert = e;
                        return model;
                    }).ToArray();
                });
        });
        changeAdapter.ThenRefresh(this);
        WhenChanged(Asserts).ThenRefresh(this);
        CreateEditAssert();
        return base.LoadDataAsync();
    }

    public WorksheetAssertViewModel EditAssert { get; set; }
    public bool DisplayAddNew { get; set; }
    public GetStorageProvider SelectedProvider { get; set; }

    public void CreateEditAssert()
    {
        EditAssert = new WorksheetAssertViewModel()
        {
            Assert = new WorksheetAssertCreateViewModel()
            {
                Value = 0,
                Name = "Assert",
                Description = "",
                IdWorksheet = Model.Worksheet.WorksheetId,
                Tax = 0,
            },
            AssertFiles = new FutureList<FileModel>(FutureList<FileModel>.Empty)
        };
    }

    public async Task DeleteAssert(WorksheetAssertViewModel assert)
    {
        using (WaiterService.WhenDisposed())
        {
            if (assert.Assert.ListState != EntityListState.Added)
            {
                var apiResult = ServerErrorManager.Eval(
                    await HttpService.WorksheetAssertApiAccess.Delete(assert.Assert.Entity.WorksheetAssertId));
                ServerErrorManager.DisplayStatus();
                if (!apiResult.Success)
                {
                    return;
                }
            }

            Asserts.Remove(assert);
        }
    }

    public async Task SaveAssert(WorksheetAssertViewModel assert)
    {
        using (WaiterService.WhenDisposed())
        {
            if (assert.Assert.Entity.WorksheetAssertId != Guid.Empty)
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetAssertApiAccess.Create(assert.Assert.Entity));
                if (apiResult.Success)
                {
                    if (assert.FilesToAdd.Any())
                    {
                        ServerErrorManager.Eval(await HttpService.WorksheetAssertApiAccess.AddFile(
                            apiResult.Object.WorksheetAssertId, SelectedProvider.StorageProviderId,
                            assert.FilesToAdd.Select(f => f.File).ToArray()));
                    }

                    Asserts.Reset();
                    await Asserts.Load();
                    DisplayAddNew = false;
                    CreateEditAssert();
                }
            }
            else
            {
                var apiResult = ServerErrorManager.Eval(await HttpService.WorksheetAssertApiAccess.Update(assert.Assert.Entity,
                    assert.Assert.Entity.WorksheetAssertId));
                if (apiResult.Success)
                {
                    assert.Assert = apiResult.Object;
                }
            }

            ServerErrorManager.DisplayStatus();
            Render();
        }
    }
}

public class WorksheetAssertViewModel
{
    public WorksheetAssertViewModel()
    {
        FilesToAdd = new List<BrowserFile>();
    }

    public EntityState<WorksheetAssertCreateViewModel> Assert { get; set; }
    public IFutureList<FileModel> AssertFiles { get; set; }
    public IList<BrowserFile> FilesToAdd { get; set; }
}