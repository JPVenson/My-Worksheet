namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels
{
    public class ContentCreateModel : ViewModelBase
    {
        private string _content_ID;

        private string _contentLang;

        private bool _isJSONBlob;

        public string Content_ID
        {
            get { return _content_ID; }
            set { SetProperty(ref _content_ID, value); }
        }

        public string ContentLang
        {
            get { return _contentLang; }
            set { SetProperty(ref _contentLang, value); }
        }

        public bool IsJSONBlob
        {
            get { return _isJSONBlob; }
            set { SetProperty(ref _isJSONBlob, value); }
        }
    }
}