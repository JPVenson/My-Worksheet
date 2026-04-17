using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.ServerInfo.InfoItems;

public class TaskTimer
{
    private readonly int _delay;
    private readonly ILogger _appLogger;

    public TaskTimer(int delay, string tag, ILogger appLogger)
    {
        Tag = tag;
        _delay = delay;
        _appLogger = appLogger;
        Consumers = [];
        _stopRequested = new CancellationTokenSource();
    }

    public string Tag { get; private set; }

    public void Add(Action toAdd)
    {
        Consumers.Add(toAdd);
    }

    public void Remove(Action toAdd)
    {
        Consumers.Remove(toAdd);
    }

    public ICollection<Action> Consumers { get; private set; }
    private Task _schedulerTask;
    private CancellationTokenSource _stopRequested;

    public void Start()
    {
        if (_schedulerTask != null)
        {
            return;
        }
        _stopRequested = new CancellationTokenSource();
        _schedulerTask = new Task(async () =>
        {
            while (!_stopRequested.IsCancellationRequested)
            {
                foreach (var consumer in Consumers.ToArray())
                {
                    try
                    {
                        consumer();
                    }
                    catch (Exception e)
                    {
                        _appLogger.LogError("ServerPull has encountered an Error", "ServerPull", new Dictionary<string, string>()
                        {
                            { "Exception", JsonConvert.SerializeObject(e)},
                            { "Consumer", consumer.ToString()},
                        });
                    }
                }

                await Task.Delay(_delay);
            }

            _schedulerTask = null;
        });
        _schedulerTask.Start();
    }

    public void Stop()
    {
        _stopRequested.Cancel();
    }

}