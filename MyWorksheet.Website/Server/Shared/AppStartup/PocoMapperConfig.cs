using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using AutoMapper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.ServerSettings;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Assosiation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Roles;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Administration.ExecuteLater;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Statistics;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics;
using Morestachio.Formatter.Framework;
using Newtonsoft.Json;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Helper;

namespace MyWorksheet.AppStartup;

public static class PocoMapperConfig
{
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

            var matches = viewModels.Where(e => e.Name.Contains(poco.Name));
            foreach (var match in matches)
            {
                d.CreateMap(match, poco)
                    .ReverseMap();
            }
        }

        MorstachioMap(d);
        QuotaMap(d, quotaService);
        PriorityQueueMap(d);
        d.CreateMap(typeof(PageResultSet<>), typeof(IDataPager<>));


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

        d.CreateMap<ContentGetModel, Cmscontent>()
            .ForMember(f => f.ContentId, f => f.MapFrom(g => g.Content_ID))
            .ForMember(f => f.ContentTemplate, f => f.MapFrom(g => g.Content_Template))
            .ReverseMap()
            .ForMember(f => f.Content_ID, f => f.MapFrom(g => g.ContentId))
            .ForMember(f => f.Content_Template, f => f.MapFrom(g => g.ContentTemplate));

        d.CreateMap<ProjectReporting, ProjectOverviewReporting>()
            //.ForMember(f => f.ProjectId, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.ProjectId, f.IdCreator)))
            .ReverseMap();

        d.CreateMap<PerDayReporting, PerDayReporting>()
            //.ForMember(f => f.IdWorksheet, e => e.ResolveUsing(f => idMapper.WorksheetId.GetFakeId(f.IdWorksheet, f.IdCreator.Value)))
            //.ForMember(f => f.ProjectId, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.ProjectId.Value, f.IdCreator.Value)))
            .ReverseMap();

        d.CreateMap<WorksheetItemReporting, WorksheetItemReporting>()
            //.ForMember(f => f.WorksheetItemId, e => e.ResolveUsing(f => idMapper.WorksheetItemId.GetFakeId(f.WorksheetItemId, f.IdCreator.Value)))
            //.ForMember(f => f.IdWorksheet, e => e.ResolveUsing(f => idMapper.WorksheetId.GetFakeId(f.IdWorksheet, f.IdCreator.Value)))
            //.ForMember(f => f.ProjectId, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.ProjectId.Value, f.IdCreator.Value)))
            .ReverseMap();

        d.CreateMap<WorksheetReporting, WorksheetReporting>()
            //.ForMember(f => f.WorksheetId, e => e.ResolveUsing(f => idMapper.WorksheetId.GetFakeId(f.WorksheetId, f.IdCreator)))
            //.ForMember(f => f.IdProject, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.IdProject, f.IdCreator)))
            .ReverseMap();

        d.CreateMap<ProjectOverviewReporting, OverviewModel>()
            //.ForMember(f => f.ProjectId, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.ProjectID, f.IdCreator)))
            .ForMember(f => f.Hours, f => f.MapFrom(e => e.WorkedHours))
            .ForMember(f => f.Earned, f => f.MapFrom(e => e.Earned))
            .ReverseMap();

        d.CreateMap<SubmittedProject, SubmittedProjectsModel>()
            //.ForMember(f => f.ProjectId, e => e.ResolveUsing(f => idMapper.ProjectId.GetFakeId(f.ProjectId, f.IdCreator.Value)))
            .ReverseMap();

        d.CreateMap<WorksheetItem, WorksheetItemModel>()
            //.ForMember(f => f.WorksheetItem_Id, f => f.MapFrom(e => e.VirtualId))
            //.ForMember(f => f.Id_Worksheet, e => e.ResolveUsing(f => idMapper.WorksheetId.GetFakeId(f.IdWorksheet, f.IdCreator)))
            .ForMember(e => e.DateOfAction, f => f.MapFrom(e => e.DateOfAction.Date))
            .ReverseMap()
            .ForMember(e => e.DateOfAction, f => f.MapFrom(e => e.DateOfAction.Date))
            .ForMember(f => f.IdWorksheet, f => f.MapFrom(e => e.IdWorksheet))
            .ForMember(f => f.WorksheetItemId, f => f.MapFrom(e => e.WorksheetItemId));

        d.CreateMap<Project, GetProjectModel>()
            //.ForMember(f => f.Project_Id, f => f.MapFrom(e => e.VirtualId))
            .ReverseMap()
            .ForMember(f => f.ProjectId, f => f.MapFrom(e => e.ProjectId));

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
            .ForMember(f => f.TaskInfo, f => f.MapFrom(e => e.Task));

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
    }
}