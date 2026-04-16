namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels
{
    public class DocumentsApiPost : DocumentsApiGet
    {
        private int _userID;

        public int UserID
        {
            get { return _userID; }
            set { SetProperty(ref _userID, value); }
        }
    }
}