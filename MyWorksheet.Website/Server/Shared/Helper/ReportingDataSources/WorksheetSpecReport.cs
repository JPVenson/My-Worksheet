using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class WorksheetSpecReport : IWorkflowTemplate
{
    private readonly IMapperService _mapperService;

    public WorksheetSpecReport(IMapperService mapperService)
    {
        _mapperService = mapperService;
        Id = new Guid("00000000-0000-0000-0004-000000000005");
        Key = ReportKey;
        Name = "Worksheet Report";
        Purpose = new[]
        {
            ReportPurposes.Worksheet
        };
    }

    public static string ReportKey { get; private set; } = "WorksheetSpecReport";

    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    public string Key { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }

    /// <inheritdoc />
    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<WorksheetSpecReportModel>));
    }

    /// <inheritdoc />
    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        var projects = db.Projects.Where(f => f.IdCreator == userId).ToArray();

        var conditionalEvalQuery = db.Worksheets
            .Where(f => f.IdCreator == userId)
            .Where(f => f.IdCurrentStatus > WorksheetStatusType.Created.ConvertToGuid())
            .ToArray();

        var externalWorksheets = conditionalEvalQuery.OrderByDescending(e => e.EndTime).ToUniqeDictionary(e =>
            projects.First(f => f.ProjectId == e.IdProject).Name +
            " / " + e.StartTime.ToString("d") + " - " +
            e.EndTime.GetValueOrDefault().ToString("d"), e => (object)e.WorksheetId);

        var paymentInfos = db.PaymentInfos
            .Where(f => f.IdAppUser == userId);
        var payment = paymentInfos
            .ToDictionary(e => e.PaymentType, e => (object)e.PaymentInfoId);

        return JsonSchemaExtensions.JsonSchema(typeof(WorkflowReportArguments))
            .ExtendDefault("Worksheet", externalWorksheets.FirstOrDefault().Value)
            .ExtendDefault("PaymentInfos", payment.FirstOrDefault().Value)
            .ExtendDefault(nameof(WorkflowReportArguments.RoundBy), 2)
            .ExtendDefault(nameof(WorkflowReportArguments.UseParentOrg), true)
            .ExtendDefault(nameof(WorkflowReportArguments.TimeDisplayArg), "d")
            .ExtendAllowedValues("Worksheet", externalWorksheets)
            .ExtendAllowedValues("PaymentInfos", payment);
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query,
        IDictionary<string, object> arguments)
    {
        var reportArguments = WorkflowReportArguments.Parse(arguments);
        if (reportArguments == null)
        {
            return null;
        }
        return GetData(db, userId, query, reportArguments);
    }

    /// <inheritdoc />
    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        return new ReportingParameterInfo[0];
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId,
        ReportingExecutionParameterValue[] query,
        WorkflowReportArguments reportArguments)
    {
        var dataModel = new WorksheetSpecReportModel();

        var worksheet = db.Worksheets.Find(reportArguments.Worksheet);
        var paymentInfo = db.PaymentInfos.Find(reportArguments.PaymentInfos);

        if (worksheet?.IdCreator != userId)
        {
            return null;
        }

        if (paymentInfo?.IdAppUser != userId)
        {
            return null;
        }

        var itemRates = db.ProjectItemRates
            .Include(f => f.IdProjectChargeRateNavigation)
            .Where(f => f.IdProject == worksheet.IdProject)
            .ToArray();

        dataModel.AdditonalInfos = reportArguments.AdditionalInfos;

        var project = db.Projects.Find(worksheet.IdProject);

        dataModel.Worksheet = _mapperService.ViewModelMapper.Map<WorksheetModel>(worksheet);
        dataModel.ItemRates = _mapperService.ViewModelMapper.Map<ProjectItemRateViewModel[]>(itemRates);
        dataModel.Project = _mapperService.ViewModelMapper.Map<GetProjectModel>(project);
        dataModel.Creator = _mapperService.ViewModelMapper.Map<AccountApiGet>(db.AppUsers.Find(project.IdCreator));
        if (project.IdOrganisation.HasValue)
        {
            dataModel.Owner =
                _mapperService.ViewModelMapper.Map<OrganizationReportingViewModel>(db.Organisations.Find(project.IdOrganisation));
            if (dataModel.Owner?.IdParentOrganisation != null)
            {
                dataModel.Owner.ParentOrganization =
                    _mapperService.ViewModelMapper.Map<OrganizationViewModel>(
                        db.Organisations.Find(dataModel.Owner?.IdParentOrganisation));
            }
        }

        dataModel.CreatorAddress = _mapperService.ViewModelMapper.Map<AddressModel>(db.Addresses.Find(dataModel.Creator.IdAddress));
        if (dataModel.Owner != null)
        {
            if (reportArguments.UseParentOrg && dataModel.Owner.ParentOrganization?.IdAddress != null)
            {
                dataModel.OwnerAddress = _mapperService.ViewModelMapper.Map<AddressModel>(db.Addresses.Find(dataModel.Owner.ParentOrganization?.IdAddress));
            }
            else
            {
                dataModel.OwnerAddress = _mapperService.ViewModelMapper.Map<AddressModel>(db.Addresses.Find(dataModel.Owner.IdAddress));
            }
        }

        dataModel.WorksheetItems = _mapperService.ViewModelMapper.Map<List<WorksheetItemModel>>(db.WorksheetItems
            .Where(e => e.IdWorksheet == reportArguments.Worksheet));

        dataModel.PaymentInfos = _mapperService.ViewModelMapper.Map<List<PaymentInfoContentModel>>(db.PaymentInfoContents
            .Where(f => f.IdPaymentInfo == reportArguments.PaymentInfos)
            .Where(f => f.IdAppUser == userId)
            .ToArray());
        dataModel.Asserts = _mapperService.ViewModelMapper.Map<List<WorksheetAssertGetViewModel>>(db.WorksheetAsserts
            .Where(f => f.IdWorksheet == reportArguments.Worksheet)
            .ToArray());
        dataModel.PaymentInfoGroup = _mapperService.ViewModelMapper.Map<PaymentInfoModel>(paymentInfo);

        dataModel.WorktimeInfos = new WorksheetSpecReportModel.WorktimeAggregations();
        if (dataModel.WorksheetItems.Any())
        {
            dataModel.WorktimeInfos.WorkedMinutes = dataModel.WorksheetItems
                .Select(e => e.ToTime - e.FromTime)
                .Aggregate((e, f) => e + f);

            dataModel.WorktimeInfos.CalculatedNet =
                (decimal)dataModel.WorksheetItems
                    .Select(e =>
                        Math.Round((e.ToTime - e.FromTime) / 60D, reportArguments.RoundBy) *
                        (double)itemRates.First(f => f.ProjectItemRateId == e.IdProjectItemRate).Rate)
                    .Aggregate((e, f) => e + f);
            dataModel.WorktimeInfos.CalculatedTax =
                (decimal)dataModel.WorksheetItems
                    .Select(e =>
                    {
                        var projectItemRate = itemRates.First(f => f.ProjectItemRateId == e.IdProjectItemRate);
                        return Math.Round(Math.Round((e.ToTime - e.FromTime) / 60D, reportArguments.RoundBy) *
                            (double)projectItemRate.Rate / 100 * (double)projectItemRate.TaxRate,
                            reportArguments.RoundBy);
                    })
                    .Aggregate((e, f) => e + f);
        }

        if (dataModel.Asserts.Any())
        {
            dataModel.WorktimeInfos.CalculatedNet +=
                dataModel.Asserts
                    .Select(e =>
                        Math.Round(e.Value, reportArguments.RoundBy))
                    .Aggregate((e, f) => e + f);
            dataModel.WorktimeInfos.CalculatedTax +=
                (decimal)dataModel.Asserts
                    .Select(e => Math.Round((e.Value) / 100 * e.Tax, reportArguments.RoundBy))
                    .Aggregate((e, f) => e + f);
        }



        dataModel.WorktimeInfos.CalculatedGross =
            Math.Round(dataModel.WorktimeInfos.CalculatedNet + dataModel.WorktimeInfos.CalculatedTax, reportArguments.RoundBy);

        //((Math.Round(dataModel.WorktimeInfos.WorkedMinutes / 60, reportArguments.RoundBy)) * (decimal) project.Honorar);
        //dataModel.WorktimeInfos.CalculatedTax = Math.Round(dataModel.WorktimeInfos.CalculatedNet / 100 * project.TaxRate, reportArguments.RoundBy);

        dataModel.AssertInfos = new WorksheetSpecReportModel.AssertAggregations()
        {
            TotalTax = dataModel.Asserts.Select(e => Math.Round(e.Value / 100 * e.Tax, reportArguments.RoundBy)).Sum(),
            TotalValue = dataModel.Asserts.Select(e => Math.Round(e.Value, reportArguments.RoundBy)).Sum()
        };

        dataModel.Infos = new WorksheetSpecReportModel.WorksheetAggregations()
        {
            CalculatedGross = dataModel.WorktimeInfos.CalculatedGross + dataModel.AssertInfos.TotalValue
        };

        dataModel.DateOfCreation = reportArguments.Date ?? DateTime.UtcNow;

        return new Dictionary<string, object>()
        {
            {"DataSource", dataModel}
        };
    }
}