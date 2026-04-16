namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Text
{
    public class TextResourceViewModel : ViewModelBase
    {
        private string _key;

        private string _lang;

        private string _page;

        private string _text;

        public string Lang
        {
            get { return _lang; }
            set { SetProperty(ref _lang, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public string Page
        {
            get { return _page; }
            set { SetProperty(ref _page, value); }
        }
    }
}