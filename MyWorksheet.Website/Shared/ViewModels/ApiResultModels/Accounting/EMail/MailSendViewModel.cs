using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail
{
    public class MailSendViewModel : ViewModelBase
    {
        private Guid? _idAttachment;

        private Guid _idContent;

        private Guid _idMailAccount;

        private Guid _mailSendId;

        private int _resendCount;

        private DateTime? _sendAt;

        private bool _success;

        public Guid MailSendId
        {
            get { return _mailSendId; }
            set { SetProperty(ref _mailSendId, value); }
        }

        public Guid IdMailAccount
        {
            get { return _idMailAccount; }
            set { SetProperty(ref _idMailAccount, value); }
        }

        public Guid IdContent
        {
            get { return _idContent; }
            set { SetProperty(ref _idContent, value); }
        }

        public Guid? IdAttachment
        {
            get { return _idAttachment; }
            set { SetProperty(ref _idAttachment, value); }
        }

        public DateTime? SendAt
        {
            get { return _sendAt; }
            set { SetProperty(ref _sendAt, value); }
        }

        public bool Success
        {
            get { return _success; }
            set { SetProperty(ref _success, value); }
        }

        public int ResendCount
        {
            get { return _resendCount; }
            set { SetProperty(ref _resendCount, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return MailSendId;
        }
    }
}