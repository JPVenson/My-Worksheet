using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow
{
    public class WorksheetWorkflowStepViewModel : ViewModelBase
    {
        private string _display;

        private Guid _value;

        public Guid Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public string Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value); }
        }
    }
}