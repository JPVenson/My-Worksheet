using System;
using System.Collections.Generic;
using System.Linq;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Webpage.Helper.Utlitiys;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class ProjectSpecReport : IReportingDataSource
{
    private readonly IMapperService _mapperService;

    public ProjectSpecReport(IMapperService mapperService)
    {
        _mapperService = mapperService;

        Id = new Guid("00000000-0000-0000-0004-000000000008");
        Key = ReportKey;
        Name = "Project Report";
        Purpose = Array.Empty<ReportPurpose>();
    }

    public class ReportArguments : ArgumentsBase
    {
        public ReportArguments()
        {

        }

        public ReportArguments(IDictionary<string, object> arguments)
        {

            TrySet(arguments, nameof(Project));
            TrySet(arguments, nameof(PaymentInfos));
            TrySet(arguments, nameof(Date));
            TrySet(arguments, nameof(InvoiceNo));
            TrySet<string>(arguments, f => AdditionalInfos = f, "Additonal");
            //Project = (long) arguments.GetOrNull("Project");
            //PaymentInfos = (long) arguments.GetOrNull("PaymentInfos");
            //Date = (DateTime?) arguments.GetOrNull("Date");
            //AdditionalInfos = (string) arguments.GetOrNull("Additonal");
            //InvoiceNo = (string) arguments.GetOrNull("InvoiceNo");
        }

        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.Project")]
        public long Project { get; set; }

        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.PaymentInfos")]
        public Guid PaymentInfos { get; set; }

        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.InvoiceNo")]
        public string InvoiceNo { get; set; }

        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.Date")]
        public DateTime? Date { get; set; }

        [JsonCanBeNull]
        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.AdditionalInfos")]
        public string AdditionalInfos { get; set; }
    }

    public static string ReportKey { get; private set; } = "ProjectSpecReport";
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }

    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<ProjectSpecReportModel>));
    }

    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        var projects = ProjectsInOrganisation.Query(db, userId)
            .Where(e => e.IdCreator == userId)
            .ToUniqeDictionary(e => e.UserOrderNo + " / " + e.Name, e => (object)e.ProjectId)
            .OrderBy(e => e.Key)
            .ToDictionary(e => e.Key, e => e.Value);

        var payment = db.PaymentInfos
            .Where(f => f.IdAppUser == userId)
            .ToDictionary(e => e.PaymentType, e => (object)e.PaymentInfoId);

        return JsonSchemaExtensions.JsonSchema(new ReportArguments())
            .ExtendDefault(nameof(ReportArguments.Project), projects.FirstOrDefault().Value)
            .ExtendDefault(nameof(ReportArguments.PaymentInfos), payment.FirstOrDefault().Value)
            .ExtendDefault("Date", DateTime.UtcNow)
            .ExtendDefault("AdditionalInfos", "")
            .ExtendAllowedValues(nameof(ReportArguments.Project), projects)
            .ExtendAllowedValues(nameof(ReportArguments.PaymentInfos), payment);
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId,
        ReportingExecutionParameterValue[] query, IDictionary<string, object> arguments)
    {
        var reportArguments = new ReportArguments(arguments);

        var dataModel = new ProjectSpecReportModel();

        var project = db.Projects.Find(reportArguments.Project);
        var paymentInfo = db.PaymentInfos.Find(reportArguments.PaymentInfos);

        if (project == null)
        {
            return null;
        }

        if (paymentInfo?.IdAppUser != userId)
        {
            return null;
        }

        dataModel.AdditonalInfos = reportArguments.AdditionalInfos;
        dataModel.InvoiceNo = reportArguments.InvoiceNo;

        dataModel.Worksheets = _mapperService.ViewModelMapper.Map<List<WorksheetModel>>(db.Worksheets.Where(f => f.IdProject == project.ProjectId).ToArray());
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
            dataModel.OwnerAddress = _mapperService.ViewModelMapper.Map<AddressModel>(db.Addresses.Find(dataModel.Owner.IdAddress));
        }

        dataModel.WorksheetItems = _mapperService.ViewModelMapper.Map<List<WorksheetItemModel>>(db.WorksheetItems
            .Where(e => dataModel.Worksheets.Select(e => e.WorksheetId).Contains(e.IdWorksheet)));

        dataModel.PaymentInfos = _mapperService.ViewModelMapper.Map<List<PaymentInfoContentModel>>(db.PaymentInfoContents
            .Where(f => f.IdPaymentInfo == reportArguments.PaymentInfos)
            .Where(f => f.IdAppUser == userId)
            .ToArray());
        dataModel.PaymentInfoGroup = _mapperService.ViewModelMapper.Map<PaymentInfoModel>(paymentInfo);

        dataModel.Infos = new ProjectSpecReportModel.Aggregations();
        dataModel.Infos.WorkedMinutes =
            dataModel.WorksheetItems.Select(e => e.ToTime - e.FromTime).Aggregate((e, f) => e + f);

        //dataModel.Infos.CalculatedNet =
        //	((Math.Round(dataModel.Infos.WorkedMinutes / 60, 2)) * (decimal) project.Honorar);
        //dataModel.Infos.CalculatedTax = Math.Round(dataModel.Infos.CalculatedNet / 100 * project.TaxRate, 2);
        //dataModel.Infos.CalculatedGross =
        //	Math.Round(dataModel.Infos.CalculatedNet + dataModel.Infos.CalculatedTax, 2);
        dataModel.Infos.DateOfCreation = reportArguments.Date ?? DateTime.UtcNow;

        return new Dictionary<string, object>()
        {
            {"DataSource", dataModel}
        };
    }

    public ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        return new ReportingParameterInfo[0];
    }
}