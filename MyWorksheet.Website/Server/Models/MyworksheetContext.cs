using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyWorksheet.Website.Server.Models;

/// <summary>
/// The design time factory for <see cref="JellyfinDbContext"/>.
/// This is only used for the creation of migrations and not during runtime.
/// </summary>
internal sealed class DesignTimeDbFactory : IDesignTimeDbContextFactory<MyworksheetContext>
{
    public MyworksheetContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyworksheetContext>();
        optionsBuilder.UseNpgsql(f => f.MigrationsAssembly(GetType().Assembly));

        return new MyworksheetContext(optionsBuilder.Options);
    }
}

public partial class MyworksheetContext : DbContext
{
    public MyworksheetContext()
    {
    }

    public MyworksheetContext(DbContextOptions<MyworksheetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AppLoggerLog> AppLoggerLogs { get; set; }

    // public virtual DbSet<AppMigration> AppMigrations { get; set; }

    public virtual DbSet<AppNumberRange> AppNumberRanges { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AssosiationInvitation> AssosiationInvitations { get; set; }

    public virtual DbSet<BillingFrameLookup> BillingFrameLookups { get; set; }

    public virtual DbSet<ClientStructure> ClientStructures { get; set; }

    public virtual DbSet<ClientStructureWithRight> ClientStructureWithRights { get; set; }

    public virtual DbSet<ClientSturctureRight> ClientSturctureRights { get; set; }

    public virtual DbSet<Cmscontent> Cmscontents { get; set; }

    public virtual DbSet<ContactEntry> ContactEntries { get; set; }

    public virtual DbSet<DashboardPlugin> DashboardPlugins { get; set; }

    public virtual DbSet<HostedStorageBlob> HostedStorageBlobs { get; set; }

    public virtual DbSet<LicenceEntry> LicenceEntries { get; set; }

    public virtual DbSet<LicenceGroup> LicenceGroups { get; set; }

    public virtual DbSet<LoginToken> LoginTokens { get; set; }

    public virtual DbSet<MailAccount> MailAccounts { get; set; }

    public virtual DbSet<MailAccountUserMap> MailAccountUserMaps { get; set; }

    public virtual DbSet<MailBlacklist> MailBlacklists { get; set; }

    public virtual DbSet<MailSend> MailSends { get; set; }

    public virtual DbSet<Maintainace> Maintainaces { get; set; }

    public virtual DbSet<MustachioTemplateFormatter> MustachioTemplateFormatters { get; set; }

    public virtual DbSet<NengineRunningTask> NengineRunningTasks { get; set; }

    public virtual DbSet<NengineTemplate> NengineTemplates { get; set; }

    public virtual DbSet<Organisation> Organisations { get; set; }

    public virtual DbSet<OrganisationRoleLookup> OrganisationRoleLookups { get; set; }

    public virtual DbSet<OrganisationUserMap> OrganisationUserMaps { get; set; }

    public virtual DbSet<OrganisationUserMapping> OrganisationUserMappings { get; set; }

    public virtual DbSet<OrganisationWorksheet> OrganisationWorksheets { get; set; }

    public virtual DbSet<OutgoingWebhook> OutgoingWebhooks { get; set; }

    public virtual DbSet<OutgoingWebhookActionLog> OutgoingWebhookActionLogs { get; set; }

    public virtual DbSet<OutgoingWebhookCase> OutgoingWebhookCases { get; set; }

    public virtual DbSet<OvertimeAccount> OvertimeAccounts { get; set; }

    public virtual DbSet<OvertimeTransaction> OvertimeTransactions { get; set; }

    public virtual DbSet<PaymentInfo> PaymentInfos { get; set; }

    public virtual DbSet<PaymentInfoContent> PaymentInfoContents { get; set; }

    public virtual DbSet<OrdersAggregated> OrdersAggregated { get; set; }

    public virtual DbSet<PaymentOrder> PaymentOrders { get; set; }

    public virtual DbSet<PaymentProvider> PaymentProviders { get; set; }

    public virtual DbSet<PaymentProviderForRegionMap> PaymentProviderForRegionMaps { get; set; }

    public virtual DbSet<PerDayReporting> PerDayReportings { get; set; }

    public virtual DbSet<PriorityQueueItem> PriorityQueueItems { get; set; }

    public virtual DbSet<Processor> Processors { get; set; }

    public virtual DbSet<ProcessorCapability> ProcessorCapabilities { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectAddressMap> ProjectAddressMaps { get; set; }

    public virtual DbSet<ProjectAddressMapLookup> ProjectAddressMapLookups { get; set; }

    public virtual DbSet<ProjectAssosiatedUserMap> ProjectAssosiatedUserMaps { get; set; }

    public virtual DbSet<ProjectBudget> ProjectBudgets { get; set; }

    public virtual DbSet<ProjectChargeRate> ProjectChargeRates { get; set; }

    public virtual DbSet<ProjectItemRate> ProjectItemRates { get; set; }

    public virtual DbSet<ProjectOverviewReporting> ProjectOverviewReportings { get; set; }

    public virtual DbSet<ProjectReporting> ProjectReportings { get; set; }

    public virtual DbSet<ProjectShareKey> ProjectShareKeys { get; set; }

    public virtual DbSet<ProjectsInOrganisation> ProjectsInOrganisations { get; set; }

    public virtual DbSet<PromisedFeature> PromisedFeatures { get; set; }

    public virtual DbSet<PromisedFeatureContent> PromisedFeatureContents { get; set; }

    public virtual DbSet<PromisedFeatureRegion> PromisedFeatureRegions { get; set; }

    public virtual DbSet<PromisedFeatureToAppUserMap> PromisedFeatureToAppUserMaps { get; set; }

    public virtual DbSet<Realm> Realms { get; set; }

    public virtual DbSet<RemoteStorage> RemoteStorages { get; set; }

    public virtual DbSet<ReportingDataSource> ReportingDataSources { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SettingsGroup> SettingsGroups { get; set; }

    public virtual DbSet<SettingsValue> SettingsValues { get; set; }

    public virtual DbSet<StorageEntry> StorageEntries { get; set; }

    public virtual DbSet<StorageProvider> StorageProviders { get; set; }

    public virtual DbSet<StorageProviderData> StorageProviderData { get; set; }

    public virtual DbSet<StorageType> StorageTypes { get; set; }

    public virtual DbSet<SubmittedProject> SubmittedProjects { get; set; }

    public virtual DbSet<SubmittedWorksheetsReporting> SubmittedWorksheetsReportings { get; set; }

    public virtual DbSet<TextResource> TextResources { get; set; }

    public virtual DbSet<UserAction> UserActions { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    public virtual DbSet<UserAssoisiatedUserMap> UserAssoisiatedUserMaps { get; set; }

    public virtual DbSet<UserAssosiatedRoleLookup> UserAssosiatedRoleLookups { get; set; }

    public virtual DbSet<UserDocumentCache> UserDocumentCaches { get; set; }

    public virtual DbSet<UserOrganisationMapping> UserOrganisationMappings { get; set; }

    public virtual DbSet<UserQuota> UserQuota { get; set; }

    public virtual DbSet<UserRoleMap> UserRoleMaps { get; set; }

    public virtual DbSet<UserWorkload> UserWorkloads { get; set; }

    public virtual DbSet<Worksheet> Worksheets { get; set; }

    public virtual DbSet<WorksheetAssert> WorksheetAsserts { get; set; }

    public virtual DbSet<WorksheetAssertsFilesMap> WorksheetAssertsFilesMaps { get; set; }

    public virtual DbSet<WorksheetComment> WorksheetComments { get; set; }

    public virtual DbSet<WorksheetGeoFence> WorksheetGeoFences { get; set; }

    public virtual DbSet<WorksheetGeoFenceLocation> WorksheetGeoFenceLocations { get; set; }

    public virtual DbSet<WorksheetGeoFenceWiFi> WorksheetGeoFenceWiFis { get; set; }

    public virtual DbSet<WorksheetItem> WorksheetItems { get; set; }

    public virtual DbSet<WorksheetItemReporting> WorksheetItemReportings { get; set; }

    public virtual DbSet<WorksheetItemStatus> WorksheetItemStatuses { get; set; }

    public virtual DbSet<WorksheetItemStatusLookup> WorksheetItemStatusLookups { get; set; }

    public virtual DbSet<WorksheetItemsStatusReporting> WorksheetItemsStatusReportings { get; set; }

    public virtual DbSet<WorksheetReporting> WorksheetReportings { get; set; }

    public virtual DbSet<WorksheetStatusHistory> WorksheetStatusHistories { get; set; }

    public virtual DbSet<WorksheetStatusLookup> WorksheetStatusLookups { get; set; }

    public virtual DbSet<WorksheetStatusLookupMap> WorksheetStatusLookupMaps { get; set; }

    public virtual DbSet<WorksheetTrack> WorksheetTracks { get; set; }

    public virtual DbSet<WorksheetWorkflow> WorksheetWorkflows { get; set; }

    public virtual DbSet<WorksheetWorkflowDataMap> WorksheetWorkflowDataMaps { get; set; }

    public virtual DbSet<WorksheetWorkflowData> WorksheetWorkflowData { get; set; }

    public virtual DbSet<WorksheetWorkflowStorageMap> WorksheetWorkflowStorageMaps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseNpgsql("name=DefaultConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyworksheetContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
