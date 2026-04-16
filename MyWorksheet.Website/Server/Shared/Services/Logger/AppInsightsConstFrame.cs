using System;
using Microsoft.ApplicationInsights.Channel;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;

public class AppInsightsConstFrame<TElemetry> : IDisposable, IAppInsightsConstFrame where TElemetry : class, ITelemetry
{
    public Action<TElemetry> FrameAction { get; }
    private readonly Action _done;

    public AppInsightsConstFrame(Action<TElemetry> action, Action done)
    {
        FrameAction = action;
        _done = done;
    }

    public void Invoke(TElemetry tElemetry)
    {
        FrameAction(tElemetry);
    }

    public void Invoke(object tElemetry)
    {
        FrameAction(tElemetry as TElemetry);
    }

    public void Dispose()
    {
        _done();
    }

    //public void Invoke<TElemetry1>(TElemetry1 tElemetry) where TElemetry1 : class, ITelemetry{
    //	FrameAction(tElemetry as TElemetry);
    //}
}