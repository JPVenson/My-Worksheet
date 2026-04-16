namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage
{
    public class StorageProviderStatistics : ViewModelBase
    {
        private long _createdFiles;

        private long _freeSpaceInMb;

        private long _usedSpaceInMb;

        public long FreeSpaceInMb
        {
            get { return _freeSpaceInMb; }
            set { SetProperty(ref _freeSpaceInMb, value); }
        }

        public long UsedSpaceInMb
        {
            get { return _usedSpaceInMb; }
            set { SetProperty(ref _usedSpaceInMb, value); }
        }

        public long CreatedFiles
        {
            get { return _createdFiles; }
            set { SetProperty(ref _createdFiles, value); }
        }
    }
}