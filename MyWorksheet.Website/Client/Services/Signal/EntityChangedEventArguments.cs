using System;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Client.Services.Signal;

public class EntityChangedEventArguments
{
    public EntityChangedEventArguments(string type, Guid[] ids, ChangeEventTypes changeEventTypes, Guid? userId)
    {
        Type = type;
        Ids = ids;
        ChangeEventTypes = changeEventTypes;
        UserId = userId;
    }

    public string Type { get; set; }
    public Guid[] Ids { get; set; }
    public ChangeEventTypes ChangeEventTypes { get; set; }
    public Guid? UserId { get; set; }
}