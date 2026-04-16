using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Shared.Dialog;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Util.View;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Components;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Dialog;

[SingletonService()]
public class DialogService
{
    private readonly NavigationService _navigationService;
    IList<VisibilityChangeTracker> Tracker { get; set; }

    public DialogService(NavigationService navigationService)
    {
        Tracker = new List<VisibilityChangeTracker>();
        _navigationService = navigationService;
        _navigationService.LocationChanged += _navigationService_LocationChanged;
    }

    private async void _navigationService_LocationChanged(object sender, LocationChangedEventArgs e)
    {
        await Hide();
    }

    public event EventHandler<DialogServiceShowEventArgs> DialogDisplayRequest;

    public VisibilityChangeTracker Show(string key, object content = null)
    {
        return Show(key, content, content, content);
    }

    public VisibilityChangeTracker Show(string key, object headerContent, object bodyContent, object footerContent)
    {
        OnDialogDisplayRequest(new DialogServiceShowEventArgs()
        {
            Key = key,
            Show = true,
            BodyContent = bodyContent,
            FooterContent = footerContent,
            HeaderContent = headerContent
        });
        VisibilityChangeTracker tracker = null;
        tracker = new VisibilityChangeTracker(() =>
        {
            Tracker.Remove(tracker);
        });
        Tracker.Add(tracker);
        return tracker;
    }

    public async Task Hide(string key = null)
    {
        OnDialogDisplayRequest(new DialogServiceShowEventArgs()
        {
            Key = key,
            Show = false
        });
        foreach (var visibilityChangeTracker in Tracker)
        {
            await visibilityChangeTracker.Invoke();
        }
        Tracker.Clear();
    }

    protected virtual void OnDialogDisplayRequest(DialogServiceShowEventArgs e)
    {
        DialogDisplayRequest?.Invoke(this, e);
    }
}

public static class DialogServiceExtensions
{
    public static VisibilityChangeTracker ShowMessageBox(this DialogService dialogService, MessageBoxDialogViewModel dialog)
    {
        return dialogService.Show("MessageBox", dialog);
    }
}

public class DialogServiceShowEventArgs : EventArgs
{
    public DialogServiceShowEventArgs()
    {

    }

    public string Key { get; set; }
    public bool Show { get; set; }

    public object HeaderContent { get; set; }
    public object BodyContent { get; set; }
    public object FooterContent { get; set; }
}

public class VisibilityChangeTracker : IDisposable
{
    private readonly IList<Delegate> _onInvoke;
    private readonly Action _onDeregister;
    private readonly TaskCompletionSource _taskCompletionSource;

    public VisibilityChangeTracker(Action onDeregister)
    {
        _onInvoke = new List<Delegate>();
        _onDeregister = onDeregister;
        _taskCompletionSource = new TaskCompletionSource();
    }

    public void Dispose()
    {
        _onDeregister();
    }

    public VisibilityChangeTracker Closed(Action action)
    {
        _onInvoke.Add(action);
        return this;
    }

    public Task Await()
    {
        return _taskCompletionSource.Task;
    }

    public VisibilityChangeTracker Closed(Func<Task> action)
    {
        _onInvoke.Add(action);
        return this;
    }

    public async Task Invoke()
    {
        _taskCompletionSource.SetResult();
        foreach (var action in _onInvoke)
        {
            if (action is Action act)
            {
                act();
            }
            else if (action is Func<Task> fnc)
            {
                await fnc();
            }
        }
    }
}