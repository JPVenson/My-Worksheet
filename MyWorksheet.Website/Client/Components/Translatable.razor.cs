using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components;

public partial class Translatable : IDisposable
{
    public Translatable()
    {
    }

    [Parameter]
    public string Key { get; set; }


    private LocalizableString _loc;

    [Parameter]
    public LocalizableString Loc
    {
        get { return _loc; }
        set
        {
            if (_loc == value)
            {
                return;
            }

            _loc = value;
            LocChanged.Raise(value);
            if (value != null)
            {
                DoTranslate();
            }
        }
    }

    [Parameter] public EventCallback<LocalizableString> LocChanged { get; set; }

    public string Content { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment<string> DisplayTemplate { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Arguments { get; set; }

    async Task<string> TranslateArgument(object val)
    {
        if (val is LocalizableString locString)
        {
            return await TextService.Localize(locString.Text, locString.Arguments);
        }
        else
        {
            return val?.ToString();
        }
    }

    private async Task<IEnumerable<string>> GetArgumentsFromElement()
    {
        if (Arguments == null)
        {
            return Enumerable.Empty<string>();
        }

        var arguments = new List<string>();
        foreach (var argument in Arguments)
        {
            arguments.Add(await TranslateArgument(argument.Value));
        }

        return arguments;
    }


    private async Task<IEnumerable<string>> GetArgumentsFromArgument()
    {
        if (Loc.Arguments == null)
        {
            return Enumerable.Empty<string>();
        }

        var arguments = new List<string>();
        foreach (var argument in Loc.Arguments)
        {
            arguments.Add(await TranslateArgument(argument));
        }

        return arguments;
    }

    [Inject]
    public TextService TextService { get; set; }

    public ValueTask<string> Translate(string key, params object[] arguments)
    {
        if (TextService == null)
        {
            return ValueTask.FromResult("[Loading]");
        }

        return TextService.Localize(key, arguments);
    }

    protected override async Task OnParametersSetAsync()
    {
        TextService.ResourcesUpdated -= TextService_ResourcesUpdated;
        TextService.ResourcesUpdated += TextService_ResourcesUpdated;
        await DoTranslate();
        await base.OnParametersSetAsync();
    }

    private async Task DoTranslate()
    {
        if (Key != null)
        {
            Content = await Translate(Key, (await GetArgumentsFromElement()).ToArray());
        }
        else if (Loc != null && Loc.Text != null)
        {
            Content = await Translate(Loc.Text, (await GetArgumentsFromArgument()).ToArray());
        }
    }

    private async void TextService_ResourcesUpdated(object sender, string e)
    {
        if (Key != null && Key.StartsWith(e))
        {
            await DoTranslate();
            StateHasChanged();
        }
        else if (Loc != null)
        {
            if (Loc.Text?.StartsWith(e) == true)
            {
                await DoTranslate();
                StateHasChanged();
            }
        }
    }

    public void Dispose()
    {
        TextService.ResourcesUpdated -= TextService_ResourcesUpdated;
    }
}