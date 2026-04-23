using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using AutoMapper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Provider;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Server.Services.ServerSettings;
using MyWorksheet.Website.Server.Services.Statistics;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Services.Workflow;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using MyWorksheet.Website.Server.Util.WorksheetItemUtil;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Storage;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Text;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow;
using Morestachio.Formatter.Framework;
using Newtonsoft.Json;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Mapping;

[SingletonService(typeof(IMapperService))]
public class MapperService : IMapperService
{
    /// <summary>Reconstructs a <see cref="DateTimeOffset"/> from a UTC-stored value and its offset in minutes.</summary>
    public static DateTimeOffset FromUtcWithOffset(DateTimeOffset utc, short offsetMinutes)
        => new DateTimeOffset(DateTime.SpecifyKind(utc.UtcDateTime.AddMinutes(offsetMinutes), DateTimeKind.Unspecified), TimeSpan.FromMinutes(offsetMinutes));

    /// <summary>Nullable variant of <see cref="FromUtcWithOffset"/>.</summary>
    public static DateTimeOffset? FromUtcWithOffset(DateTimeOffset? utc, short? offsetMinutes)
        => utc.HasValue ? FromUtcWithOffset(utc.Value, offsetMinutes ?? 0) : null;

    public MapperService(IUserQuotaService userQuotaService)
    {
        ViewModelMapper = new Mapper(new MapperConfiguration((c) =>
        {
            DefaultMap(c, userQuotaService);
            QuotaMap(c, userQuotaService);
        }));

        ReportingMapper = new Mapper(new MapperConfiguration((c) =>
        {
            DefaultMap(c, userQuotaService);
            MorstachioMap(c);
        }));

        SchedulerMapper = new Mapper(new MapperConfiguration((c) =>
        {
            DefaultMap(c, userQuotaService);
            PriorityQueueMap(c);
        }));
    }

    public IMapper ViewModelMapper { get; set; }
    public IMapper ReportingMapper { get; set; }
    public IMapper SchedulerMapper { get; set; }


    public static void MorstachioMap(IMapperConfigurationExpression d)
    {
        d.CreateMap<MorestachioFormatterModel, MustachioFormatterViewModel>()
            .ForMember(e => e.InputType, f => f.MapFrom(e => (JsonHelper.TranslateAnyCsToTs(e.InputType) ?? e.InputType.Name)))
            .ForMember(e => e.OutputType, f => f.MapFrom(e => (JsonHelper.TranslateAnyCsToTs(e.OutputType) ?? e.OutputType.Name)));

        d.CreateMap<InputDescription, InputDescriptionViewModel>()
            .ForMember(e => e.OutputType, f => f.MapFrom(e => e.OutputType == null ? null : (JsonHelper.TranslateAnyCsToTs(e.OutputType) ?? e.OutputType.Name)));
    }

    public static void QuotaMap(IMapperConfigurationExpression d, IUserQuotaService quotaService)
    {

        d.CreateMap<UserQuota, UserQuotaViewModel>()
            .ForMember(e => e.Value, e => e.MapFrom(f => f.QuotaValue))
            .ForMember(e => e.Type, e => e.MapFrom(f => f.QuotaType))
            .ForMember(e => e.MaxValue, e => e.MapFrom(f => f.QuotaMax))
            .ForMember(e => e.Name, e => e.MapFrom(f => quotaService.UserQuotaParts[(Quotas)f.QuotaType].Name))
            ;
    }

    public static void PriorityQueueMap(IMapperConfigurationExpression d)
    {
        var xmlSerializer = new DataContractSerializer(typeof(SerializableObjectDictionary<string, object>));

        d.CreateMap<PriorityQueueItem, PriorityQueueElement>()
            .ForMember(e => e.Arguments, f => f.MapFrom((from, to) =>
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(@from.DataArguments)))
                {
                    return xmlSerializer.ReadObject(ms) as IDictionary<string, object>;
                }
            }))
            .ForMember(e => e.Version, e => e.MapFrom(f => f.Version == null ? null : Version.Parse(f.Version)))
            .ForMember(e => e.UserId, e => e.MapFrom(f => f.IdCreator))
            .ForMember(e => e.Level, e => e.MapFrom(f => f.Level))
            .ReverseMap()
            .ForMember(e => e.DataArguments, e => e.MapFrom((from, to) =>
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        xmlSerializer.WriteObject(ms, from.Arguments);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }))
            .ForMember(e => e.IdCreator, e => e.MapFrom(f => f.UserId));
    }

    public static void DefaultMap(IMapperConfigurationExpression d, IUserQuotaService quotaService)
    {
        var pocos = typeof(Project).Assembly.GetTypes().Where(f => f.Namespace == typeof(Project).Namespace);
        var viewModels = typeof(ProjectItemRateViewModel).Assembly.GetTypes();

        foreach (var poco in pocos)
        {
            var name = poco.Name;
            // AutoMapper member-flattening would try to map DateTimeOffset.Offset (TimeSpan)
            // into *Offset (short/short?) columns, for which no TimeSpan→short map exists.
            // Collect those properties upfront so we can Ignore() them on every ViewModel→Model map.
            var offsetProps = poco.GetProperties()
                .Where(p => p.Name.EndsWith("Offset") &&
                            (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)))
                .Select(p => p.Name)
                .ToList();

            var matches = viewModels
                .Where(e => e.Name.Contains(poco.Name)
                            || e.Name.Replace('s', 'z').Contains(poco.Name)
                            || e.Name.Contains(poco.Name.Replace('s', 'z')));
            foreach (var match in matches)
            {
                var mapExpr = d.CreateMap(match, poco);
                foreach (var prop in offsetProps)
                    mapExpr = mapExpr.ForMember(prop, opt => opt.Ignore());
                mapExpr.ReverseMap();
            }
        }

        d.CreateMap(typeof(PageResultSet<>), typeof(IDataPager<>))
            .ReverseMap()
            .ForMember(nameof(IDataPager<object>.CurrentPageItems), f => f.MapFrom(nameof(PageResultSet<object>.CurrentPageItems)))
            .ForMember(nameof(IDataPager<object>.CurrentPage), f => f.MapFrom(nameof(PageResultSet<object>.CurrentPage)))
            .ForMember(nameof(IDataPager<object>.MaxPage), f => f.MapFrom(nameof(PageResultSet<object>.MaxPage)))
            .ForMember(nameof(IDataPager<object>.PageSize), f => f.MapFrom(nameof(PageResultSet<object>.PageSize)))
            .ForMember(nameof(IDataPager<object>.TotalItemCount), f => f.MapFrom(nameof(PageResultSet<object>.TotalItemCount)))
            ;

        d.CreateMap<IWorksheetStatusType, WorksheetWorkflowStepViewModel>()
            .ForMember(e => e.Display, e => e.MapFrom(f => f.DisplayKey))
            .ForMember(e => e.Value, e => e.MapFrom(f => f.ConvertToGuid()))
            .ReverseMap();

        d.CreateMap<TextResourceViewModel, TextResourceEntity>()
            .ReverseMap()
            .ForMember(e => e.Lang, e => e.MapFrom(f => f.Lang.Name));

        d.CreateMap<ProjectsInOrganisation, GetProjectModel>()
            .ReverseMap();
        d.CreateMap<Project, ProjectsInOrganisation>()
            .ReverseMap();
        d.CreateMap<OrganisationUserMap, OrganizationMapViewModel>()
            .ReverseMap();
        d.CreateMap<StatisticsDataPresenterViewModel, StatisticsDataPresenter>()
            .ReverseMap();

        d.CreateMap<StorageProviderData, KeyValuePair<string, object>>()
            .ForMember(e => e.Key, e => e.MapFrom(f => f.Key))
            .ForMember(e => e.Value, e => e.MapFrom(f => f.Value))
            .ReverseMap()
            .ForMember(e => e.Key, e => e.MapFrom(f => f.Key))
            .ForMember(e => e.Value, e => e.MapFrom(f => f.Value));

        d.CreateMap<WorksheetWorkflowStatusLookupViewModel, WorksheetStatusLookup>()
            .ReverseMap();
        d.CreateMap<WorksheetStatusHistory, WorksheetStatusModel>()
            .ReverseMap();
        d.CreateMap<StorageProviderInfo, StorageProviderMap>()
            .ReverseMap();
        d.CreateMap<BlobManagerSetOperationResult, StandardOperationResultBase<StorageEntryViewModel>>()
            .ReverseMap();

        d.CreateMap<AppUser, AccountApiUserGetInfo>()
            .ForMember(e => e.UserID, f => f.MapFrom(e => e.AppUserId))
            .ReverseMap()
            .ForMember(e => e.AppUserId, f => f.MapFrom(e => e.UserID));
        d.CreateMap<AppUser, AccountApiAdminGet>()
            .ForMember(e => e.UserID, f => f.MapFrom(e => e.AppUserId))
            .ReverseMap()
            .ForMember(e => e.AppUserId, f => f.MapFrom(e => e.UserID));

        d.CreateMap<UserAssoisiatedUserMap, UserToUserAssosiationViewModel>()
            .ForMember(e => e.Child, e => e.MapFrom(f => f.IdChildUserNavigation))
            .ForMember(e => e.Role, e => e.MapFrom(f => f.IdUserRelationNavigation))
            .ForMember(e => e.UserAssoisiatedUserMapId, e => e.MapFrom(f => f.UserAssoisiatedUserMapId))
            .ForMember(e => e.Parent, e => e.MapFrom(f => f.IdParentUserNavigation));

        d.CreateMap<NengineRunningTask, NEngineHistoryGet>()
            .ForMember(e => e.ArgumentsRepresentation,
                e => e.MapFrom(f => JsonConvert.DeserializeObject<ScheduleReportModel>(f.ArgumentsRepresentation ?? "")));

        d.CreateMap<NengineTemplate, NEngineApiElement>().ReverseMap();
        d.CreateMap<NengineTemplate, NEngineTemplateLookup>().ReverseMap();

        d.CreateMap<PostContactApiModel, ContactEntry>()
            .ForMember(f => f.Email, f => f.MapFrom(e => e.EmailAddress))
            .ReverseMap();

        d.CreateMap<UserToUserRoleViewModel, Role>()
            .ForMember(e => e.RoleId, e => e.MapFrom(f => f.Id))
            .ForMember(e => e.RoleName, e => e.MapFrom(f => f.Name))
            .ReverseMap()
            .ForMember(e => e.Id, e => e.MapFrom(f => f.RoleId))
            .ForMember(e => e.Name, e => e.MapFrom(f => f.RoleName));

        d.CreateMap<ContentCreateModel, Cmscontent>()
            .ForMember(f => f.ContentId, f => f.MapFrom(g => g.Content_ID))
            .ReverseMap()
            .ForMember(f => f.Content_ID, f => f.MapFrom(g => g.ContentId));

        d.CreateMap<AddressModel, Address>()
            .ForMember(f => f.EmailAddress, f => f.MapFrom(g => g.Email))
            .ReverseMap()
            .ForMember(f => f.Email, f => f.MapFrom(g => g.EmailAddress));

        d.CreateMap<UpdateAddressModel, Address>()
            .ForMember(f => f.EmailAddress, f => f.MapFrom(g => g.Email))
            .ReverseMap()
            .ForMember(f => f.Email, f => f.MapFrom(g => g.EmailAddress));

        d.CreateMap<UpdateOrganizationMapViewModel, OrganisationUserMap>()
            .ReverseMap();

        d.CreateMap<ContentGetModel, Cmscontent>()
            .ForMember(f => f.ContentId, f => f.MapFrom(g => g.Content_ID))
            .ForMember(f => f.ContentTemplate, f => f.MapFrom(g => g.Content_Template))
            .ReverseMap()
            .ForMember(f => f.Content_ID, f => f.MapFrom(g => g.ContentId))
            .ForMember(f => f.Content_Template, f => f.MapFrom(g => g.ContentTemplate));

        d.CreateMap<ProjectOverviewReporting, OverviewModel>()
            .ForMember(f => f.Hours, f => f.MapFrom(e => e.WorkedHours))
            .ForMember(f => f.Earned, f => f.MapFrom(e => e.Earned))
            .ReverseMap();

        d.CreateMap<SubmittedProject, SubmittedProjectsModel>()
            .ReverseMap();

        d.CreateMap<WorksheetItem, WorksheetItemModel>()
            .ForMember(e => e.DateOfAction, f => f.MapFrom(e =>
                FromUtcWithOffset(e.DateOfAction, e.DateOfActionOffset)))
            .ReverseMap()
            .ForMember(e => e.DateOfAction, f => f.MapFrom(e => e.DateOfAction.ToUniversalTime()))
            .ForMember(e => e.DateOfActionOffset, f => f.MapFrom(e => (short)e.DateOfAction.Offset.TotalMinutes))
            .ForMember(f => f.IdWorksheet, f => f.MapFrom(e => e.IdWorksheet))
            .ForMember(f => f.WorksheetItemId, f => f.MapFrom(e => e.WorksheetItemId));

        d.CreateMap<Project, GetProjectModel>()
            .ReverseMap();

        d.CreateMap<UserAction, UserActionAdminGet>()
            .ForMember(f => f.ActionIP,
                f => f.MapFrom(e => e.ActionIp.Select(g => g.ToString("X2")).Aggregate((a, s) => a + "." + s)));

        d.CreateMap<AccountApiUserPost, AppUser>()
            .ReverseMap()
            .ForMember(e => e.ContactName, e => e.MapFrom(f => string.IsNullOrWhiteSpace(f.ContactName) ? f.Username : f.ContactName));
        d.CreateMap<UserAssosiationModel, UserAssoisiatedUserMap>().ReverseMap();

        d.CreateMap<AccountApiUserGetInfo, AppUser>()
            .ForMember(f => f.IsAktive, f => f.MapFrom(e => e.IsActive))
            .ForMember(s => s.AppUserId, f => f.MapFrom(s => s.UserID))
            .ReverseMap()
            .ForMember(f => f.IsActive, f => f.MapFrom(e => e.IsAktive))
            .ForMember(s => s.UserID, f => f.MapFrom(s => s.AppUserId));
        d.CreateMap<AccountApiGet, AppUser>()
            .ReverseMap();

        d.CreateMap<AccountApiUserGetInfo, UserOrganisationMapping>()
            .ForMember(f => f.IsAktive, f => f.MapFrom(e => e.IsActive))
            .ForMember(s => s.AppUserId, f => f.MapFrom(s => s.UserID))
            .ReverseMap()
            .ForMember(f => f.IsActive, f => f.MapFrom(e => e.IsAktive))
            .ForMember(s => s.UserID, f => f.MapFrom(s => s.AppUserId));

        d.CreateMap<AccountApiAdminGet, AppUser>()
            .ForMember(f => f.IsAktive, f => f.MapFrom(e => e.IsActive))
            .ForMember(s => s.AppUserId, f => f.MapFrom(s => s.UserID))
            .ReverseMap()
            .ForMember(f => f.IsActive, f => f.MapFrom(e => e.IsAktive))
            .ForMember(s => s.UserID, f => f.MapFrom(s => s.AppUserId));
        d.CreateMap<AccountApiAdminPost, AppUser>()
            .ForMember(f => f.IsAktive, f => f.MapFrom(e => e.IsActive))
            .ReverseMap()
            .ForMember(f => f.IsActive, f => f.MapFrom(e => e.IsAktive));

        d.CreateMap<UserDocumentCache, DocumentsApiPost>()
            .ForMember(f => f.UserID, f => f.MapFrom(e => e.IdUser))
            .ForMember(f => f.HostedOn, f => f.MapFrom(e => e.HostenOn))
            .ReverseMap()
            .ForMember(f => f.HostenOn, f => f.MapFrom(e => e.HostedOn))
            .ForMember(f => f.IdUser, f => f.MapFrom(e => e.UserID));

        d.CreateMap<GetFeature, PromisedFeatureContent>()
            .ForMember(f => f.PromisedFeatureContentId, f => f.MapFrom(e => e.PromisedFeatureContentId))
            .ReverseMap()
            .ForMember(f => f.PromisedFeatureContentId, f => f.MapFrom(e => e.PromisedFeatureContentId));

        d.CreateMap<GetFeatureMeta, PromisedFeature>()
            .ForMember(f => f.PromisedFeatureId, f => f.MapFrom(e => e.PromisedFeatureId))
            .ReverseMap()
            .ForMember(f => f.PromisedFeatureId, f => f.MapFrom(e => e.PromisedFeatureId));

        d.CreateMap<RegionInfo, CurrencyLookup>()
            .ForMember(f => f.IsoCode, f => f.MapFrom(e => e.ISOCurrencySymbol))
            .ForMember(f => f.Sign, f => f.MapFrom(e => e.CurrencySymbol));

        d.CreateMap<NumberRangeModel, AppNumberRange>()
            .ReverseMap();
        d.CreateMap<CreateNumberRangeModel, AppNumberRange>()
            .ReverseMap();

        d.CreateMap<ConfigurationModelGetFlat, KeyValuePair<string, object>>()
            .ForMember(f => f.Key, f => f.MapFrom(e => e.Path))
            .ForMember(f => f.Value, f => f.MapFrom(e => e.Value))
            .ReverseMap()
            .ForMember(f => f.Path, f => f.MapFrom(e => e.Key))
            .ForMember(f => f.Value, f => f.MapFrom(e => e.Value));

        d.CreateMap<ServerSettingsModel, ConfigurationModelGet>()
            .ForMember(f => f.Childs, f => f.MapFrom(e => e.Childs.Values.Cast<ServerSettingsModel>()))
            .ForMember(f => f.Settings, f => f.MapFrom(e => e.SettingsObjects.Values.Cast<SettingsObject>()))
            .ForMember(e => e.Path, e => e.MapFrom(f => f.Path))
            .ReverseMap()
            .ForMember(f => f.Childs, f => f.MapFrom(e => e.Childs.ToDictionary(w => w.Name, w => w)))
            .ForMember(f => f.SettingsObjects, f => f.MapFrom(e => e.Settings.ToDictionary(w => w.Name, w => w)))
            .ForMember(e => e.Path, e => e.MapFrom(f => f.Path));
        d.CreateMap<ISettingsObject, ConfigurationSettingModelGet>()
            .ForMember(e => e.FullOriginalName, e => e.MapFrom(f => f.Path))
            .ReverseMap()
            .ForMember(e => e.Path, e => e.MapFrom(f => f.FullOriginalName));

        d.CreateMap<ITask, TaskInfo>().ForMember(f => f.Name, f => f.MapFrom(e => e.NamedTask));
        d.CreateMap<ITaskRunner, SchedulerInfo>()
            .ForMember(f => f.NextRun, f => f.MapFrom(e => DateTime.UtcNow + (e.DetermininateNextRun() ?? TimeSpan.FromDays(1000))))
            .ForMember(f => f.LastRun, f => f.MapFrom(e => DateTime.UtcNow - (e.DetermininateNextRun() ?? TimeSpan.FromDays(1000))))
            .ForMember(f => f.TaskInfo, f => f.MapFrom(e => e.Task))
            .ForMember(f => f.Schedule, f => f.MapFrom(e => e.IntervalText()));

        d.CreateMap<DashboardWorksheet, DashboardWorksheetModel>()
            .ForMember(f => f.StartTime, e => e.MapFrom(f => f.StartTime.Date))
            .ForMember(f => f.EndTime, e => e.MapFrom(f => f.EndTime))
            .ReverseMap();
        //.ForMember(f => f.IdProject, f => f.ResolveUsing(g => idMapper.ProjectId.GetFakeId(g.IdProject, g.IdCreator)))
        //.ForMember(f => f.WorksheetId, f => f.ResolveUsing(g => idMapper.WorksheetId.GetFakeId(g.WorksheetId, g.IdCreator)));


        d.CreateMap<DataExport, DataExportModel>();
        d.CreateMap<DataDimension, DataDimensionModel>()
            .ForMember(e => e.DisplayAs, e => e.MapFrom(f => f.DisplayAs.TypeName));
        d.CreateMap<LabelOversight, LabelOversightModel>();
        d.CreateMap<DataSample, DataSampleModel>();
        d.CreateMap<DataLine, DataLineModel>();
        d.CreateMap<DataPoint, DataPointModel>();

        d.CreateMap<WorksheetTimeTrackerViewModel, WorksheetTrack>()
            .ForMember(e => e.DateStarted, f => f.MapFrom(e => e.StartTime.ToUniversalTime()))
            .ForMember(e => e.DateStartedOffset, f => f.MapFrom(e => (short)e.StartTime.Offset.TotalMinutes))
            .ReverseMap()
            .ForMember(e => e.IdProject, f => f.MapFrom(e => e.IdWorksheetNavigation.IdProject))
            .ForMember(e => e.StartTime, f => f.MapFrom(e =>
                FromUtcWithOffset(e.DateStarted, e.DateStartedOffset)));

        d.CreateMap<Worksheet, WorksheetModel>()
            .ForMember(e => e.StartTime, f => f.MapFrom(e =>
                FromUtcWithOffset(e.StartTime, e.StartTimeOffset)))
            .ForMember(e => e.EndTime, f => f.MapFrom(e =>
                FromUtcWithOffset(e.EndTime, e.EndTimeOffset)))
            .ForMember(e => e.InvoiceDueDate, f => f.MapFrom(e =>
                FromUtcWithOffset(e.InvoiceDueDate, e.InvoiceDueDateOffset)))
            .ReverseMap()
            .ForMember(e => e.StartTime, f => f.MapFrom(e => e.StartTime.ToUniversalTime()))
            .ForMember(e => e.StartTimeOffset, f => f.MapFrom(e => (short)e.StartTime.Offset.TotalMinutes))
            .ForMember(e => e.EndTime, f => f.MapFrom(e => e.EndTime.HasValue ? e.EndTime.Value.ToUniversalTime() : (DateTimeOffset?)null))
            .ForMember(e => e.EndTimeOffset, f => f.MapFrom(e => e.EndTime.HasValue ? (short?)((short)e.EndTime.Value.Offset.TotalMinutes) : null))
            .ForMember(e => e.InvoiceDueDate, f => f.MapFrom(e => e.InvoiceDueDate.HasValue ? e.InvoiceDueDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
            .ForMember(e => e.InvoiceDueDateOffset, f => f.MapFrom(e => e.InvoiceDueDate.HasValue ? (short?)((short)e.InvoiceDueDate.Value.Offset.TotalMinutes) : null));

        d.CreateMap<ProjectBudget, ProjectBudgetViewModel>()
            .ForMember(e => e.Deadline, f => f.MapFrom(e =>
                FromUtcWithOffset(e.Deadline, e.DeadlineOffset)))
            .ForMember(e => e.ValidFrom, f => f.MapFrom(e =>
                FromUtcWithOffset(e.ValidFrom, e.ValidFromOffset)))
            .ReverseMap()
            .ForMember(e => e.Deadline, f => f.MapFrom(e => e.Deadline.HasValue ? e.Deadline.Value.ToUniversalTime() : (DateTimeOffset?)null))
            .ForMember(e => e.DeadlineOffset, f => f.MapFrom(e => e.Deadline.HasValue ? (short?)((short)e.Deadline.Value.Offset.TotalMinutes) : null))
            .ForMember(e => e.ValidFrom, f => f.MapFrom(e => e.ValidFrom.HasValue ? e.ValidFrom.Value.ToUniversalTime() : (DateTimeOffset?)null))
            .ForMember(e => e.ValidFromOffset, f => f.MapFrom(e => e.ValidFrom.HasValue ? (short?)((short)e.ValidFrom.Value.Offset.TotalMinutes) : null));

        d.CreateMap<PriorityQueueItem, PriorityQueueItemViewModel>()
            .ForMember(e => e.DateOfCreation, f => f.MapFrom(e =>
                FromUtcWithOffset(e.DateOfCreation, e.DateOfCreationOffset)))
            .ForMember(e => e.DateOfDone, f => f.MapFrom(e =>
                FromUtcWithOffset(e.DateOfDone, e.DateOfDoneOffset)))
            .ReverseMap()
            .ForMember(e => e.DateOfCreation, f => f.MapFrom(e => e.DateOfCreation.ToUniversalTime()))
            .ForMember(e => e.DateOfCreationOffset, f => f.MapFrom(e => (short)e.DateOfCreation.Offset.TotalMinutes))
            .ForMember(e => e.DateOfDone, f => f.MapFrom(e => e.DateOfDone.HasValue ? e.DateOfDone.Value.ToUniversalTime() : (DateTimeOffset?)null))
            .ForMember(e => e.DateOfDoneOffset, f => f.MapFrom(e => e.DateOfDone.HasValue ? (short?)((short)e.DateOfDone.Value.Offset.TotalMinutes) : null));

        d.CreateMap<ReportingDataSourceViewModel, IReportingDataSource>()
            .ReverseMap();

        d.CreateMap<MergeReportViewModel, MergeReport>()
            .ReverseMap();

        d.CreateMap<Project, ProjectSpecReportModel>()
            .ReverseMap();

        d.CreateMap<ProjectReporting, ProjectReportingViewModel>()
            .ReverseMap();

        d.CreateMap<WorksheetReporting, WorksheetReportingViewModel>()
            .ReverseMap();

        d.CreateMap<NEngineTemplateCreate, NengineTemplate>()
            .ReverseMap();

        d.CreateMap<NEngineTemplateUpdate, NengineTemplate>()
            .ReverseMap();

        d.CreateMap<object, DashboardWorksheetModel>();
    }
}

public interface IMapperService
{
    IMapper ViewModelMapper { get; }
    IMapper ReportingMapper { get; }
    IMapper SchedulerMapper { get; }
}