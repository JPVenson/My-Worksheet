using System;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation
{
    public class UserToUserAssosiationViewModel : ViewModelBase
    {
        private AccountApiUserPost _child;

        private AccountApiUserPost _parent;

        private UserToUserRoleViewModel _role;

        private Guid _userAssoisiatedUserMapId;

        public AccountApiUserPost Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }

        public AccountApiUserPost Child
        {
            get { return _child; }
            set { SetProperty(ref _child, value); }
        }

        public UserToUserRoleViewModel Role
        {
            get { return _role; }
            set { SetProperty(ref _role, value); }
        }

        public Guid UserAssoisiatedUserMapId
        {
            get { return _userAssoisiatedUserMapId; }
            set { SetProperty(ref _userAssoisiatedUserMapId, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return UserAssoisiatedUserMapId;
        }
    }
}