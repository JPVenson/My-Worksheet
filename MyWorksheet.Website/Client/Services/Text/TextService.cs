using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.LocalStorage;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Client.Services.Text;

[SingletonService()]
public class TextService : RequireInit
{
    private readonly HttpService _httpService;
    private readonly StorageService _storageService;
    private readonly LazyAssemblyLoader _assemblyLoader;

    public TextService(HttpService httpService, StorageService storageService, LazyAssemblyLoader assemblyLoader)
    {
        _httpService = httpService;
        _storageService = storageService;
        _assemblyLoader = assemblyLoader;

        SupportedCultures = new List<CultureInfo>()
        {
            new CultureInfo("DE-DE"),
            new CultureInfo("EN-US"),
        };
        UiStates = new List<TextResourceState>();
        foreach (var supportedCulture in SupportedCultures)
        {
            UiStates.Add(new TextResourceState()
            {
                Culture = supportedCulture,
                State = null,
            });
        }
        CurrentCulture = UiStates.FirstOrDefault(e => Equals(e.Culture, CultureInfo.CurrentUICulture)) ?? UiStates.FirstOrDefault();
    }

    public override async ValueTask InitAsync()
    {
        await base.InitAsync();
        var uiResourceStates = await _storageService.CurrentUIResourcesState.ReadValue();
        if (uiResourceStates == null)
        {
            uiResourceStates = new ClientResourceState();
            uiResourceStates.PreferredUiLanguages = CurrentCulture.Culture.Name.ToUpper();
            await _storageService.CurrentUIResourcesState.WriteValue(uiResourceStates);
        }

        await UpdateCulture(SupportedCultures.FirstOrDefault(e => e.Name.ToUpper() == uiResourceStates.PreferredUiLanguages.ToUpper()));
    }

    public IList<TextResourceState> UiStates { get; set; }

    public class TextResourceState
    {
        public TextResourceState()
        {
            TextResources = new Dictionary<string, TextResourcesLookup>();
        }

        public CultureInfo Culture { get; set; }
        public string State { get; set; }
        public IDictionary<string, TextResourcesLookup> TextResources { get; set; }
    }

    public class TextResourcesLookup
    {
        public TextResourcesLookup(string @group, Task loadingTask)
        {
            Group = @group;
            LoadingTask = loadingTask;
            Resources = Array.Empty<TextResourceViewModel>();
        }
        public TextResourcesLookup(string @group, IEnumerable<TextResourceViewModel> resources)
        {
            Group = @group;
            LoadingTask = Task.CompletedTask;
            Resources = resources.ToArray();
        }

        public string Group { get; }
        public Task LoadingTask { get; }
        public TextResourceViewModel[] Resources { get; set; }
    }

    public TextResourceState CurrentCulture { get; set; }
    public CultureInfo CurrentSystemUiCulture { get; set; }
    public IList<CultureInfo> SupportedCultures { get; set; }

    public event EventHandler<string> ResourcesUpdated;

    public async ValueTask<string> Localize(string key, params object[] arguments)
    {
        key = key.ToUpper();
        if (key == "{0}")
        {
            return Transform(key, key, arguments);
        }
        var group = key.Split("/")[0];
        if (CurrentCulture.TextResources.TryGetValue(group, out var resource))
        {
            var resourceValue = resource
                .Resources
                .FirstOrDefault(e => string.Equals(e.Key, key, StringComparison.InvariantCultureIgnoreCase));
            return Transform(key, resourceValue?.Text, arguments);
        }
        return $"[{key}]";
    }

    public string Transform(string key, string value, params object[] arguments)
    {
        if (value == null)
        {
            return $"[{key}]";
        }

        for (int i = 0; i < arguments.Length; i++)
        {
            var arg = arguments[i];
            value = value.Replace("{" + i + "}", arg?.ToString());
        }

        return value;
    }

    protected virtual void OnResourcesUpdated(string e)
    {
        ResourcesUpdated?.Invoke(this, e);
    }

    public async Task UpdateCulture(CultureInfo value)
    {
        CurrentCulture = UiStates.FirstOrDefault(e => Equals(e.Culture, value)) ?? UiStates.First();

        var needsReload = false;
        var serverState = (await _httpService.TextApiAccess.GetState());

        if (CurrentCulture.State == null)
        {
            needsReload = true;
        }
        else
        {
            needsReload = serverState.Object.FirstOrDefault(e => e.CultureName == value.Name)?.State !=
                          CurrentCulture.State;
        }

        if (needsReload)
        {
            var resources = (await _httpService.TextApiAccess.GetAll(value.Name));
            CurrentCulture.TextResources = resources.Object.GroupBy(e => e.Page)
                .ToDictionary(e => e.Key, e => new TextResourcesLookup(e.Key, e.ToArray()));
            CurrentCulture.State = serverState.Object.First(e => String.Equals(e.CultureName, value.Name, StringComparison.CurrentCultureIgnoreCase)).State;
        }
        CurrentSystemUiCulture = value;
        OnResourcesUpdated(string.Empty);

        await _storageService.CurrentUIResourcesState.WriteValue(new ClientResourceState()
        {
            PreferredUiLanguages = CurrentCulture.Culture.Name.ToUpper()
        });

        //foreach (var textResourcesLookup in TextResources)
        //{
        //	await textResourcesLookup.Value.LoadingTask;
        //}
        //TextResources.Clear();
    }
}