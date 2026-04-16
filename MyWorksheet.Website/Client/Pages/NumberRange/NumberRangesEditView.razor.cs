using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Editor;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.ResLoaded;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.NumberRange;

public partial class NumberRangesEditView
{

    public StandaloneCodeEditor MonacoEditor { get; set; }

    [Inject]
    public HttpService HttpService { get; set; }

    [Inject]
    public ResourceLoaderService ResourceLoaderService { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    public EntityState<NumberRangeModel> NumberRange { get; set; }

    public IObjectSchemaInfo Schema { get; set; }
    public int CounterTestValue { get; set; }
    public string TestResult { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();
        TrackBreadcrumb(BreadcrumbService.AddModuleLink("Links/NumberRanges"));
        NumberRange = ServerErrorManager.Eval(await HttpService.NumberRangeApiAccess.Get(Id));
        if (NumberRange == null)
        {
            return;
        }
        Schema = ServerErrorManager.EvalAndUnbox(await HttpService.NumberRangeApiAccess.GetStructure(NumberRange.Entity.Code));
        await SetTitleAsync(new LocalizableString("Links/NumberRange.Title", NumberRange.Entity.Code));


        await ResourceLoaderService.AddResource(new StyleLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.css"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/loader.js"));
        await ResourceLoaderService.AddResource(new ScriptResource("require.config({ paths: { 'vs': '_content/BlazorMonaco/lib/monaco-editor/min/vs' } });"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.js"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/jsInterop.js"));
    }

    private async Task EditorCreated()
    {
        await Global.SetTheme(JsRuntime, "vs-dark");
        await MonacoEditor.SetValue(NumberRange.Entity.Template);
        AddDisposable(MonacoEditor);
    }

    public async Task Save()
    {
        await RefreshTemplate();
        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            var apiResult = ServerErrorManager.Eval(await HttpService.NumberRangeApiAccess.Update(NumberRange.Entity));
            if (apiResult.Success)
            {
                ModuleService.NavigateTo("/NumberRange/" + apiResult.Object.AppNumberRangeId);
            }
        }
        ServerErrorManager.DisplayStatus();
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor arg)
    {
        return new StandaloneEditorConstructionOptions()
        {
            Value = NumberRange?.Entity.Template,
        };
    }

    private async Task RefreshTemplate()
    {
        NumberRange.Entity.Template = await MonacoEditor.GetValue();
    }

    private async Task Test()
    {
        await RefreshTemplate();

        ServerErrorManager.Clear();
        using (WaiterService.WhenDisposed())
        {
            TestResult = ServerErrorManager.EvalAndUnbox(await HttpService.NumberRangeApiAccess.Test(NumberRange.Entity.Code,
                NumberRange.Entity.Template, CounterTestValue));
        }
    }
}