namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment
{
    public class GetFeatureMeta : GetFeatureMetaPublic
    {
        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }
    }
}