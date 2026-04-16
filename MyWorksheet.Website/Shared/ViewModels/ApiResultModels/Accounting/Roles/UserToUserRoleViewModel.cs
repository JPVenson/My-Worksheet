using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles
{
    public class UserToUserRoleViewModel : ViewModelBase
    {
        private string _description;

        private Guid _id;

        private string _name;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return Id;
        }
    }
}