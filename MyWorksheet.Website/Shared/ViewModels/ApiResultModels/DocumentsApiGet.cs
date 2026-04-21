using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels;

public class DocumentsApiGet : ViewModelBase
{
    private string _fileType;

    private string _hostedOn;

    private string _link;

    private string _name;

    private Guid _userDocumentCacheId;

    public Guid UserDocumentCacheId
    {
        get { return _userDocumentCacheId; }
        set { SetProperty(ref _userDocumentCacheId, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public string Link
    {
        get { return _link; }
        set { SetProperty(ref _link, value); }
    }

    public string HostedOn
    {
        get { return _hostedOn; }
        set { SetProperty(ref _hostedOn, value); }
    }

    public string FileType
    {
        get { return _fileType; }
        set { SetProperty(ref _fileType, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return UserDocumentCacheId;
    }
}