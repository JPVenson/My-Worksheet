using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.GeoFencing
{
    public class GeoWiFiNameViewModel : ViewModelBase
    {
        private string _key;

        private string _name;

        private Guid _worksheetGeoFenceWiFiId;

        public Guid WorksheetGeoFenceWiFiId
        {
            get { return _worksheetGeoFenceWiFiId; }
            set { SetProperty(ref _worksheetGeoFenceWiFiId, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return WorksheetGeoFenceWiFiId;
        }
    }
}