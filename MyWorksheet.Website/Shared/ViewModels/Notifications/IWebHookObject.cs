using System;
using System.Xml.Serialization;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.Notifications;

[XmlInclude(typeof(GetProjectModel))]
[XmlInclude(typeof(WorksheetModel))]
[XmlInclude(typeof(WorksheetItemModel))]
public interface IWebHookObject
{
    ActionTypes Type { get; set; }

    object Content { get; set; }

    DateTime SendAt { get; set; }
    DateTime CreatedAt { get; set; }
    string User { get; set; }
}