using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator.Attributes;
using MyWorksheet.Website.Shared.Services;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

[SingletonService()]
public class InitReportDataSources : RequireInit
{
    public override ValueTask InitAsync(IServiceProvider services)
    {
        ReportDataSources.Init(services.GetService<ActivatorService>());
        return base.InitAsync(services);
    }
}

public static class ReportDataSources
{
    public static void Init(ActivatorService activatorService)
    {
        ProjectSpecReport = activatorService.ActivateType<ProjectSpecReport>();
        WorkflowMailReportDataSource = activatorService.ActivateType<WorkflowMailReportDataSource>();
        WorksheetSpecReport = activatorService.ActivateType<WorksheetSpecReport>();
        FullProjectTreeData = activatorService.ActivateType<FullTreeDataSource>();
        SubmittedWorksheets = activatorService.ActivateType<SubmittedWorksheetDataSource>();
        Worksheets = activatorService.ActivateType<WorksheetDataSource>();
        WorksheetItemsPerDay = activatorService.ActivateType<WorksheetItemsPerDayDataSource>();
        WorksheetItems = activatorService.ActivateType<WorksheetItemsDataSource>();
        Projects = activatorService.ActivateType<ProjectDataSource>();
        MailWorksheetSubmittedReport = activatorService.ActivateType<MailWorksheetSubmittedReport>();
    }

    public static IReportingDataSource Projects { get; private set; }
    public static IReportingDataSource WorksheetItems { get; private set; }
    public static IReportingDataSource WorksheetItemsPerDay { get; private set; }
    public static IReportingDataSource Worksheets { get; private set; }
    public static IReportingDataSource SubmittedWorksheets { get; private set; }
    public static IReportingDataSource FullProjectTreeData { get; private set; }
    public static IReportingDataSource WorksheetSpecReport { get; set; }
    public static IReportingDataSource WorkflowMailReportDataSource { get; set; }
    public static IReportingDataSource ProjectSpecReport { get; set; }
    public static IReportingDataSource MailWorksheetSubmittedReport { get; set; }

    public static IEnumerable<IReportingDataSource> Yield()
    {
        yield return Projects;
        yield return Worksheets;
        yield return SubmittedWorksheets;
        yield return WorksheetItems;
        yield return WorksheetItemsPerDay;
        yield return FullProjectTreeData;
        yield return WorksheetSpecReport;
        yield return ProjectSpecReport;
        yield return MailWorksheetSubmittedReport;
        //yield return WorkflowMailReportDataSource;
    }
}