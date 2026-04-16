namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class NEngineApiElement : ViewModelBase
    {
        private string _comment;

        private int _nEngineTemplate_ID;

        private string _template;

        private string _type;

        public int NEngineTemplate_ID
        {
            get { return _nEngineTemplate_ID; }
            set { SetProperty(ref _nEngineTemplate_ID, value); }
        }

        public string Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }
    }
}