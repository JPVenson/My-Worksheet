using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class WorksheetStatusViewModel : ViewModelBase
    {
        private string _action;

        private string _description;

        private bool _isPersitent;

        private DateTime _virtualDay;

        private Guid _worksheetItemStatusLookupId;

        public Guid WorksheetItemStatusLookupId
        {
            get { return _worksheetItemStatusLookupId; }
            set { SetProperty(ref _worksheetItemStatusLookupId, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public string Action
        {
            get { return _action; }
            set { SetProperty(ref _action, value); }
        }

        public bool IsPersitent
        {
            get { return _isPersitent; }
            set { SetProperty(ref _isPersitent, value); }
        }

        public DateTime VirtualDay
        {
            get { return _virtualDay; }
            set { SetProperty(ref _virtualDay, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return WorksheetItemStatusLookupId;
        }
    }
}