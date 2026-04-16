using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Text;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public abstract class EntityTrackingComponent : ComponentViewBase
{
    [Inject]
    public ChangeTrackingService ChangeTrackingService { get; set; }

    public void Track<TEntity>(OnEntityChanged changed)
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(typeof(TEntity), changed));
    }

    public void Track(string entityName, OnEntityChanged changed)
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(entityName, changed));
    }

    public void Track<T>(T entity, OnEntityChanged changed) where T : ViewModelBase
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(entity, changed));
    }

    public void Track<TEntity>(OnEntityChangedAsync changed)
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(typeof(TEntity), changed));
    }

    public void Track(string entityName, OnEntityChangedAsync changed)
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(entityName, changed));
    }

    public void Track<T>(T entity, OnEntityChangedAsync changed) where T : ViewModelBase
    {
        AddDisposable(ChangeTrackingService.RegisterTracking(entity, changed));
    }
}