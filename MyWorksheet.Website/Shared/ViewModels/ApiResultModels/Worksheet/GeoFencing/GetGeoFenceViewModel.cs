using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.GeoFencing;

public class GetGeoFenceViewModel : ViewModelBase
{
    private GeoPositionViewModel[] _geoPositions;

    private GeoWiFiNameViewModel[] _geoWiFis;

    private Guid _idProject;

    private bool _isEnabled;

    private string _name;

    private Guid _worksheetGeoFenceId;

    public Guid WorksheetGeoFenceId
    {
        get { return _worksheetGeoFenceId; }
        set { SetProperty(ref _worksheetGeoFenceId, value); }
    }

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    public Guid IdProject
    {
        get { return _idProject; }
        set { SetProperty(ref _idProject, value); }
    }

    public bool IsEnabled
    {
        get { return _isEnabled; }
        set { SetProperty(ref _isEnabled, value); }
    }

    public GeoPositionViewModel[] GeoPositions
    {
        get { return _geoPositions; }
        set { SetProperty(ref _geoPositions, value); }
    }

    public GeoWiFiNameViewModel[] GeoWiFis
    {
        get { return _geoWiFis; }
        set { SetProperty(ref _geoWiFis, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetGeoFenceId;
    }
}