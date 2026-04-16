using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow
{
    public class WorksheetWorkflowDataMapViewModel : CreateWorksheetWorkflowDataMapViewModel
    {
        private Guid _idCreator;

        private Guid _worksheetWorkflowDataMapId;

        public Guid WorksheetWorkflowDataMapId
        {
            get { return _worksheetWorkflowDataMapId; }
            set { SetProperty(ref _worksheetWorkflowDataMapId, value); }
        }

        public Guid IdCreator
        {
            get { return _idCreator; }
            set { SetProperty(ref _idCreator, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return WorksheetWorkflowDataMapId;
        }
    }
}