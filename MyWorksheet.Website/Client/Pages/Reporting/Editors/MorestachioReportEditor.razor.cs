using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Editor;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Client.Components.Fullscreen;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.ResLoaded;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Morestachio;
using Morestachio.Framework.Tokenizing;
using Morestachio.Parsing.ParserErrors;
using Morestachio.TemplateContainers;

namespace MyWorksheet.Website.Client.Pages.Reporting.Editors;

public partial class MorestachioReportEditor
{
    public MorestachioReportEditor()
    {
        MorestachioErrors = new List<IMorestachioError>();
        ModelParts = new List<ModelParts>();
        ModelParts.Add(SelectedPart = new ModelParts()
        {
            Name = "Document",
            Content = "",
            CaretPosition = null,
            IsRootDocument = true
        });
        EditorId = Guid.NewGuid().ToString("N");
    }

    public StandaloneCodeEditor Editor
    {
        get { return _editor; }
        set
        {
            if (SetProperty(ref _editor, value))
            {
                //EditorCreated();
            }
        }
    }

    public string EditorId { get; }

    private EntityState<NEngineTemplateLookup> _reportModel;
    private ValueState<string> _templateContent;
    private StandaloneCodeEditor _editor;
    private FullscreenController _fullscreenController;

    [Parameter]
    public ValueState<string> TemplateContent
    {
        get { return _templateContent; }
        set
        {
            if (SetProperty(ref _templateContent, value, TemplateContentChanged))
            {
                SetValue();
            }
        }
    }

    [Parameter]
    public EntityState<NEngineTemplateLookup> ReportModel
    {
        get { return _reportModel; }
        set { SetProperty(ref _reportModel, value, ReportModelChanged); }
    }

    [Parameter]
    public EventCallback<ValueState<string>> TemplateContentChanged { get; set; }

    [Parameter]
    public EventCallback<EntityState<NEngineTemplateLookup>> ReportModelChanged { get; set; }

    [Parameter]
    public EventCallback SaveRequest { get; set; }

    [Parameter]
    public string ContainerClass { get; set; }

    [Parameter]
    public string EditorClass { get; set; }

    [Parameter]
    public IObjectSchemaInfo DataSourceSchema { get; set; }

    [Inject]
    public ResourceLoaderService ResourceLoaderService { get; set; }

    private void SetValue()
    {
        var partials = new Regex("({{!StartPartial (\\w*)}})(.*|[\\s\\S]*)(?:{{!EndPartial \\2}})");
        var rootTemplate = TemplateContent.Entity ?? "";

        foreach (var match in partials.Matches(rootTemplate).Reverse())
        {
            var partialName = match.Groups[2].Value;
            var partialContent = match.Groups[3].Value;
            var part = ModelParts.Find(e => e.Name == partialName);
            if (part == null)
            {
                part = new ModelParts()
                {
                    Name = partialName,
                    CaretPosition = null,
                    IsRootDocument = false,
                };
                ModelParts.Add(part);
            }

            var partialNameLength = "{{#DECLARE }}".Length + partialName.Length;
            part.Content = partialContent.Substring(partialNameLength,
                partialContent.Length - "{{/DECLARE}}".Length -
                partialNameLength);
            rootTemplate = rootTemplate.Remove(match.Index, match.Length);
        }

        var root = ModelParts.Find(e => e.IsRootDocument);
        root.Content = rootTemplate;
        Render();
    }


    public List<ModelParts> ModelParts { get; set; }
    public ModelParts SelectedPart { get; set; }

    public FullscreenController FullscreenController
    {
        get { return _fullscreenController; }
        set { _fullscreenController = value; }
    }

    private EditorLayoutInfo _oldLayout;

    public ICollection<IMorestachioError> MorestachioErrors { get; set; }
    public bool DisplaySchema { get; set; }

    public override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        await ResourceLoaderService
            .AddResource(new StyleLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.css"));


        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/jsInterop.js"));
        await ResourceLoaderService.AddResource(new ScriptResource(@"		
var require = { 
	paths: {
		'vs': '_content/BlazorMonaco/lib/monaco-editor/min/vs',
		'vs/basic-languages/morestachio/morestachio': 'scripts/app/morestachio/language-provider'
	}
};
"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/loader.js"));
        await ResourceLoaderService.AddResource(new ScriptLinkResource("_content/BlazorMonaco/lib/monaco-editor/min/vs/editor/editor.main.js"));
        await Task.Delay(1000);
        await ResourceLoaderService.AddResource(new ScriptResource(@"		

			require(['vs/basic-languages/morestachio/morestachio'], lang => {
					monaco.languages.register({ id: 'morestachio' });
					monaco.languages.setMonarchTokensProvider('morestachio', lang.language);
					monaco.languages.registerFoldingRangeProvider('morestachio', lang.foldingProvider);
					monaco.languages.registerHoverProvider('morestachio', lang.formatterProvider);
					monaco.editor.defineTheme(lang.theme.name, lang.theme);
					monaco.editor.setTheme('morestachioTheme');
			});
"));
    }

    private async Task<bool> EditorCreated()
    {
        if (ViewState != ViewInitState.Initialized)
        {
            return false;
        }

        await Editor.AddCommand((int)KeyMod.CtrlCmd | (int)KeyCode.KeyS, SaveRequested, null);
        await Editor.AddCommand((int)KeyCode.F11, ToggleFullscreen, null);
        await Global.SetTheme(JsRuntime, "morestachioTheme");

        FullscreenController.OnFullscreenChanged.Register(async () =>
        {
            if (await FullscreenController.HasFullscreen())
            {
                _oldLayout = await Editor.GetLayoutInfo();
                var dimensions = await FullscreenController.GetViewportDimensions();

                await Editor.Layout(new Dimension()
                {
                    Height = (int)dimensions.Height,
                    Width = (int)dimensions.Width
                });
            }
            else if (_oldLayout != null)
            {
                await Editor.Layout(new Dimension()
                {
                    Height = (int)_oldLayout.Height,
                    Width = (int)_oldLayout.Width
                });
            }
        }).TrackedBy(this);
        AddDisposable(Editor);
        return true;
    }

    private async void ToggleFullscreen(object[] args)
    {
        if (await FullscreenController.HasFullscreen())
        {
            await FullscreenController.ExitFullscreen();
        }
        else
        {
            await FullscreenController.RequestFullscreen();
        }
    }

    private async void SaveRequested(object[] args2)
    {
        await ValueChanged(new ModelChangedEvent());
        await SaveRequest.RaiseAsync();
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = "morestachio",
            Value = SelectedPart?.Content ?? ""
        };
    }

    public string ComposeValue()
    {
        var rootItem = ModelParts.Find(e => e.IsRootDocument);
        var sb = new StringBuilder();
        foreach (var part in ModelParts.Where(e => !e.IsRootDocument))
        {
            sb.Append("{{!StartPartial " + part.Name + "}}");
            sb.Append("{{#DECLARE " + part.Name + "}}");
            sb.Append(part.Content);
            sb.Append("{{/DECLARE}}");
            sb.Append("{{!EndPartial " + part.Name + "}}");
        }

        sb.Append(rootItem.Content);
        return sb.ToString();
    }

    private async Task ValueChanged(ModelChangedEvent obj)
    {
        if (SelectedPart == null)
        {
            return;
        }

        SelectedPart.Content = await Editor.GetValue();
        TemplateContent.Entity = ComposeValue();
    }

    private async Task RefreshEditorValue()
    {
        if (SelectedPart == null)
        {
            return;
        }

        SelectedPart.Content = await Editor.GetValue();
        TemplateContent.Entity = ComposeValue();

        MorestachioErrors.Clear();
        foreach (var error in await Parser.Validate(new StringTemplateContainer(TemplateContent.Entity)))
        {
            MorestachioErrors.Add(error);
        }

        var marker = MorestachioErrors.Select(f => new ModelMarker()
        {
            Message = f.HelpText,
            Code = "",
            StartLineNumber = f.Location.RangeStart.Row,
            StartColumn = f.Location.RangeStart.Column,
            EndColumn = f.Location.RangeEnd.Column,
            EndLineNumber = f.Location.RangeEnd.Row
        });
    }

    private async Task SetPartial(ModelParts partialTab)
    {
        DisplaySchema = false;
        if (partialTab == SelectedPart)
        {
            return;
        }

        if (SelectedPart != null)
        {
            SelectedPart.Content = await Editor.GetValue();
            SelectedPart.CaretPosition = await Editor.GetPosition();
        }

        SelectedPart = partialTab;
        await Editor.SetValue(SelectedPart.Content);
        if (SelectedPart.CaretPosition != null)
        {
            await Editor.SetPosition(SelectedPart.CaretPosition, "");
        }
    }

    private async Task AddNewPartial()
    {
        DisplaySchema = false;
        var modelParts = new ModelParts()
        {
            Content = "",
            CaretPosition = null,
            Name = "New",
            IsRootDocument = false
        };
        ModelParts.Add(modelParts);
        await SetPartial(modelParts);
    }

    private void ToggleSchemaDisplay()
    {
        DisplaySchema = !DisplaySchema;
        if (DisplaySchema)
        {
            SelectedPart = null;
        }
        else
        {
            SelectedPart = ModelParts.Find(e => e.IsRootDocument);
            OnNextRender(EditorCreated);
        }
    }
}

public class ModelParts
{
    public Position CaretPosition { get; set; }
    public bool IsRootDocument { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
    public double ScrollPosition { get; set; }
}

public class ModelMarker
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("endColumn")]
    public int EndColumn { get; set; }

    [JsonPropertyName("endLineNumber")]
    public int EndLineNumber { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("startColumn")]
    public int StartColumn { get; set; }

    [JsonPropertyName("startLineNumber")]
    public int StartLineNumber { get; set; }
}

public enum MarkerSeverity
{
    Error,
    Info,
    Hint,
    Warning
}