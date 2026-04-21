using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class ProjectAddressKindModel : ViewModelBase
{
    public Guid ProjectAddressMapLookupId { get; set; }
    public string Code { get; set; }
    public string DisplayKey { get; set; }
    public string DescriptionKey { get; set; }

    public override Guid? GetModelIdentifier()
    {
        return ProjectAddressMapLookupId;
    }
}