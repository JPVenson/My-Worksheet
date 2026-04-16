using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting
{
    public class NEngineParameterInfo : ViewModelBase
    {
        private ReportingOperator[] _allowedOperators;
        private ReportingParamterValue[] _allowedValues;
        private string _display;
        private string _name;
        private string _type;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value); }
        }

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public ReportingParamterValue[] AllowedValues
        {
            get { return _allowedValues; }
            set { SetProperty(ref _allowedValues, value); }
        }

        public ReportingOperator[] AllowedOperators
        {
            get { return _allowedOperators; }
            set { SetProperty(ref _allowedOperators, value); }
        }
    }
}