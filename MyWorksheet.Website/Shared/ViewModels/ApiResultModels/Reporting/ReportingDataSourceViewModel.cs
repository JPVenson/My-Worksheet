using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting
{
    public class ReportingDataSourceViewModel : ViewModelBase
    {
        private Guid _id;

        private string _key;

        private string _name;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return Id;
        }
    }
}