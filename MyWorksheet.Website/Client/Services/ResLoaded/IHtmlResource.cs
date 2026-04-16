using System;
using MyWorksheet.Website.Client.Util;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Services.ResLoaded;

public interface IHtmlResource : IEquatable<IHtmlResource>
{
    RenderFragment Render();
    OneTimePubSubEvent OnLoaded { get; }
}