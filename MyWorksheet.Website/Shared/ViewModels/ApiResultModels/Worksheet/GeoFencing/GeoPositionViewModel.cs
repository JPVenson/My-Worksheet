using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.GeoFencing;

public class GeoPositionViewModel : ViewModelBase
{
    private int _fenceGroup;

    private decimal _latitude;

    private decimal _longitude;

    private Guid _worksheetGeoFenceId;

    public Guid WorksheetGeoFenceId
    {
        get { return _worksheetGeoFenceId; }
        set { SetProperty(ref _worksheetGeoFenceId, value); }
    }

    public decimal Longitude
    {
        get { return _longitude; }
        set { SetProperty(ref _longitude, value); }
    }

    public decimal Latitude
    {
        get { return _latitude; }
        set { SetProperty(ref _latitude, value); }
    }

    public int FenceGroup
    {
        get { return _fenceGroup; }
        set { SetProperty(ref _fenceGroup, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetGeoFenceId;
    }
}