using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload
{
    public class UpdateUserWorkloadViewModel : CreateUserWorkloadViewModel
    {
        private Guid _userWorkloadId;

        public Guid UserWorkloadId
        {
            get { return _userWorkloadId; }
            set { SetProperty(ref _userWorkloadId, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return UserWorkloadId;
        }
    }
}