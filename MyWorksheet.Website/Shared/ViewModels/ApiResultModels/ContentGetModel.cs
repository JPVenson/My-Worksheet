namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels
{
    public class ContentGetModel : ViewModelBase
    {
        private int _cMSContnetID;

        private string _content;

        private string _content_ID;

        private string _content_Template;

        private string _contentLang;

        private bool _isJSONBlob;

        private bool _requireAuth;

        public int CMSContnetID
        {
            get { return _cMSContnetID; }
            set { SetProperty(ref _cMSContnetID, value); }
        }

        public string Content_ID
        {
            get { return _content_ID; }
            set { SetProperty(ref _content_ID, value); }
        }

        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public string ContentLang
        {
            get { return _contentLang; }
            set { SetProperty(ref _contentLang, value); }
        }

        public string Content_Template
        {
            get { return _content_Template; }
            set { SetProperty(ref _content_Template, value); }
        }

        public bool IsJSONBlob
        {
            get { return _isJSONBlob; }
            set { SetProperty(ref _isJSONBlob, value); }
        }

        public bool RequireAuth
        {
            get { return _requireAuth; }
            set { SetProperty(ref _requireAuth, value); }
        }
    }
}