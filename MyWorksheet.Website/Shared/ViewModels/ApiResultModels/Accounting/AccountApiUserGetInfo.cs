using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting
{
    public class AccountApiUserGetInfo : AccountApiUserPost
    {
        private bool _headhunter;

        private Guid _idCountry;

        private bool _isActive;

        private bool _needPasswordReset;

        private Guid _userID;

        private string _username;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public Guid UserID
        {
            get { return _userID; }
            set { SetProperty(ref _userID, value); }
        }

        public bool Headhunter
        {
            get { return _headhunter; }
            set { SetProperty(ref _headhunter, value); }
        }

        public bool NeedPasswordReset
        {
            get { return _needPasswordReset; }
            set { SetProperty(ref _needPasswordReset, value); }
        }

        public Guid IdCountry
        {
            get { return _idCountry; }
            set { SetProperty(ref _idCountry, value); }
        }
    }
}