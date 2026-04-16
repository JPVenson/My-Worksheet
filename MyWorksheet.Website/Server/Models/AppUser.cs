using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Models;

public partial class AppUser
{
    public Guid AppUserId { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string ContactName { get; set; }

    public bool IsAktive { get; set; }

    public bool MailVerified { get; set; }

    public DateTime? MailVerifiedAt { get; set; }

    public byte MailVerifiedCounter { get; set; }

    public bool AllowUpdates { get; set; }

    public byte[] PasswordHash { get; set; }

    public bool NeedPasswordReset { get; set; }

    public bool AllowFeatureRedeeming { get; set; }

    public bool IsTestUser { get; set; }

    public Guid? IdAddress { get; set; }

    public Guid IdCountry { get; set; }

    public DateTime CreateDate { get; set; }

    public byte[] RowState { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = [];

    public virtual ICollection<AppNumberRange> AppNumberRanges { get; set; } = [];

    public virtual ICollection<AssosiationInvitation> AssosiationInvitations { get; set; } = [];

    public virtual Address IdAddressNavigation { get; set; }

    public virtual PromisedFeatureRegion IdCountryNavigation { get; set; }

    public virtual ICollection<LoginToken> LoginTokens { get; set; } = [];

    public virtual ICollection<MailAccountUserMap> MailAccountUserMaps { get; set; } = [];

    public virtual ICollection<MailAccount> MailAccounts { get; set; } = [];

    public virtual ICollection<MustachioTemplateFormatter> MustachioTemplateFormatters { get; set; } = [];

    public virtual ICollection<NengineRunningTask> NengineRunningTasks { get; set; } = [];

    public virtual ICollection<NengineTemplate> NengineTemplates { get; set; } = [];

    public virtual ICollection<OrganisationUserMap> OrganisationUserMaps { get; set; } = [];

    public virtual ICollection<OutgoingWebhookActionLog> OutgoingWebhookActionLogs { get; set; } = [];

    public virtual ICollection<OutgoingWebhook> OutgoingWebhooks { get; set; } = [];

    public virtual ICollection<OvertimeAccount> OvertimeAccounts { get; set; } = [];

    public virtual ICollection<PaymentInfoContent> PaymentInfoContents { get; set; } = [];

    public virtual ICollection<PaymentInfo> PaymentInfos { get; set; } = [];

    public virtual ICollection<PaymentOrder> PaymentOrders { get; set; } = [];

    public virtual ICollection<PriorityQueueItem> PriorityQueueItems { get; set; } = [];

    public virtual ICollection<ProjectAssosiatedUserMap> ProjectAssosiatedUserMaps { get; set; } = [];

    public virtual ICollection<ProjectBudget> ProjectBudgets { get; set; } = [];

    public virtual ICollection<ProjectItemRate> ProjectItemRates { get; set; } = [];

    public virtual ICollection<Project> Projects { get; set; } = [];

    public virtual ICollection<PromisedFeatureToAppUserMap> PromisedFeatureToAppUserMaps { get; set; } = [];

    public virtual ICollection<RemoteStorage> RemoteStorages { get; set; } = [];

    public virtual ICollection<SettingsGroup> SettingsGroups { get; set; } = [];

    public virtual ICollection<SettingsValue> SettingsValues { get; set; } = [];

    public virtual ICollection<StorageEntry> StorageEntries { get; set; } = [];

    public virtual ICollection<StorageProviderData> StorageProviderData { get; set; } = [];

    public virtual ICollection<StorageProvider> StorageProviders { get; set; } = [];

    public virtual ICollection<UserAction> UserActions { get; set; } = [];

    public virtual ICollection<UserActivity> UserActivities { get; set; } = [];

    public virtual ICollection<UserAssoisiatedUserMap> UserAssoisiatedUserMapIdChildUserNavigations { get; set; } = [];

    public virtual ICollection<UserAssoisiatedUserMap> UserAssoisiatedUserMapIdParentUserNavigations { get; set; } = [];

    public virtual ICollection<UserDocumentCache> UserDocumentCaches { get; set; } = [];

    public virtual ICollection<UserQuota> UserQuota { get; set; } = [];

    public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; } = [];

    public virtual ICollection<UserWorkload> UserWorkloads { get; set; } = [];

    public virtual ICollection<WorksheetAssert> WorksheetAsserts { get; set; } = [];

    public virtual ICollection<WorksheetGeoFence> WorksheetGeoFences { get; set; } = [];

    public virtual ICollection<WorksheetItemStatusLookup> WorksheetItemStatusLookups { get; set; } = [];

    public virtual ICollection<WorksheetItemStatus> WorksheetItemStatuses { get; set; } = [];

    public virtual ICollection<WorksheetItem> WorksheetItems { get; set; } = [];

    public virtual ICollection<WorksheetStatusHistory> WorksheetStatusHistories { get; set; } = [];

    public virtual ICollection<WorksheetTrack> WorksheetTracks { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowDataMap> WorksheetWorkflowDataMaps { get; set; } = [];

    public virtual ICollection<WorksheetWorkflowStorageMap> WorksheetWorkflowStorageMaps { get; set; } = [];

    public virtual ICollection<Worksheet> Worksheets { get; set; } = [];
}
