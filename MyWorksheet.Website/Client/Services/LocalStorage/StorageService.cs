using System;
using System.Linq;
using System.Threading;
using Blazored.LocalStorage;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.LocalStorage.Entities;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.LocalStorage;

[SingletonService()]
public class StorageService
{
    public Func<ILocalStorageService> LocalStorageFactory { get; }

    public StorageService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        LocalStorageFactory = () =>
        {
            return scope.ServiceProvider.GetService<ILocalStorageService>();
        };

        PresentationState = new StorageEntry<PresentationState>(this, "PresentationState", new PresentationState()
        {
            Enabled = false
        });
        StorageState = new StorageEntry<StorageState>(this, "StorageState");
        LoginToken = new StorageEntry<LoginResult>(this, "LoginToken");
        CurrentUIResourcesState = new StorageEntry<ClientResourceState>(this, "CurrentUIResourcesState");
    }

    public StorageEntry<StorageState> StorageState { get; set; }
    public StorageEntry<PresentationState> PresentationState { get; set; }

    public StorageEntry<LoginResult> LoginToken { get; set; }
    public StorageEntry<ClientResourceState> CurrentUIResourcesState { get; set; }
}