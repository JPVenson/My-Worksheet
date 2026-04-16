using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Reporting;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Reporting;

public partial class ReportEditView : NavigationPageBase
{
    private ValueState<string> _templateContent;

    public ReportEditView()
    {
        TemplateContent = new ValueState<string>("");
        ReportFileNameTemplate = new ValueState<string>("");
    }

    [Parameter]
    public Guid? Id { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ReportingService ReportingService { get; set; }
    [Inject]
    public CurrentUserStore CurrentUserStore { get; set; }

    public EntityState<NEngineTemplateLookup> ReportModel { get; set; }

    public ValueState<string> ReportFileNameTemplate { get; set; }

    public ReportingDataSourceViewModel SelectedDataSource { get; set; }

    public IObjectSchemaInfo DataSourceSchema { get; set; }

    public ValueState<string> TemplateContent
    {
        get { return _templateContent; }
        set
        {
            _templateContent = value;
        }
    }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/Reporting"));

        await ReportingService.ReportDataSources.Load();
        WhenChanged(ReportingService)
            .ThenRefresh(this);

        if (!Id.HasValue)
        {
            await ReportingService.Formatters.Load();
            var nEngineTemplateLookup = new NEngineTemplateLookup()
            {
                Name = "New",
                UsedDataSource = (SelectedDataSource = ReportingService.ReportDataSources.First()).Key,
                Comment = null,
                IdCreator = CurrentUserStore.CurrentToken.UserData.UserInfo.UserID,
                FileExtention = "txt",
                UsedFormattingEngine = ReportingService.Formatters.First().Key,
            };
            ReportModel = new EntityState<NEngineTemplateLookup>(nEngineTemplateLookup);
            await SetTitleAsync(new LocalizableString("Links/Reporting.Title", new LocalizableString("Common/New")));
        }
        else
        {
            ReportModel = ServerErrorManager.Eval(await HttpService.TemplateManagementApiAccess.GetTemplate(Id.Value));
            if (ReportModel == null)
            {
                return;
            }
            SelectedDataSource = ReportingService.ReportDataSources.FirstOrDefault(e => e.Key == ReportModel.Entity.UsedDataSource);
            TemplateContent = ServerErrorManager.EvalAndUnbox(await HttpService.TemplateManagementApiAccess.GetTemplateContent(Id.Value));
            await SetTitleAsync(new LocalizableString("Links/Reporting.Title", ReportModel.Entity.Name));
        }

        ReportFileNameTemplate = new ValueState<string>(ReportModel.Entity.FileNameTemplate);

        await ReloadSchema();
    }

    public async Task ReloadSchema()
    {
        var scheduleReportSchemaModel = ServerErrorManager.EvalAndUnbox(await HttpService.ReportManagementApiAccess.GetSchemaForRds(SelectedDataSource.Key));
        DataSourceSchema = scheduleReportSchemaModel.QuerySchema;
    }

    public async Task Delete()
    {
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.TemplateManagementApiAccess.Delete(Id.Value));
            if (apiResult.Success)
            {
                ModuleService.NavigateTo("/Reports");
            }
            ServerErrorManager.DisplayStatus("Common/Deleted");
        }
    }

    public async Task Save()
    {
        using (WaiterService.WhenDisposed())
        {
            ReportModel.Entity.FileNameTemplate = ReportFileNameTemplate.Entity;
            if (ReportModel.ListState == EntityListState.Added)
            {
                var model = new NEngineTemplateCreate()
                {
                    Name = ReportModel.Entity.Name,
                    Comment = ReportModel.Entity.Comment,
                    FileExtention = ReportModel.Entity.FileExtention,
                    FileNameTemplate = ReportModel.Entity.FileNameTemplate,
                    IdCreator = ReportModel.Entity.IdCreator,
                    Template = TemplateContent.Entity,
                    UsedDataSource = ReportModel.Entity.UsedDataSource,
                    UsedFormattingEngine = ReportModel.Entity.UsedFormattingEngine
                };
                var apiResult = ServerErrorManager.Eval(await HttpService.TemplateManagementApiAccess.Create(model));
                if (apiResult.Success)
                {
                    NavigationService.NavigateTo("/Report/" + apiResult.Object.NEngineTemplateId);
                    ReportModel = apiResult.Object;
                    await PluginService.PluginsChanged.RaiseAsync();
                    await SetTitleAsync(new LocalizableString("Links/Reporting.Title", ReportModel.Entity.Name));
                }
            }
            else
            {
                if (ReportModel.IsObjectDirty || TemplateContent.IsObjectDirty || ReportFileNameTemplate.IsObjectDirty)
                {
                    var apiResult = ServerErrorManager.Eval(await HttpService.TemplateManagementApiAccess.Update(new NEngineTemplateUpdate()
                    {
                        Name = ReportModel.Entity.Name,
                        Comment = ReportModel.Entity.Comment,
                        IdCreator = ReportModel.Entity.IdCreator,
                        FileExtention = ReportModel.Entity.FileExtention,
                        NEngineTemplateId = ReportModel.Entity.NEngineTemplateId,
                        Template = TemplateContent.Entity,
                        FileNameTemplate = ReportFileNameTemplate.Entity
                    }));
                    if (apiResult.Success)
                    {
                        ReportModel = apiResult;
                        TemplateContent.SetPristine();
                    }
                }
            }

            ServerErrorManager.DisplayStatus();
        }
    }
}