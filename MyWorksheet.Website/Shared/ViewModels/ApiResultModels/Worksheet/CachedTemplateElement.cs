using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class CachedTemplateElement : ViewModelBase
    {
        private string _executingUser;

        private DateTime _expiresAt;

        private string _filename;

        private string _key;

        private object _state;

        private string _template;

        private string _templateName;

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }

        public string TemplateName
        {
            get { return _templateName; }
            set { SetProperty(ref _templateName, value); }
        }

        public string Filename
        {
            get { return _filename; }
            set { SetProperty(ref _filename, value); }
        }

        public string ExecutingUser
        {
            get { return _executingUser; }
            set { SetProperty(ref _executingUser, value); }
        }

        public DateTime ExpiresAt
        {
            get { return _expiresAt; }
            set { SetProperty(ref _expiresAt, value); }
        }

        public object State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
        //public TemplateRunningState RunningState { get; set; }
    }
}