using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class QuickListEditContentApi : ViewModelBase
{
    private int _cMSContnetID;

    private string _content;

    private string _contentId;

    private string _contentTemplate;

    private bool _jsonBlob;

    private bool _requireAuth;

    [Required]
    public int CMSContnetID
    {
        get { return _cMSContnetID; }
        set { SetProperty(ref _cMSContnetID, value); }
    }

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
    public bool JsonBlob
    {
        get { return _jsonBlob; }
        set { SetProperty(ref _jsonBlob, value); }
    }

    [Required]
    public string ContentTemplate
    {
        get { return _contentTemplate; }
        set { SetProperty(ref _contentTemplate, value); }
    }

    [Required]
    public bool RequireAuth
    {
        get { return _requireAuth; }
        set { SetProperty(ref _requireAuth, value); }
    }
}