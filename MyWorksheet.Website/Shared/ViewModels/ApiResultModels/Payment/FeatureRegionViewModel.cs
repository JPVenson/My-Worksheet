using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;

public class FeatureRegionViewModel : ViewModelBase
{
    private Guid _promisedFeatureRegionId;
    private string _regionName;
    private string _regionShortName;
    private string _currency;
    private bool _isActive;
    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public string Currency
    {
        get { return _currency; }
        set { _currency = value; }
    }

    public string RegionShortName
    {
        get { return _regionShortName; }
        set { _regionShortName = value; }
    }

    public string RegionName
    {
        get { return _regionName; }
        set { _regionName = value; }
    }

    public Guid PromisedFeatureRegionId
    {
        get { return _promisedFeatureRegionId; }
        set { _promisedFeatureRegionId = value; }
    }

}
