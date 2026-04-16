using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload
{
    public class GetUserWorkloadViewModel : UpdateUserWorkloadViewModel
    {
        private Guid _idAppUser;

        public Guid IdAppUser
        {
            get { return _idAppUser; }
            set { SetProperty(ref _idAppUser, value); }
        }
    }
}