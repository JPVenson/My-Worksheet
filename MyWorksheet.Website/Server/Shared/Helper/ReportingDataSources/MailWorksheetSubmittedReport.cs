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
using MyWorksheet.Website.Server.Services.ServerManager;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Services.WorksheetWorkflowManagment.MailFlow.TokenManager;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public class MailWorksheetSubmittedReport : IReportingDataSource
{
    private readonly IMapperService _mapperService;
    private readonly IServerManagerService _serverManagerService;
    private readonly IEMailWorkflowTokenManager _mailWorkflowTokenManager;

    public MailWorksheetSubmittedReport(IMapperService mapperService, IServerManagerService serverManagerService,
        IEMailWorkflowTokenManager mailWorkflowTokenManager)
    {
        _mapperService = mapperService;
        _serverManagerService = serverManagerService;
        _mailWorkflowTokenManager = mailWorkflowTokenManager;
        Id = new Guid("00000000-0000-0000-0004-000000000010");
        Key = ReportKey;
        Name = "Mail Workflow Submitted Report";
        Purpose = new[]
        {
            ReportPurposes.Worksheet
        };
    }
    public static string ReportKey { get; private set; } = "MailWorkflowSubReport";

    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }

    public class ReportArguments : ArgumentsBase
    {
        public static ReportArguments Parse(IDictionary<string, object> arguments)
        {
            var opts = new ReportArguments();
            opts.SetOrAbort<Guid>(arguments, e => opts.Worksheet = e, nameof(Worksheet));
            opts.Set<string>(arguments, e => opts.AdditionalInfos = e, nameof(AdditionalInfos));
            opts.Set<bool>(arguments, e => opts.UseParentOrg = e, nameof(UseParentOrg));
            return opts.GetIfValid() as ReportArguments;
        }

        [JsonDisplayKey("Entity/Worksheet")]
        public Guid Worksheet { get; set; }

        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.UseParentOrgAsAddress")]
        public bool UseParentOrg { get; set; }

        [JsonCanBeNull]
        [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.AdditionalInfos")]
        public string AdditionalInfos { get; set; }
    }

    public IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<MailWorkflowSubmittedReportModel>));
    }

    public IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        var projects = db.Projects.Where(f => f.IdCreator == userId).ToArray();

        var conditionalEvalQuery = db.Worksheets
            .Where(f => f.IdCreator == userId)
            .Where(e => e.IdCurrentStatus > WorksheetStatusType.Created.ConvertToGuid())
            .ToArray();

        var externalWorksheets = conditionalEvalQuery.OrderByDescending(e => e.EndTime).ToUniqeDictionary(e =>
                projects.First(f => f.ProjectId == e.IdProject).Name +
                " / " + e.StartTime.ToString("d") + " - " + e.EndTime?.ToString("d"),
            e => (object)e.WorksheetId);

        return JsonSchemaExtensions.JsonSchema(typeof(ReportArguments))
            .ExtendDefault(nameof(ReportArguments.Worksheet), externalWorksheets.FirstOrDefault().Value)
            .ExtendDefault(nameof(ReportArguments.UseParentOrg), true)
            .ExtendAllowedValues(nameof(ReportArguments.Worksheet), externalWorksheets);
    }

    public IDictionary<string, object> GetData(MyworksheetContext db, Guid userId, ReportingExecutionParameterValue[] query, IDictionary<string, object> arguments)
    {
        var reportArguments = ReportArguments.Parse(arguments);
        if (reportArguments == null)
        {
            return null;
        }

        var mapper = _mapperService.ViewModelMapper;
        var serverManager = _serverManagerService;
        var knownServer = serverManager.GetServerWith("UiServer").First();

        var dataModel = new MailWorkflowSubmittedReportModel();

        var worksheet = db.Worksheets.Include(e => e.IdProjectNavigation)
            .Where(e => e.WorksheetId == reportArguments.Worksheet)
            .FirstOrDefault();

        if (worksheet?.IdCreator != userId)
        {
            return null;
        }

        dataModel.AdditonalInfos = reportArguments.AdditionalInfos;

        var uri = new Uri(knownServer.HostName);
        var uriBuilder = new UriBuilder("https", uri.Host, uri.Port, "/browser-ui/Workflow/Mail/UpdateWorksheet");

        dataModel.Worksheet = mapper.Map<WorksheetModel>(worksheet);
        dataModel.Project = mapper.Map<GetProjectModel>(worksheet.IdProjectNavigation);
        dataModel.Creator = mapper.Map<AccountApiGet>(db.AppUsers.Find(worksheet.IdProjectNavigation.IdCreator));
        if (worksheet.IdProjectNavigation.IdOrganisation.HasValue)
        {
            dataModel.Owner = mapper.Map<OrganizationReportingViewModel>(db.Organisations.Find(worksheet.IdProjectNavigation.IdOrganisation));
            if (dataModel.Owner?.IdParentOrganisation != null)
            {
                dataModel.Owner.ParentOrganization =
                    mapper.Map<OrganizationViewModel>(db.Organisations.Find(dataModel.Owner?.IdParentOrganisation));
            }
        }

        dataModel.CreatorAddress = mapper.Map<AddressModel>(db.Addresses.Find(dataModel.Creator.IdAddress));
        if (dataModel.Owner != null)
        {
            if (reportArguments.UseParentOrg && dataModel.Owner.ParentOrganization?.IdAddress != null)
            {
                dataModel.OwnerAddress = mapper.Map<AddressModel>(db.Addresses.Find(dataModel.Owner.ParentOrganization?.IdAddress));
            }
            else
            {
                dataModel.OwnerAddress = mapper.Map<AddressModel>(db.Addresses.Find(dataModel.Owner.IdAddress));
            }
        }

        var code = _mailWorkflowTokenManager.Encode(new EMailWorkflowExternalToken()
        {
            WorksheetId = reportArguments.Worksheet,
            ValidUntil = DateTimeOffset.UtcNow.AddDays(7),
            UserId = userId,
            Signer = dataModel.OwnerAddress.FirstName + " " + dataModel.OwnerAddress.LastName
        });

        uriBuilder.Query = "id=" + reportArguments.Worksheet + "&accept=true&code=" + code;
        dataModel.AcceptUrl = uriBuilder.ToString();

        uriBuilder.Query = "id=" + reportArguments.Worksheet + "&accept=false&code=" + code;
        dataModel.RejectUrl = uriBuilder.ToString();

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