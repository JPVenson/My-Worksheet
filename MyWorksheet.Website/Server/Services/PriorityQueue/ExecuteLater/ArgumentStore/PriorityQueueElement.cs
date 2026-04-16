using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;

[Serializable]
public sealed class PriorityQueueElement
{
    public PriorityQueueElement()
    {
    }

    public PriorityQueueElement(IComparable level, Guid userId, string actionKey, IDictionary<string, object> arguments, string[] followUps)
        : this(level, userId, actionKey, arguments, followUps.Select(e => new PriorityQueueElement(level, userId, e, arguments, new PriorityQueueElement[0])).ToArray())
    {
    }

    public PriorityQueueElement(IComparable level, Guid userId, string actionKey, IDictionary<string, object> arguments, PriorityQueueElement[] followUps)
    {
        UserId = userId;
        ActionKey = actionKey;
        Arguments = new SerializableObjectDictionary<string, object>(arguments);
        FollowUps = followUps;
        Level = level;
    }

    public IComparable Level { get; set; }
    public Guid UserId { get; set; }
    public string ActionKey { get; set; }
    public SerializableObjectDictionary<string, object> Arguments { get; set; }
    public Version Version { get; set; }
    public PriorityQueueElement[] FollowUps { get; set; }

    public string[] CapabilitiesRequired { get; set; }

    public PriorityQueueElement CreateCopy()
    {
        return new PriorityQueueElement(Level, UserId, ActionKey, Arguments.ToDictionary(e => e.Key, e => e.Value), FollowUps.Select(e => e.CreateCopy()).ToArray());
    }
}