using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;

namespace MyWorksheet.Website.Shared.ViewModels.Workflow;

public class MailAppUser : AccountApiUserPost
{
    public AddressModel Address
    {
        get { return _address; }
        set { SetProperty(ref _address, value); }
    }

    private AddressModel _address;
}
