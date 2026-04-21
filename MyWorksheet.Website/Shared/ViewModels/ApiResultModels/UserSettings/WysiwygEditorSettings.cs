namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings;

public class WysiwygEditorSettings : ViewModelBase
{
    private bool _forHtml;

    public bool ForHtml
    {
        get { return _forHtml; }
        set { SetProperty(ref _forHtml, value); }
    }
}