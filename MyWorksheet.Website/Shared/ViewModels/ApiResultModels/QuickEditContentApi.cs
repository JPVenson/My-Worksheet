using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class QuickEditContentApi : ViewModelBase
{
    private string _content;

    private string _contentId;

    private bool _requireAuth;

    [Required]
    public string ContentId
    {
        get { return _contentId; }
        set { SetProperty(ref _contentId, value); }
    }

    [Required]
    public string Content
    {
        get { return _content; }
        set { SetProperty(ref _content, value); }
    }

    [Required]
    public bool RequireAuth
    {
        get { return _requireAuth; }
        set { SetProperty(ref _requireAuth, value); }
    }
}