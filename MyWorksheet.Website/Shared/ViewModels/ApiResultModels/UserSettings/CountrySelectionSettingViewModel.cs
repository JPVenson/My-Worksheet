using System;
using System.ComponentModel.DataAnnotations;
using MyWorksheet.Public.Models.Attr;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings
{
    [SettingsElement("CountrySelectionSetting")]
    public class CountrySelectionSettingViewModel : ViewModelBase
    {
        private CountrySettingViewModel _selectedCountry;

        public CountrySelectionSettingViewModel()
        {
            SelectedCountry = new CountrySettingViewModel();
        }

        public CountrySettingViewModel SelectedCountry
        {
            get { return _selectedCountry; }
            set { SetProperty(ref _selectedCountry, value); }
        }

        public class CountrySettingViewModel : ViewModelBase
        {
            private string _currency;

            private bool _isActive;

            private Guid _promisedFeatureRegionId;

            private string _regionName;

            private string _regionShortName;

            [Required]
            public Guid PromisedFeatureRegionId
            {
                get { return _promisedFeatureRegionId; }
                set { SetProperty(ref _promisedFeatureRegionId, value); }
            }

            [Required]
            [StringLength(4)]
            public string RegionShortName
            {
                get { return _regionShortName; }
                set { SetProperty(ref _regionShortName, value); }
            }

            [Required]
            [StringLength(4)]
            public string Currency
            {
                get { return _currency; }
                set { SetProperty(ref _currency, value); }
            }

            [Required]
            public bool IsActive
            {
                get { return _isActive; }
                set { SetProperty(ref _isActive, value); }
            }

            [Required]
            [StringLength(20)]
            public string RegionName
            {
                get { return _regionName; }
                set { SetProperty(ref _regionName, value); }
            }

            public override Guid? GetModelIdentifier()
            {
                return PromisedFeatureRegionId;
            }
        }
    }
}