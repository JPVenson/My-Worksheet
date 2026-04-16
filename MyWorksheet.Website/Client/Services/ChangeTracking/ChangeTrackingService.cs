using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Server.Services;
using MyWorksheet.Website.Shared.ViewModels;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.ChangeTracking;

public delegate void OnEntityChanged(EntityChangedEventArguments arguments);
public delegate Task OnEntityChangedAsync(EntityChangedEventArguments arguments);

[SingletonService()]
public class ChangeTrackingService : RequireInit
{
    private readonly SignalService _signalService;

    public ChangeTrackingService(SignalService signalService)
    {
        _signalService = signalService;
        Trackers = new Dictionary<string, IList<IChangeTracker>>();
    }

    public override async ValueTask InitAsync(IServiceProvider services)
    {
        await base.InitAsync();
        _signalService.ObjectChangedHub.ExternalEntityChanged.Register(OnExternalEntityChanged);
    }

    private void OnExternalEntityChanged(EntityChangedEventArguments arguments)
    {
        if (Trackers.TryGetValue(arguments.Type, out var tracker))
        {
            foreach (var changeTracker in tracker)
            {
                changeTracker.Invoke(arguments);
            }
        }
    }

    public IDictionary<string, IList<IChangeTracker>> Trackers { get; set; }

    public IChangeTracker RegisterTracking(IChangeTracker tracker)
    {
        if (tracker.TrackingType is null)
        {
            return ChangeTracker.None;
        }

        if (!Trackers.TryGetValue(tracker.TrackingType, out var trackers))
        {
            trackers = new List<IChangeTracker>();
            Trackers.Add(tracker.TrackingType, trackers);
        }
        trackers.Add(tracker);
        return tracker;
    }

    public IChangeTracker RegisterTracking<T>(T obj, OnEntityChanged changed) where T : ViewModelBase
    {
        var id = obj.GetModelIdentifier();
        var trackingName = obj.GetType().GetTrackingName();
        return RegisterTracking(trackingName, (arguments) =>
        {
            if (arguments.Ids.Contains(id.Value))
            {
                return;
            }

            changed(arguments);
        });
    }

    public IChangeTracker RegisterTracking<T>(T obj, OnEntityChangedAsync changed) where T : ViewModelBase
    {
        var id = obj.GetModelIdentifier();
        var trackingName = obj.GetType().GetTrackingName();
        return RegisterTracking(trackingName, (arguments) =>
        {
            if (arguments.Ids.Contains(id.Value))
            {
                return;
            }

            changed(arguments);
        });
    }

    public IChangeTracker RegisterTracking(Type type, OnEntityChanged changed)
    {
        return RegisterTracking(type.GetTrackingName(), changed);
    }

    public IChangeTracker RegisterTracking(Type type, OnEntityChangedAsync changed)
    {
        return RegisterTracking(type.GetTrackingName(), changed);
    }

    public IChangeTracker RegisterTracking(string trackingType, OnEntityChanged changed)
    {
        return RegisterTracking(new ChangeTracker(this, trackingType, changed));
    }

    public IChangeTracker RegisterTracking(string trackingType, OnEntityChangedAsync changed)
    {
        return RegisterTracking(new ChangeTracker(this, trackingType, changed));
    }

    public void UnregisterTracker(ChangeTracker changeTracker)
    {
        Trackers[changeTracker.TrackingType].Remove(changeTracker);
    }
}

public class ChangeTracker : IChangeTracker
{
    static ChangeTracker()
    {
        None = new NoneChangeTracker();
    }

    public static IChangeTracker None { get; set; }

    private readonly ChangeTrackingService _trackingService;
    private readonly OnEntityChanged _changed;
    private readonly OnEntityChangedAsync _changedAsync;

    public ChangeTracker(ChangeTrackingService trackingService, string trackingType, OnEntityChanged changed)
    {
        _trackingService = trackingService;
        _changed = changed;
        TrackingType = trackingType;
    }

    public ChangeTracker(ChangeTrackingService trackingService, string trackingType, OnEntityChangedAsync changed)
    {
        _trackingService = trackingService;
        _changedAsync = changed;
        TrackingType = trackingType;
    }

    public string TrackingType { get; }

    public void Dispose()
    {
        _trackingService.UnregisterTracker(this);
    }

    public Task Invoke(EntityChangedEventArguments arguments)
    {
        try
        {
            if (_changed != null)
            {
                _changed(arguments);
                return Task.CompletedTask;
            }

            return _changedAsync(arguments);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}