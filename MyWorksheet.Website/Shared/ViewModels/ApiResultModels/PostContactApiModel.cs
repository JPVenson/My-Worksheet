namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class PostContactApiModel : ViewModelBase
{
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string Message { get; set; }
    public string ContactType { get; set; }
    public string RecaptureMessage { get; set; }
}