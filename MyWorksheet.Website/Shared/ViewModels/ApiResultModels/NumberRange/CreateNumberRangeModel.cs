namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange
{
    public class CreateNumberRangeModel : ViewModelBase
    {
        public string Template { get; set; }
        public long Counter { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}