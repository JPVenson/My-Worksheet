using System;
using Morestachio.Formatter.Framework.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class MonetaryCalculatorFormatters
{
    [MorestachioFormatter("GetSum", "Get the Sum of Honoar for a given ChargeRate and Rate")]
    public static double GetSum(
        [SourceObject] int worktime,
        string chargeRateCode,
        double rate)
    {
        var result = -1D;
        if (chargeRateCode == "PER_HOUR")
        {
            result = (worktime / 60D) * rate;
        }
        if (chargeRateCode == "PER_MINUTE")
        {
            result = (worktime) * rate;
        }
        if (chargeRateCode == "PER_QUARTER_MINUTE")
        {
            result = (worktime / 15) * rate;
        }
        if (chargeRateCode == "PER_DAY")
        {
            result = 0;
        }
        if (chargeRateCode == "PER_STARTED_HOUR")
        {
            var hours = worktime / 60;
            var fraction = hours % 60;
            if (fraction != 0)
            {
                hours -= hours % 1;
                hours += 1;
            }
            result = hours * rate;
        }

        return Math.Round(result, 2);
    }
}