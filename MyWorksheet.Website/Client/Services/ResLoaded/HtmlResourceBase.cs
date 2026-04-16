using MyWorksheet.Website.Client.Util;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Services.ResLoaded;

public abstract class HtmlResourceBase : IHtmlResource
{
    public HtmlResourceBase()
    {
        OnLoaded = new OneTimePubSubEvent();
    }

    public abstract bool Equals(IHtmlResource? other);
    public abstract RenderFragment Render();

    public OneTimePubSubEvent OnLoaded { get; set; }
}