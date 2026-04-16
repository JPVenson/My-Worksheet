using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out
{
    public class OutgoingWebhookModelGet : ViewModelBase
    {
        private string _callingUrl;
        private Guid _idOutgoingWebhookCase;
        private bool _isActive;
        private Guid _outgoingWebhookId;
        private string _secret;
        private string _numberRangeEntry;
        private string _name;

        public string NumberRangeEntry
        {
            get { return _numberRangeEntry; }
            set { SetProperty(ref _numberRangeEntry, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public Guid OutgoingWebhookId
        {
            get { return _outgoingWebhookId; }
            set { SetProperty(ref _outgoingWebhookId, value); }
        }

        public string CallingUrl
        {
            get { return _callingUrl; }
            set { SetProperty(ref _callingUrl, value); }
        }

        public Guid IdOutgoingWebhookCase
        {
            get { return _idOutgoingWebhookCase; }
            set { SetProperty(ref _idOutgoingWebhookCase, value); }
        }

        public string Secret
        {
            get { return _secret; }
            set { SetProperty(ref _secret, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return OutgoingWebhookId;
        }
    }
}