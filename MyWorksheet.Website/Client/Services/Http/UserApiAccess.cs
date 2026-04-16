using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Http;

public class PaymentInfoApiAccess : RestHttpAccessBase<PaymentInfoModel>
{
    public PaymentInfoApiAccess(HttpService httpService) : base(httpService, "PaymentInfoApi")
    {
    }

    public ValueTask<ApiResult<PaymentInfoModel[]>> Get()
    {
        return Get<PaymentInfoModel[]>(BuildApi("Get"));
    }

    public ValueTask<ApiResult<PaymentInfoModel>> Get(Guid id)
    {
        return Get<PaymentInfoModel>(BuildApi("GetSingle", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<PaymentInfoContentModel[]>> GetFields(Guid id)
    {
        return Get<PaymentInfoContentModel[]>(BuildApi("GetFields", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<PaymentInfoModel>> Create(UpdatePaymentInfoModel model)
    {
        return Post<UpdatePaymentInfoModel, PaymentInfoModel>(BuildApi("Create"), model);
    }

    public ValueTask<ApiResult<PaymentInfoModel>> Update(UpdatePaymentInfoModel model)
    {
        return Post<UpdatePaymentInfoModel, PaymentInfoModel>(BuildApi("Update"), model);
    }
}

public class UserApiAccess : RestHttpAccessBase<AccountApiAdminGet, AccountApiAdminPost>
{
    public UserApiAccess(HttpService httpService)
        : base(httpService, "AccountApi")
    {
    }

    public ValueTask<ApiResult<AccountApiUserGet>> GetCurrentUserData()
    {
        return Get<AccountApiUserGet>(BuildApi("CurrentUserData"));
    }
}

//public class WorksheetItemStatusAccess : RestHttpAccessBase<WorksheetItemStatusLookup>
//{
//	public WorksheetItemStatusAccess(HttpService httpService)
//		: base(httpService, "WorksheetItemStatusApi")
//	{
//	}

//	public ValueTask<ApiResult<AccountApiUserGet>> GetCurrentUserData()
//	{
//		return Get<AccountApiUserGet>(BuildApi("CurrentUserData"));
//	}
//}


public class SchedulerApiAccess : HttpAccessBase
{
    public SchedulerApiAccess(HttpService httpService)
        : base(httpService, "SchedulerApi")
    {
    }

    public ValueTask<ApiResult<SchedulerInfo[]>> GetSchedulerInfos()
    {
        return Get<SchedulerInfo[]>(BuildApi("SchedulerInfos"));
    }

    public ValueTask<ApiResult> RunTask(string schedulerTaskName)
    {
        return Post(BuildApi("Run", new
        {
            name = schedulerTaskName
        }));
    }
}


public class ProjectTrackerApiAccess : HttpAccessBase
{
    public ProjectTrackerApiAccess(HttpService httpService)
        : base(httpService, "ProjectTrackerApi")
    {
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> BeginTrack(Guid worksheetId, Guid projectItemRateId)
    {
        return Post<object, WorksheetTimeTrackerViewModel>(BuildApi("BeginTrack", new
        {
            worksheetId,
            projectItemRateId
        }), null);
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel[]>> GetTracks()
    {
        return Get<WorksheetTimeTrackerViewModel[]>(BuildApi("GetTrackers"));
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> GetTrack(Guid worksheetId)
    {
        return Get<WorksheetTimeTrackerViewModel>(BuildApi("GetTracker", new
        {
            worksheetId
        }));
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> EndTrack(Guid worksheetId,
        DateTimeOffset? fromTime = null,
        DateTimeOffset? toTime = null,
        string comment = null)
    {
        return Post<object, WorksheetTimeTrackerViewModel>(BuildApi("EndTrack", new
        {
            worksheetId,
            fromTime,
            toTime,
            comment
        }), null);
    }

    public ValueTask<ApiResult<WorksheetTimeTrackerViewModel>> AbortTrack(Guid worksheetId)
    {
        return Post<object, WorksheetTimeTrackerViewModel>(BuildApi("AbortTrack", new
        {
            worksheetId
        }), null);
    }

    public ValueTask<ApiResult> UpdateTrack(Guid worksheetId, string comment)
    {
        return Post<object>(BuildApi("UpdateTrack", new
        {
            worksheetId,
            comment
        }), null);
    }
}

public class LoggerApiAccess : HttpAccessBase
{
    public LoggerApiAccess(HttpService httpService)
        : base(httpService, "LoggerApi")
    {
    }

    public ValueTask<ApiResult<AppLoggerLogViewModel[]>> GetEntries(string keyFilter, int page, int pageSize)
    {
        return Get<AppLoggerLogViewModel[]>(BuildApi("Get", new
        {
            key = keyFilter,
            page,
            pageSize
        }));
    }
}

public class NumberRangeApiAccess : HttpAccessBase
{
    public NumberRangeApiAccess(HttpService httpService)
        : base(httpService, "NumberRangeApi")
    {
    }

    public ValueTask<ApiResult<NumberRangeModel[]>> Get()
    {
        return Get<NumberRangeModel[]>(BuildApi("Get"));
    }

    public ValueTask<ApiResult<NumberRangeModel>> Get(Guid id)
    {
        return Get<NumberRangeModel>(BuildApi("GetSingle", new
        {
            id
        }));
    }

    public ValueTask<ApiResult<NumberRangeCodeMap[]>> GetCodeMappings()
    {
        return Get<NumberRangeCodeMap[]>(BuildApi("GetCodeMappings"));
    }

    public ValueTask<ApiResult<JsonSchema>> GetStructure(string code)
    {
        return Get<JsonSchema>(BuildApi("JsonSchema", new
        {
            code
        }));
    }

    public ValueTask<ApiResult<string>> Test(string code, string template, long counter)
    {
        return Get<string>(BuildApi("Test", new
        {
            code,
            template,
            counter
        }));
    }

    public ValueTask<ApiResult<NumberRangeModel>> Update(CreateNumberRangeModel model)
    {
        return Post<CreateNumberRangeModel, NumberRangeModel>(BuildApi("Update"), model);
    }
}