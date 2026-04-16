using System;
using System.Collections.Generic;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports
{
    public class ScheduleReportModel : ViewModelBase
    {
        private IDictionary<string, object> _arguments;

        private ReportingExecutionParameterValue[] _parameterValues;

        private bool _preview;

        private Guid? _storageProvider;

        private Guid _templateId;

        public ReportingExecutionParameterValue[] ParameterValues
        {
            get { return _parameterValues; }
            set { SetProperty(ref _parameterValues, value); }
        }

        public IDictionary<string, object> Arguments
        {
            get { return _arguments; }
            set { SetProperty(ref _arguments, value); }
        }

        public Guid TemplateId
        {
            get { return _templateId; }
            set { SetProperty(ref _templateId, value); }
        }

        public Guid? StorageProvider
        {
            get { return _storageProvider; }
            set { SetProperty(ref _storageProvider, value); }
        }

        public bool Preview
        {
            get { return _preview; }
            set { SetProperty(ref _preview, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return TemplateId;
        }
    }
}