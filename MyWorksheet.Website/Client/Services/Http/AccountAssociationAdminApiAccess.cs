using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

namespace MyWorksheet.Website.Client.Services.Http;

public class ReportManagementApiAccess : HttpAccessBase
{
    public ReportManagementApiAccess(HttpService httpService) : base(httpService, "ReportManagementApi")
    {

    }

    public ValueTask<ApiResult<string>> GetJavascriptBaseLib()
    {
        return Get<string>(BuildApi("GetJavascriptBaseLib"));
    }

    public ValueTask<ApiResult<KeyValuePair<string, string>[]>> GetTemplateFormatter()
    {
        return Get<KeyValuePair<string, string>[]>(BuildApi("GetTemplateFormatter"));
    }

    public ValueTask<ApiResult<MorestachioParsedTemplateViewModel>> TokenizeTemplate()
    {
        return Get<MorestachioParsedTemplateViewModel>(BuildApi("TokenizeTemplate"));
    }

    public ValueTask<ApiResult<ScheduleReportSchemaModel>> GetSchemaForRds(string dataSource)
    {
        return Get<ScheduleReportSchemaModel>(BuildApi("GetSchemaForRds", new
        {
            dataSource
        }));
    }

    public ValueTask<ApiResult<JsonSchema>> GetEngineAddons(string reportingEngine)
    {
        return Get<JsonSchema>(BuildApi("GetEngineAddons", new
        {
            reportingEngine
        }));
    }

    public ValueTask<ApiResult<ReportingDataSourceViewModel[]>> Rds()
    {
        return Get<ReportingDataSourceViewModel[]>(BuildApi("Rds"));
    }

    public ValueTask<ApiResult<NEngineParameterInfo[]>> RdsArguments(string key)
    {
        return Get<NEngineParameterInfo[]>(BuildApi("RdsArguments", new
        {
            key
        }));
    }

    public ValueTask<ApiResult<Guid>> ScheduleReport(ScheduleReportModel model)
    {
        return Post<ScheduleReportModel, Guid>(BuildApi("ScheduleReport"), model);
    }
}

public class TemplateManagementApiAccess : HttpAccessBase
{
    public TemplateManagementApiAccess(HttpService httpService) : base(httpService, "TemplateManagementApi")
    {
    }

    public ValueTask<ApiResult<NEngineTemplateLookup[]>> GetTemplates(string purpose = null)
    {
        return Get<NEngineTemplateLookup[]>(BuildApi("Get", new
        {
            purpose
        }));
    }

    public ValueTask<ApiResult<NEngineTemplateLookup>> GetTemplate(Guid id)
    {
        return Get<NEngineTemplateLookup>(BuildApi("GetSingle", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<PageResultSet<NEngineTemplateLookup>>> GetTemplates(int page, int pageSize, string search = null)
    {
        return Get<PageResultSet<NEngineTemplateLookup>>(BuildApi("Search", new
        {
            page,
            pageSize,
            search
        }));
    }

    public ValueTask<ApiResult<string>> GetTemplateContent(Guid id)
    {
        return Get<string>(BuildApi("GetContent", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<NEngineTemplateLookup>> Create(NEngineTemplateCreate template)
    {
        return Post<NEngineTemplateCreate, NEngineTemplateLookup>(BuildApi("Create"), template);
    }

    public ValueTask<ApiResult<NEngineTemplateLookup>> Update(NEngineTemplateUpdate template)
    {
        return Post<NEngineTemplateUpdate, NEngineTemplateLookup>(BuildApi("Update"), template);
    }

    public ValueTask<ApiResult<string>> IssueReportDownloadToken(Guid reportId, int? historyEntry = null)
    {
        return Get<string>(BuildApi("IssueReportDownloadToken", new
        {
            reportId,
            historyEntry
        }));
    }


    public string DownloadPreviewUrl(Guid reportId, string token)
    {
        return BuildApi("DownloadPreviewTemplate", new
        {
            token,
            reportId = reportId,
            displayInline = true
        });
    }

    public string DownloadUrl(Guid runningTemplateId, string token)
    {
        return BuildApi("DownloadGeneratedTemplate", new
        {
            token,
            runningTemplateId
        });
    }

    public ValueTask<ApiResult> Delete(Guid id)
    {
        return Post(BuildApi("Delete", new { id }));
    }
}

public class AccountAssociationAdminApiAccess : HttpAccessBase
{
    public AccountAssociationAdminApiAccess(HttpService httpService) : base(httpService, "AccountUserAssociationApi/Admin")
    {
    }

    public ValueTask<ApiResult> RedeemInviteLink(Guid parentId, Guid childId, Guid roleId)
    {
        return Post(BuildApi("AddUserAssociation", new
        {
            parentId,
            childId,
            roleId
        }));
    }

    public ValueTask<ApiResult> RemoveUserAssociation(Guid parentId, Guid childId, Guid roleId)
    {
        return Post(BuildApi("RemoveUserAssociation", new
        {
            parentId,
            childId,
            roleId
        }));
    }

    public ValueTask<ApiResult<UserAssosiationModel[]>> GetUserAssociation(Guid id)
    {
        return Get<UserAssosiationModel[]>(BuildApi("GetUserAssociation", new
        {
            id
        }));
    }
}

public class EMailAccountApiAccess : RestHttpAccessBase<EMailAccountViewModel, EMailAccountViewModel>
{
    public EMailAccountApiAccess(HttpService httpService) : base(httpService, "EMailAccountApi")
    {
    }

    public ValueTask<ApiResult<EMailAccountViewModel[]>> Get()
    {
        return base.Get<EMailAccountViewModel[]>(BuildApi("Get"));
    }

    public ValueTask<ApiResult<MailSendViewModel[]>> GetHistory(Guid mailAccountId)
    {
        return base.Get<MailSendViewModel[]>(BuildApi("GetHistory", new { mailAccountId }));
    }

    //public ValueTask<ApiResult<EMailAccountViewModel>> Create(EMailAccountViewModel model)
    //{
    //	return base.Post<EMailAccountViewModel, EMailAccountViewModel>(BuildApi("Create"), model);
    //}

    //public ValueTask<ApiResult<EMailAccountViewModel>> Update(EMailAccountViewModel model)
    //{
    //	return base.Post<EMailAccountViewModel, EMailAccountViewModel>(BuildApi("Update"), model);
    //}

    //public ValueTask<ApiResult> Delete(Guid mailAccountId)
    //{
    //	return base.Post(BuildApi("Delete", new { mailAccountId }));
    //}
}