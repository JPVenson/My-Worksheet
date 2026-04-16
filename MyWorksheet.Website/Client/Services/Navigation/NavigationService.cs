using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Pages.Base;
using MyWorksheet.Website.Client.Services.Dialog;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Navigation;

[SingletonService()]
public class NavigationService
{
    public NavigationService(NavigationManager navigationManager)
    {
        NavigationManager = navigationManager;
        NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        FragmentChanges = new List<Action>();
        QueryFragments = new Dictionary<string, string>();
        SetFragmentFromUri(NavigationManager.Uri, false);
    }

    public string Uri { get; set; }

    private void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        SetFragmentFromUri(e.Location, e.IsNavigationIntercepted);
    }

    public void SetFragmentFromUri(string uriText, bool isIntercepted)
    {
        var uri = new Uri(uriText);
        QueryFragments["Whole"] = uri.Fragment.TrimStart('#');

        foreach (var fragmentChange in FragmentChanges)
        {
            fragmentChange();
        }

        var newUri = new UriBuilder(uriText) { Fragment = "" };
        var oldUri = new UriBuilder(Uri ?? "none") { Fragment = "" };
        //if (newUri.ToString() == oldUri.ToString())
        //{
        //	return;
        //}

        Uri = uriText;
        OnLocationChanged(new LocationChangedEventArgs(oldUri.ToString(), uriText, isIntercepted));
    }

    public void SetFragment(string fragment)
    {
        var uriBuilder = new UriBuilder(NavigationManager.Uri);
        if (uriBuilder.Fragment == fragment)
        {
            return;
        }
        uriBuilder.Fragment = fragment;
        NavigationManager.NavigateTo(uriBuilder.ToString());
    }

    public void NavigateTo(string url, bool force = false)
    {
        if (force)
        {
            SameLocationChanged?.Invoke(this, new LocationChangedEventArgs(Uri, url, false));
            NavigationManager.NavigateTo(url);
            return;
        }

        NavigationManager.NavigateTo(url, force);
    }

    public event EventHandler<LocationChangedEventArgs> SameLocationChanged;
    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    public IDictionary<string, string> QueryFragments { get; set; }

    public NavigationManager NavigationManager { get; }

    public IList<Action> FragmentChanges { get; set; }

    public IDisposable WhenFragmentChanges(Action action)
    {
        FragmentChanges.Add(action);
        return new Disposable(() =>
        {
            FragmentChanges.Remove(action);
        });
    }

    protected virtual void OnLocationChanged(LocationChangedEventArgs e)
    {
        LocationChanged?.Invoke(this, e);
    }
}

public class LocationChangedEventArgs : Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs
{
    public string OldLocation { get; }

    public LocationChangedEventArgs(string oldLocation, string location, bool isNavigationIntercepted) : base(location, isNavigationIntercepted)
    {
        OldLocation = oldLocation;
    }
}