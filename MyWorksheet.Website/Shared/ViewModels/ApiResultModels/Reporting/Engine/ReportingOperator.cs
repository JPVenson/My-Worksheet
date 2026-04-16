using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine
{
    public class ReportingOperator : ViewModelBase
    {
        private string _display;

        private string _helpText;

        private string _value;

        private Func<object, object> _valueFormatter;

        public ReportingOperator()
        {
            ValueFormatter = o => o;
        }

        public string Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string HelpText
        {
            get { return _helpText; }
            set { SetProperty(ref _helpText, value); }
        }

        public Func<object, object> ValueFormatter
        {
            get { return _valueFormatter; }
            set { SetProperty(ref _valueFormatter, value); }
        }

        public string EmitOperator()
        {
            return Value;
        }

        public object EmitValue(object value)
        {
            return ValueFormatter(value);
        }
    }
}