using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class SubmittedProjectsModel : ViewModelBase
    {
        private Guid _projectId;

        private long _wCount;

        public Guid ProjectId
        {
            get { return _projectId; }
            set { SetProperty(ref _projectId, value); }
        }

        public long WCount
        {
            get { return _wCount; }
            set { SetProperty(ref _wCount, value); }
        }
        //public Guid IdCreator { get; set; }

        public override Guid? GetModelIdentifier()
        {
            return ProjectId;
        }
    }
}