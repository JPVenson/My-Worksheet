namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow
{
    public class WorksheetWorkflowStepTransitionViewModel : ViewModelBase
    {
        private WorksheetWorkflowStepViewModel _from;

        private WorksheetWorkflowStepViewModel[] _to;

        public WorksheetWorkflowStepViewModel From
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }

        public WorksheetWorkflowStepViewModel[] To
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }
    }
}