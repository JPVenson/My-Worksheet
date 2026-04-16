using MyWorksheet.Website.Client.Services.LocalStorage;
using MyWorksheet.Website.Client.Services.LocalStorage.Entities;
using MyWorksheet.Website.Client.Util;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Presentation;

[SingletonService()]
public class PresentationModeService
{
    private readonly StorageService _storageService;

    public PresentationModeService(StorageService storageService)
    {
        _storageService = storageService;
        PresentationStateChanged = _storageService.PresentationState.ValueChanged;
        _storageService.PresentationState.ValueChanged.Register((e) => PresentationState = e);
    }

    public PresentationState PresentationState { get; private set; }

    public PubSubEvent<PresentationState> PresentationStateChanged { get; set; }

}