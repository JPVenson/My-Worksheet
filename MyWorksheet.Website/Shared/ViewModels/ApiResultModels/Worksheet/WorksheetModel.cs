using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    [ObjectTracking("Worksheet")]
    public class WorksheetModel : ViewModelBase
    {
        private DateTimeOffset? _endTime;

        private Guid _idCreator;

        private Guid? _idCurrentStatus;

        private Guid _idProject;

        private Guid _idProjectItemRate;

        private Guid? _idWorksheetWorkflow;

        private Guid? _idWorksheetWorkflowDataMap;

        private string _no;

        private string _numberRangeEntry;

        private string _serviceDescription;

        private DateTimeOffset _startTime;

        private bool _submitted;

        private Guid _worksheetId;
        private DateTimeOffset? _invoiceDueDate;

        public DateTimeOffset? InvoiceDueDate
        {
            get { return _invoiceDueDate; }
            set { SetProperty(ref _invoiceDueDate, value); }
        }

        public Guid WorksheetId
        {
            get { return _worksheetId; }
            set { SetProperty(ref _worksheetId, value); }
        }

        public DateTimeOffset StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        public DateTimeOffset? EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        public bool Submitted
        {
            get { return _submitted; }
            set { SetProperty(ref _submitted, value); }
        }

        public string ServiceDescription
        {
            get { return _serviceDescription; }
            set { SetProperty(ref _serviceDescription, value); }
        }

        public string No
        {
            get { return _no; }
            set { SetProperty(ref _no, value); }
        }

        public Guid IdProject
        {
            get { return _idProject; }
            set { SetProperty(ref _idProject, value); }
        }

        public Guid IdCreator
        {
            get { return _idCreator; }
            set { SetProperty(ref _idCreator, value); }
        }

        public Guid? IdWorksheetWorkflow
        {
            get { return _idWorksheetWorkflow; }
            set { SetProperty(ref _idWorksheetWorkflow, value); }
        }

        public Guid? IdWorksheetWorkflowDataMap
        {
            get { return _idWorksheetWorkflowDataMap; }
            set { SetProperty(ref _idWorksheetWorkflowDataMap, value); }
        }

        public Guid? IdCurrentStatus
        {
            get { return _idCurrentStatus; }
            set { SetProperty(ref _idCurrentStatus, value); }
        }

        public Guid IdProjectItemRate
        {
            get { return _idProjectItemRate; }
            set { SetProperty(ref _idProjectItemRate, value); }
        }

        public string NumberRangeEntry
        {
            get { return _numberRangeEntry; }
            set { SetProperty(ref _numberRangeEntry, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return WorksheetId;
        }
    }
}