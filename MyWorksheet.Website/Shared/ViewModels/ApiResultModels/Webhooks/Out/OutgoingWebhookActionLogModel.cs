using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out
{
    public class OutgoingWebhookActionLogModel : ViewModelBase
    {
        private DateTime _dateOfAction;

        private string _error;

        private Guid _idOutgoingWebhook;

        private Guid _outgoingWebhookActionLogId;

        private bool _success;

        public Guid OutgoingWebhookActionLogId
        {
            get { return _outgoingWebhookActionLogId; }
            set { SetProperty(ref _outgoingWebhookActionLogId, value); }
        }

        public bool Success
        {
            get { return _success; }
            set { SetProperty(ref _success, value); }
        }

        public string Error
        {
            get { return _error; }
            set { SetProperty(ref _error, value); }
        }

        public DateTime DateOfAction
        {
            get { return _dateOfAction; }
            set { SetProperty(ref _dateOfAction, value); }
        }

        public Guid IdOutgoingWebhook
        {
            get { return _idOutgoingWebhook; }
            set { SetProperty(ref _idOutgoingWebhook, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return OutgoingWebhookActionLogId;
        }
    }
}