using MyWorksheet.Website.Client.Util;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.UI;

[SingletonService()]
public class UIService
{
    public UIService()
    {
        UiLoaded = new OneTimePubSubEvent();
    }

    public OneTimePubSubEvent UiLoaded { get; set; }
}