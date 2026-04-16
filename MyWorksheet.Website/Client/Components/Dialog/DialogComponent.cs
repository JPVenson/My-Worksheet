using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Dialog;

public interface IDialogComponent : IDisposable
{
    string Key { get; }
    RenderFragment GetHeader(object currentDialogHeaderContent);
    RenderFragment GetBody(object currentDialogFooterContent);
    RenderFragment GetFooter(object currentDialogFooterContent);
    void Render();

    IDictionary<string, object> Attributes { get; }
    bool IsFullscreen { get; }
}

public abstract class DialogViewModelBase : ViewModelBase
{
    protected DialogViewModelBase()
    {

    }

    public DialogService DialogService { get; set; }

    public async Task ExecuteAndClose(Action action)
    {
        action();
        await Close();
    }

    public async Task ExecuteAndClose(Func<Task> action)
    {
        await action();
        await Close();
    }

    public async Task Close()
    {
        await DialogService.Hide();
    }
}


public class DialogComponent<THeader, TBody, TFooter> : ComponentBase, IDialogComponent
    where THeader : class
    where TBody : class
    where TFooter : class
{
    public DialogComponent()
    {

    }

    [Parameter]
    public THeader HeaderContent { get; set; }

    [Parameter]
    public TBody BodyContent { get; set; }

    [Parameter]
    public TFooter FooterContent { get; set; }

    [Parameter]
    [Required]
    public string Key { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Attributes { get; set; }

    [Parameter]
    public bool IsFullscreen { get; set; }

    public RenderFragment GetHeader(object currentDialogHeaderContent)
    {
        return HeaderTemplate((currentDialogHeaderContent as THeader) ?? HeaderContent);
    }

    public RenderFragment GetBody(object currentDialogFooterContent)
    {
        return BodyTemplate((currentDialogFooterContent as TBody) ?? BodyContent);
    }

    public RenderFragment GetFooter(object currentDialogFooterContent)
    {
        return FooterTemplate((currentDialogFooterContent as TFooter) ?? FooterContent);
    }

    public void Render()
    {
        StateHasChanged();
    }

    [Parameter]
    public RenderFragment<THeader> HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment<TBody> BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment<TFooter> FooterTemplate { get; set; }

    [CascadingParameter(Name = "DialogComponent")]
    public DialogContainerComponent Container { get; set; }

    protected override void OnParametersSet()
    {
        if (HeaderContent is LocalizableString && HeaderTemplate == null)
        {
            HeaderTemplate = value =>
            {
                return new RenderFragment(builder =>
                {
                    builder.OpenElement(0, "h2");
                    builder.OpenComponent<Translatable>(0);
                    builder.AddAttribute(1, nameof(Translatable.Loc), value);
                    builder.CloseComponent();
                    builder.CloseComponent();
                });
            };
        }

        if (BodyContent is LocalizableString && BodyTemplate == null)
        {
            BodyTemplate = value =>
            {
                return new RenderFragment(builder =>
                {
                    builder.OpenComponent<Translatable>(0);
                    builder.AddAttribute(1, nameof(Translatable.Loc), value);
                    builder.CloseComponent();
                });
            };
        }

        if (FooterContent is LocalizableString && FooterTemplate == null)
        {
            FooterTemplate = value =>
            {
                return new RenderFragment(builder =>
                {
                    builder.OpenComponent<Translatable>(0);
                    builder.AddAttribute(1, nameof(Translatable.Loc), value);
                    builder.CloseComponent();
                });
            };
        }

        base.OnParametersSet();
        Container.Add(this);
    }

    public void Dispose()
    {
        Container.Remove(this);
    }
}