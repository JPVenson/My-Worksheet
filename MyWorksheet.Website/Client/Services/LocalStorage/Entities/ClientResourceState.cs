using System.Collections.Generic;
using MyWorksheet.Website.Client.Services.Auth;

namespace MyWorksheet.Website.Client.Services.LocalStorage;

public class ClientResourceState
{
    public string PreferredUiLanguages { get; set; }
    public IList<UiResourceState> UiResourceStates { get; set; }
}