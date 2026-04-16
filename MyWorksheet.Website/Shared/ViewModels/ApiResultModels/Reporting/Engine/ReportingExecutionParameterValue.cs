namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine
{
    public class ReportingExecutionParameterValue : ReportingParamterValue
    {
        private string _logicalOperator;

        private bool _not;

        private ReportingRelationalOperator _relationalOperator;

        public string LogicalOperator
        {
            get { return _logicalOperator; }
            set { SetProperty(ref _logicalOperator, value); }
        }

        public bool Not
        {
            get { return _not; }
            set { SetProperty(ref _not, value); }
        }

        public ReportingRelationalOperator RelationalOperator
        {
            get { return _relationalOperator; }
            set { SetProperty(ref _relationalOperator, value); }
        }
    }
}