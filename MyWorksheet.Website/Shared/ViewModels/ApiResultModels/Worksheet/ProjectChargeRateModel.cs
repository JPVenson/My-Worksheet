using System;
using Morestachio.Formatter.Predefined.Accounting;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    public class ProjectChargeRateModel : ViewModelBase
    {
        private string _code;

        private string _displayKey;

        private Guid _projectChargeRateId;

        public Guid ProjectChargeRateId
        {
            get { return _projectChargeRateId; }
            set { SetProperty(ref _projectChargeRateId, value); }
        }

        public string DisplayKey
        {
            get { return _displayKey; }
            set { SetProperty(ref _displayKey, value); }
        }

        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        public MoneyChargeRate ToMRate()
        {
            if (Code == "PER_HOUR")
            {
                return MoneyChargeRate.PerHour;
            }
            else if (Code == "PER_MINUTE")
            {
                return MoneyChargeRate.PerMinute;
            }
            else if (Code == "PER_QUARTER_MINUTE")
            {
                return MoneyChargeRate.PerQuarterHour;
            }
            else if (Code == "PER_DAY")
            {
                return MoneyChargeRate.PerDay;
            }
            else if (Code == "PER_STARTED_HOUR")
            {
                return MoneyChargeRate.PerStartedHour;
            }

            throw new InvalidOperationException("Rate type not found");
        }

        public override Guid? GetModelIdentifier()
        {
            return ProjectChargeRateId;
        }
    }
}