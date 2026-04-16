using System;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PriorityQueueItemAttribute : Attribute
{
    public PriorityQueueItemAttribute(string key)
    {
        Key = key;
    }

    public string Key { get; private set; }
}