namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class LabelOversightModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
    }
}