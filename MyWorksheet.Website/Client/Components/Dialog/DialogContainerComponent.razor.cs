using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Client.Services.OverlayDraw;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Dialog;

public partial class DialogContainerComponent
{
    public DialogContainerComponent()
    {
        Dialogs = new Dictionary<string, DialogViewModel>();
    }
    private OverlayHandler _overlayHandle;
    private int _currentIndex;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Inject]
    public DialogService DialogService { get; set; }

    [Inject]
    public OverlayDrawOrderService OverlayDrawOrderService { get; set; }

    public IDictionary<string, DialogViewModel> Dialogs { get; set; }

    public DialogInstanceViewModel CurrentDialog { get; set; }

    public void RegisterDialogViewModel(object viewModel)
    {
        if (viewModel is DialogViewModelBase vm)
        {
            vm.DialogService = DialogService;
            vm.PropertyChanged -= VmOnPropertyChanged;
            vm.PropertyChanged += VmOnPropertyChanged;
        }
    }

    public void UnRegisterDialogViewModel(object viewModel)
    {
        if (viewModel is DialogViewModelBase vm)
        {
            vm.PropertyChanged -= VmOnPropertyChanged;
        }
    }

    private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    public void Show(string key, object headerContent, object bodyContent, object footerContent)
    {
        if (Dialogs.TryGetValue(key, out var dialogViewModel))
        {
            _overlayHandle = OverlayDrawOrderService.Reserve();
            _overlayHandle.OrderItem.Register((idx) =>
            {
                _currentIndex = idx;
                StateHasChanged();
            });

            RegisterDialogViewModel(headerContent);
            RegisterDialogViewModel(bodyContent);
            RegisterDialogViewModel(footerContent);

            CurrentDialog = new DialogInstanceViewModel()
            {
                Dialog = dialogViewModel,
                HeaderContent = headerContent,
                BodyContent = bodyContent,
                FooterContent = footerContent
            };
        }
    }

    public void Hide(string key = null)
    {
        if (CurrentDialog != null && (key == null || CurrentDialog?.Dialog.Key == key))
        {
            UnRegisterDialogViewModel(CurrentDialog.HeaderContent);
            UnRegisterDialogViewModel(CurrentDialog.BodyContent);
            UnRegisterDialogViewModel(CurrentDialog.FooterContent);
            CurrentDialog.Hide();
            Task.Delay(TimeSpan.FromSeconds(.4)).ContinueWith((t) =>
            {
                CurrentDialog = null;
                StateHasChanged();
            });
            _overlayHandle.DisposeAsync();
        }
    }

    public void Add(IDialogComponent dialogComponent)
    {
        Dialogs[dialogComponent.Key] = (new DialogViewModel()
        {
            Key = dialogComponent.Key,
            DialogView = dialogComponent
        });
    }

    public void Remove(IDialogComponent dialogComponent)
    {
        Dialogs.Remove(Dialogs.FirstOrDefault(e => e.Key == dialogComponent.Key));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DialogService.DialogDisplayRequest += DialogService_DialogDisplayRequest;
    }

    private void DialogService_DialogDisplayRequest(object sender, DialogServiceShowEventArgs e)
    {
        if (e.Show)
        {
            Show(e.Key, e.HeaderContent, e.BodyContent, e.FooterContent);
        }
        else
        {
            Hide(e.Key);
        }
        StateHasChanged();
    }
}

public class DialogViewModel
{
    public IDialogComponent DialogView { get; set; }
    public string Key { get; set; }
}

public class DialogInstanceViewModel
{
    public DialogViewModel Dialog { get; set; }

    public object HeaderContent { get; set; }
    public object BodyContent { get; set; }
    public object FooterContent { get; set; }

    public bool ToBeHidden { get; set; }

    public void Hide()
    {
        ToBeHidden = true;
    }
}