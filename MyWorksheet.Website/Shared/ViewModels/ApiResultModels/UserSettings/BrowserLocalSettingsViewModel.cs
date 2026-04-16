using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyWorksheet.Public.Models.Attr;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings
{
    [SettingsElement("BrowserLocalSettings")]
    public class BrowserLocalSettingsViewModel : ViewModelBase
    {
        private Theme _customTheme;

        private bool _fullscreenLayout = true;

        private bool _stayOnSmViewPort;

        private string _themeName;

        private string _timeLocale;

        private IList<TimeOfDayThemeSwitch> _timeOfDayThemeSwitches = new Collection<TimeOfDayThemeSwitch>();

        private bool _useDecimalTimes;

        private WysiwygEditorSettings _wysiwygEditor = new WysiwygEditorSettings();

        public bool StayOnSmViewPort
        {
            get { return _stayOnSmViewPort; }
            set { SetProperty(ref _stayOnSmViewPort, value); }
        }

        public bool FullscreenLayout
        {
            get { return _fullscreenLayout; }
            set { SetProperty(ref _fullscreenLayout, value); }
        }

        public string ThemeName
        {
            get { return _themeName; }
            set { SetProperty(ref _themeName, value); }
        }

        [FixedValues(null, "en", "de")]
        public string TimeLocale
        {
            get { return _timeLocale; }
            set { SetProperty(ref _timeLocale, value); }
        }

        public bool UseDecimalTimes
        {
            get { return _useDecimalTimes; }
            set { SetProperty(ref _useDecimalTimes, value); }
        }

        public Theme CustomTheme
        {
            get { return _customTheme; }
            set { SetProperty(ref _customTheme, value); }
        }

        public IList<TimeOfDayThemeSwitch> TimeOfDayThemeSwitches
        {
            get { return _timeOfDayThemeSwitches; }
            set { SetProperty(ref _timeOfDayThemeSwitches, value); }
        }

        public WysiwygEditorSettings WysiwygEditor
        {
            get { return _wysiwygEditor; }
            set { SetProperty(ref _wysiwygEditor, value); }
        }
    }
}