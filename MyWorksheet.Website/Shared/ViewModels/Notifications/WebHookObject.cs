using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.Notifications
{
    /// <summary>
    ///     The Structure of the Payload of a Webhook. Should never be translated for display nameing as displayed as Raw
    ///     struture
    /// </summary>
    /// <seealso cref="IWebHookObject" />
    [XmlInclude(typeof(GetProjectModel))]
    [XmlInclude(typeof(WorksheetModel))]
    [XmlInclude(typeof(WorksheetItemModel))]
    [XmlInclude(typeof(UserActivityViewModel))]
    [KnownType(typeof(GetProjectModel))]
    [KnownType(typeof(WorksheetModel))]
    [KnownType(typeof(WorksheetItemModel))]
    [KnownType(typeof(UserActivityViewModel))]
    public class WebHookObject : ViewModelBase, IWebHookObject
    {
        private object _content;
        private DateTime _createdAt;
        private DateTime _sendAt;
        private ActionTypes _type;
        private string _user;

        [JsonComment("WebhookData/Comment.Type")]
        public ActionTypes Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        [JsonComment("WebhookData/Comment.Content")]
        public object Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        [JsonComment("WebhookData/Comment.SendAt")]
        public DateTime SendAt
        {
            get { return _sendAt; }
            set { SetProperty(ref _sendAt, value); }
        }

        [JsonComment("WebhookData/Comment.CreatedAt")]
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { SetProperty(ref _createdAt, value); }
        }

        [JsonComment("WebhookData/Comment.User")]
        public string User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }
    }
}