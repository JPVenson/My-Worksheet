using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater
{
    public class PriorityQueueTaskViewModel : ViewModelBase
    {
        private string _key;

        private string _name;

        private Version _version;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public Version Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
    }
}