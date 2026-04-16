using System;
using System.Linq;
using System.Reflection;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public class ReportingParameterInfo
{
    public ReportingParameterInfo()
    {

    }

    public ReportingParameterInfo(PropertyInfo argValue)
    {
        Name = argValue.Name;
        Type = argValue.PropertyType;
        AllowedOperators = ReportingOperators.GetForType(Type).ToArray();
    }

    public string Name { get; set; }
    public Type Type { get; set; }
    public ReportingParamterValue[] AllowedValues { get; set; }
    public ReportingOperator[] AllowedOperators { get; set; }
    public string Display { get; set; }
    public bool IsRequired { get; set; }
    public bool AllowOnce { get; set; }
}