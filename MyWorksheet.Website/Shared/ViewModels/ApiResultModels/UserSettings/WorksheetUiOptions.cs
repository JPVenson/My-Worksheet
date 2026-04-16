using System.ComponentModel.DataAnnotations;
using MyWorksheet.Public.Models.Attr;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.UserSettings
{
    [SettingsElement("WorksheetUiOptions")]
    public class WorksheetUiOptions : ViewModelBase
    {
        private UiOptions _display;
        private UiOptions _print;
        private UiFilter _search;

        public WorksheetUiOptions()
        {
            Search = new UiFilter();
            Display = new UiOptions();
            Print = new UiOptions();
            BrowserLocalSettings = new ClientSettings();
        }

        public UiFilter Search
        {
            get { return _search; }
            set { SetProperty(ref _search, value); }
        }

        public UiOptions Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value); }
        }

        public UiOptions Print
        {
            get { return _print; }
            set { SetProperty(ref _print, value); }
        }

        public ClientSettings BrowserLocalSettings { get; set; }

        public class ClientSettings
        {
            public bool UseDecimalTimes { get; set; }
            public string ThemeName { get; set; }
            public string TimeLocale { get; set; }
        }

        public class UiFilter : ViewModelBase
        {
            private UiOrdering _elementsOrder;
            private string _filterObject;
            private bool _onlyPast;
            private bool _onlyWeekDays;
            private bool _orderDirection;
            private string _orderProp;
            private bool _workDays;

            public bool OnlyPast
            {
                get { return _onlyPast; }
                set { SetProperty(ref _onlyPast, value); }
            }

            public bool OnlyWeekDays
            {
                get { return _onlyWeekDays; }
                set { SetProperty(ref _onlyWeekDays, value); }
            }

            [MaxLength(25)]
            public string OrderProp
            {
                get { return _orderProp; }
                set { SetProperty(ref _orderProp, value); }
            }

            public bool OrderDirection
            {
                get { return _orderDirection; }
                set { SetProperty(ref _orderDirection, value); }
            }

            public string FilterObject
            {
                get { return _filterObject; }
                set { SetProperty(ref _filterObject, value); }
            }

            public UiOrdering ElementsOrder
            {
                get { return _elementsOrder; }
                set { SetProperty(ref _elementsOrder, value); }
            }

            public bool WorkDays
            {
                get { return _workDays; }
                set { SetProperty(ref _workDays, value); }
            }

            public class UiOrdering : ViewModelBase
            {
                private bool _filterDirection;
                private string _propertyName;

                [Required]
                [MaxLength(25)]
                public string PropertyName
                {
                    get { return _propertyName; }
                    set { SetProperty(ref _propertyName, value); }
                }

                public bool FilterDirection
                {
                    get { return _filterDirection; }
                    set { SetProperty(ref _filterDirection, value); }
                }
            }
        }

        public class UiOptions : ViewModelBase
        {
            private bool? _showAllDates;
            private bool? _showDetailedTimes;
            private bool? _showExtendedTimes;
            private bool? _showFullWorktimeOnDay;

            public bool? ShowExtendedTimes
            {
                get { return _showExtendedTimes; }
                set { SetProperty(ref _showExtendedTimes, value); }
            }

            public bool? ShowDetailedTimes
            {
                get { return _showDetailedTimes; }
                set { SetProperty(ref _showDetailedTimes, value); }
            }

            public bool? ShowFullWorktimeOnDay
            {
                get { return _showFullWorktimeOnDay; }
                set { SetProperty(ref _showFullWorktimeOnDay, value); }
            }

            public bool? ShowAllDates
            {
                get { return _showAllDates; }
                set { SetProperty(ref _showAllDates, value); }
            }
        }
    }
}