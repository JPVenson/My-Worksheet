using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out
{
    public class OutgoingWebhookCaseModel : ViewModelBase
    {
        private string _descriptionHtml;

        private string _name;

        private Guid _outgoingWebhookCaseId;

        public Guid OutgoingWebhookCaseId
        {
            get { return _outgoingWebhookCaseId; }
            set { SetProperty(ref _outgoingWebhookCaseId, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string DescriptionHtml
        {
            get { return _descriptionHtml; }
            set { SetProperty(ref _descriptionHtml, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return OutgoingWebhookCaseId;
        }
    }
}