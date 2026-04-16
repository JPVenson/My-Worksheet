using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.Reporting;

public static class DataSourceHelper
{
    public static List<ReportingParameterInfo> FillRemaining<T>(List<ReportingParameterInfo> source, Guid userId, params string[] exclude)
    {
        foreach (var type in typeof(T).GetProperties())
        {
            if (source.Any(e => e.Name.Equals(type.Name, StringComparison.OrdinalIgnoreCase)) || exclude.Contains(type.Name))
            {
                continue;
            }

            var op = new ReportingParameterInfo(type);
            if (type.PropertyType == typeof(bool))
            {
                op.AllowedValues = new[]
                {
                    new ReportingParamterValue("Yes", true),
                    new ReportingParamterValue("No", false),
                };
            }
            source.Add(op);
        }
        return source;
    }

    public static IEnumerable<CompairsonResult> CompareIncommingArguments(ReportingParameterInfo[] source,
        ReportingExecutionParameterValue[] incomming)
    {
        var arguments = new List<CompairsonResult>();

        foreach (var sourceParameter in source)
        {
            if (sourceParameter.IsRequired)
            {
                if (!incomming.Any(e => e.Name.Equals(sourceParameter.Name)))
                {
                    arguments.Add(new CompairsonResult() { Name = sourceParameter.Name, Text = "Required but missing" });
                }
            }

            if (sourceParameter.AllowOnce)
            {
                if (incomming.SingleOrDefault(e => e.Name.Equals(sourceParameter.Name)) == null)
                {
                    arguments.Add(new CompairsonResult() { Name = sourceParameter.Name, Text = "Required once but missing or more then 1" });
                }
            }
        }

        foreach (var reportingExecutionParameterValue in incomming)
        {
            if (string.IsNullOrWhiteSpace(reportingExecutionParameterValue.Name))
            {
                arguments.Add(new CompairsonResult()
                {
                    Text = "Name is missing"
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(reportingExecutionParameterValue.Value?.ToString()))
            {
                arguments.Add(new CompairsonResult()
                {
                    Name = reportingExecutionParameterValue.Name,
                    Text = "Value is null"
                });
                continue;
            }

            var inSource = source.FirstOrDefault(e => e.Name == reportingExecutionParameterValue.Name);
            if (inSource == null)
            {
                arguments.Add(new CompairsonResult()
                {
                    Name = reportingExecutionParameterValue.Name,
                    Text = "Does not exist"
                });
                continue;
            }
            if (inSource.AllowedValues != null && inSource.AllowedValues.Any() && !inSource.AllowedValues.Any(f =>
                {
                    if (f.Value.Equals(reportingExecutionParameterValue.Value))
                    {
                        return true;
                    }
                    reportingExecutionParameterValue.Value = Convert.ChangeType(reportingExecutionParameterValue.Value, f.Value.GetType());
                    return f.Value.Equals(reportingExecutionParameterValue.Value);
                }))
            {
                arguments.Add(new CompairsonResult()
                {
                    Name = reportingExecutionParameterValue.Name,
                    Text = "Non Allowed value"
                });
            }
            if (inSource.AllowedOperators.All(e => e.Display != reportingExecutionParameterValue.LogicalOperator))
            {
                arguments.Add(new CompairsonResult()
                {
                    Name = reportingExecutionParameterValue.Name,
                    Text = "Non Allowed Operator"
                });
            }

            if (reportingExecutionParameterValue.Value.GetType() == inSource.Type)
            {
                continue;
            }
            if (inSource.Type.IsInstanceOfType(reportingExecutionParameterValue.Value))
            {
                continue;
            }
            if (typeof(IConvertible).IsAssignableFrom(inSource.Type))
            {
                try
                {
                    reportingExecutionParameterValue.Value = Convert.ChangeType(reportingExecutionParameterValue.Value, inSource.Type);
                }
                catch
                {
                    arguments.Add(new CompairsonResult()
                    {
                        Name = reportingExecutionParameterValue.Name,
                        Text = "Invalid Value. Invalid Structure"
                    });
                }
            }
            else if (typeof(DateTimeOffset) == inSource.Type)
            {
                DateTimeOffset dto;
                if (!DateTimeOffset.TryParse(reportingExecutionParameterValue.Value?.ToString(), out dto))
                {
                    arguments.Add(new CompairsonResult()
                    {
                        Name = reportingExecutionParameterValue.Name,
                        Text = "Invalid Value. Invalid Structure"
                    });
                    continue;
                }

                reportingExecutionParameterValue.Value = dto;
            }
            else
            {
                arguments.Add(new CompairsonResult()
                {
                    Name = reportingExecutionParameterValue.Name,
                    Text = "Invalid Value. Invalid Structure"
                });
            }

        }
        return arguments;
    }

    public static ReportingExecutionParameterValue[] MapQuery<T>(ReportingExecutionParameterValue[] arguments)
    {
        return arguments;
        // var typeCache = config.GetOrCreateClassInfoCache(typeof(T));

        // foreach (var arg in arguments)
        // {
        //     arg.Name = typeCache.Propertys[arg.Name].DbName;
        // }

        // return arguments;
    }

    public static IQueryable<T> TranslateQuery<T>(IQueryable<T> source, ReportingExecutionParameterValue[] incomming)
    {
        if (incomming.Any())
        {
            return source;
        }

        var argument = Expression.Parameter(typeof(T));

        Expression Parse(ReportingExecutionParameterValue reportingExecutionParameterValue)
        {
            var property = Expression.Property(argument, reportingExecutionParameterValue.Name);

            switch (reportingExecutionParameterValue.LogicalOperator)
            {
                case "Equals":
                    return Expression.Equal(property, Expression.Constant(reportingExecutionParameterValue.Value));
                case "Not Equals":
                    return Expression.NotEqual(property, Expression.Constant(reportingExecutionParameterValue.Value));
                case "Bigger as":
                    return Expression.GreaterThan(property, Expression.Constant(reportingExecutionParameterValue.Value));
                case "Smaller as":
                    return Expression.LessThan(property, Expression.Constant(reportingExecutionParameterValue.Value));
            }
            throw new InvalidOperationException("Does not yet support argument");
        }

        var condition = Parse(incomming.First());

        foreach (var reportingExecutionParameterValue in incomming.Skip(1))
        {
            switch (reportingExecutionParameterValue.RelationalOperator)
            {
                case ReportingRelationalOperator.And:
                    condition = Expression.And(condition, Parse(reportingExecutionParameterValue));
                    break;
                case ReportingRelationalOperator.Or:
                    condition = Expression.Or(condition, Parse(reportingExecutionParameterValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return source.Where(Expression.Lambda<Func<T, bool>>(condition));
    }

    public static string CreateQueryFromArguments(ScheduleReportModel arguments)
    {
        return JsonConvert.SerializeObject(arguments, Formatting.None, new JsonSerializerSettings()
        {
            Converters = [
                new Newtonsoft.Json.Converters.StringEnumConverter()
            ]
        });
        //var sb = new List<string>();
        //for (var index = 0; index < arguments.Length; index++)
        //{
        //	var reportingExecutionParameterValue = arguments[index];
        //	sb.Add(reportingExecutionParameterValue.Name + " " + reportingExecutionParameterValue.LogicalOperator + " " +
        //		   reportingExecutionParameterValue.Value);
        //	if (index != arguments.Length - 1)
        //	{
        //		sb.Add(reportingExecutionParameterValue.RelationalOperator.ToString().ToUpper());
        //	}
        //}

        //return sb.AggregateIf(e => e.Any(), (e, f) => e + " " + f);
    }
}