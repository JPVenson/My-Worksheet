using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWorksheet.Website.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLoggerLog",
                columns: table => new
                {
                    AppLoggerLog_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateInserted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AppLogge__63EB1FEC99B56109", x => x.AppLoggerLog_Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingFrameLookup",
                columns: table => new
                {
                    BillingFrameLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayKey = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BillingF__70B27A955F9709F1", x => x.BillingFrameLookup_Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientStructure",
                columns: table => new
                {
                    ClientStructure_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuItemOnly = table.Column<bool>(type: "boolean", nullable: false),
                    CanBeDirectlyNavigated = table.Column<bool>(type: "boolean", nullable: false),
                    ParentRoute = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayRoute = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    AdditonalInfos = table.Column<string>(type: "text", nullable: true),
                    ControllerName = table.Column<string>(type: "text", nullable: true),
                    UrlRoute = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    InActiveNotice = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClientSt__A0DF204E2323EB4A", x => x.ClientStructure_Id);
                    table.ForeignKey(
                        name: "FK_ClientStructure_ClientStructure",
                        column: x => x.ParentRoute,
                        principalTable: "ClientStructure",
                        principalColumn: "ClientStructure_Id");
                });

            migrationBuilder.CreateTable(
                name: "CMSContent",
                columns: table => new
                {
                    CMSContnetID = table.Column<Guid>(type: "uuid", nullable: false),
                    Content_ID = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Content_Lang = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, defaultValue: "DE"),
                    Content_Template = table.Column<string>(type: "text", nullable: true),
                    IsJSONBlob = table.Column<bool>(type: "boolean", nullable: false),
                    RequireAuth = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CMSConte__CF54045F91475EAE", x => x.CMSContnetID);
                });

            migrationBuilder.CreateTable(
                name: "ContactEntry",
                columns: table => new
                {
                    ContactEntry_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EMail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    SenderIP = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ContactType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ContactE__837C92F2C0FD6F2F", x => x.ContactEntry_ID);
                });

            migrationBuilder.CreateTable(
                name: "DashboardPlugin",
                columns: table => new
                {
                    DashboardPlugin_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArgumentsQuery = table.Column<string>(type: "character varying(1800)", maxLength: 1800, nullable: true),
                    GridWidth = table.Column<int>(type: "integer", nullable: false),
                    GridHeight = table.Column<int>(type: "integer", nullable: false),
                    GridX = table.Column<int>(type: "integer", nullable: false),
                    GridY = table.Column<int>(type: "integer", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dashboar__C4DB2EB59F8746C6", x => x.DashboardPlugin_Id);
                });

            migrationBuilder.CreateTable(
                name: "HostedStorageBlob",
                columns: table => new
                {
                    HostedStorageBlob_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "md5(random()::text || clock_timestamp()::text)::uuid"),
                    Value = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HostedSt__A8E7782454F63DCE", x => x.HostedStorageBlob_Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenceGroup",
                columns: table => new
                {
                    LicenceGroup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descriptor = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LicenceG__A1ED2FB4F63CF692", x => x.LicenceGroup_Id);
                });

            migrationBuilder.CreateTable(
                name: "MailBlacklist",
                columns: table => new
                {
                    MailBlacklist_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClearName = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    X2Hash = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MailBlac__CDF7559396AB43BF", x => x.MailBlacklist_Id);
                });

            migrationBuilder.CreateTable(
                name: "Maintainace",
                columns: table => new
                {
                    Maintainace_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Until = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CallerIp = table.Column<string>(type: "text", nullable: false),
                    CompiledView = table.Column<string>(type: "text", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Maintain__2625DEFA4D224C0B", x => x.Maintainace_Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRoleLookup",
                columns: table => new
                {
                    OrganisationRoleLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organisa__CCD48189B32782BF", x => x.OrganisationRoleLookup_Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingWebhookCase",
                columns: table => new
                {
                    OutgoingWebhookCase_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DescriptionHtml = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Outgoing__34D0A6874D990802", x => x.OutgoingWebhookCase_Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProvider",
                columns: table => new
                {
                    PaymentProvider_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PaymentKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentP__97C6F421B69212DE", x => x.PaymentProvider_Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAddressMapLookup",
                columns: table => new
                {
                    ProjectAddressMapLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DisplayKey = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DescriptionKey = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectA__DA4D65583947D8B1", x => x.ProjectAddressMapLookup_Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectChargeRate",
                columns: table => new
                {
                    ProjectChargeRate_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayKey = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectC__318A3D87E517D337", x => x.ProjectChargeRate_Id);
                });

            migrationBuilder.CreateTable(
                name: "PromisedFeature",
                columns: table => new
                {
                    PromisedFeature_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayKey = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ReoccuringFeature = table.Column<bool>(type: "boolean", nullable: false),
                    InclusiveFeature = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__9288D982D9D4CC2D", x => x.PromisedFeature_Id);
                });

            migrationBuilder.CreateTable(
                name: "PromisedFeatureRegion",
                columns: table => new
                {
                    PromisedFeatureRegion_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegionName = table.Column<string>(type: "text", nullable: false),
                    RegionShortName = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__60049507A4ECC70B", x => x.PromisedFeatureRegion_Id);
                });

            migrationBuilder.CreateTable(
                name: "Realm",
                columns: table => new
                {
                    Realm_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Named = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Realm__AAE78C15B8122F4D", x => x.Realm_Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportingDataSource",
                columns: table => new
                {
                    ReportingDataSource_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reportin__42E76A8D52184CA9", x => x.ReportingDataSource_Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Role_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__D80AB4BB68E9D82A", x => x.Role_Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageType",
                columns: table => new
                {
                    StorageType_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StorageT__087172C4EA898CBA", x => x.StorageType_Id);
                });

            migrationBuilder.CreateTable(
                name: "TextResource",
                columns: table => new
                {
                    Id_TextResource = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Lang = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, defaultValue: "DE"),
                    Page = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TextReso__2E280A4EB48D1CF1", x => x.Id_TextResource);
                });

            migrationBuilder.CreateTable(
                name: "UserAssosiatedRoleLookup",
                columns: table => new
                {
                    UserAssosiatedRoleLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAsso__1891B2C46E63CA18", x => x.UserAssosiatedRoleLookup_Id);
                });

            migrationBuilder.CreateTable(
                name: "WorksheetStatusLookup",
                columns: table => new
                {
                    WorksheetStatusLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayKey = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DescriptionKey = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    AllowModifications = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__BB44935C5AAACAC9", x => x.WorksheetStatusLookup_Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenceEntry",
                columns: table => new
                {
                    LicenceEntry_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Descriptor = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Id_LicenceGroup = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProgramKey = table.Column<string>(type: "text", nullable: false, defaultValueSql: "md5(random()::text || clock_timestamp()::text)::uuid::text"),
                    IsUsernameRelevant = table.Column<string>(type: "text", nullable: true),
                    IsPcNameRelevant = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LicenceE__9A97FEC4779E583D", x => x.LicenceEntry_Id);
                    table.ForeignKey(
                        name: "FK_LicenceEntry_LicenceGroup",
                        column: x => x.Id_LicenceGroup,
                        principalTable: "LicenceGroup",
                        principalColumn: "LicenceGroup_Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviderForRegionMap",
                columns: table => new
                {
                    PaymentProviderForRegionMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_PaymentProvider = table.Column<Guid>(type: "uuid", nullable: false),
                    Region_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentP__A9D4368BAA30115F", x => x.PaymentProviderForRegionMap_Id);
                    table.ForeignKey(
                        name: "FK_PaymentProviderForRegionMap_PaymentProvider",
                        column: x => x.Id_PaymentProvider,
                        principalTable: "PaymentProvider",
                        principalColumn: "PaymentProvider_Id");
                    table.ForeignKey(
                        name: "FK_PaymentProviderForRegionMap_Region",
                        column: x => x.Region_Id,
                        principalTable: "PromisedFeatureRegion",
                        principalColumn: "PromisedFeatureRegion_Id");
                });

            migrationBuilder.CreateTable(
                name: "PromisedFeatureContent",
                columns: table => new
                {
                    PromisedFeatureContent_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DescriptionShort = table.Column<string>(type: "text", nullable: true),
                    DescriptionLong = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LimitTo = table.Column<int>(type: "integer", nullable: true),
                    LimitToUser = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Id_PromisedFeatureRegion = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_PromisedFeature = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__0B5A49DFCEF4D3B3", x => x.PromisedFeatureContent_Id);
                    table.ForeignKey(
                        name: "FK_PromisedFeatureContent_PromisedFeature",
                        column: x => x.Id_PromisedFeature,
                        principalTable: "PromisedFeature",
                        principalColumn: "PromisedFeature_Id");
                    table.ForeignKey(
                        name: "FK_PromisedFeatureContent_Region",
                        column: x => x.Id_PromisedFeatureRegion,
                        principalTable: "PromisedFeatureRegion",
                        principalColumn: "PromisedFeatureRegion_Id");
                });

            migrationBuilder.CreateTable(
                name: "Processor",
                columns: table => new
                {
                    Processor_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalIdentity = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    IpOrHostname = table.Column<string>(type: "text", nullable: false),
                    AuthKey = table.Column<string>(type: "text", nullable: false),
                    Id_Realm = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__8B9681ECB719FA44", x => x.Processor_Id);
                    table.ForeignKey(
                        name: "FK_Processor_Realm",
                        column: x => x.Id_Realm,
                        principalTable: "Realm",
                        principalColumn: "Realm_Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientSturctureRight",
                columns: table => new
                {
                    ClientSturctureRight_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Role = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ClientStructure = table.Column<Guid>(type: "uuid", nullable: false),
                    Inverse = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ClientSt__9E26344370E84D35", x => x.ClientSturctureRight_Id);
                    table.ForeignKey(
                        name: "FK_ClientSturctureRight_ClientStructure",
                        column: x => x.Id_ClientStructure,
                        principalTable: "ClientStructure",
                        principalColumn: "ClientStructure_Id");
                    table.ForeignKey(
                        name: "FK_ClientSturctureRight_Role",
                        column: x => x.Id_Role,
                        principalTable: "Role",
                        principalColumn: "Role_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetWorkflow",
                columns: table => new
                {
                    WorksheetWorkflow_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DisplayKey = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    NeedsCustomData = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    Id_DefaultStep = table.Column<Guid>(type: "uuid", nullable: false),
                    WorksheetStatusLookupId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__1F09726A9D7BB59F", x => x.WorksheetWorkflow_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflow_DefaultStep",
                        column: x => x.Id_DefaultStep,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflow_WorksheetStatusLookup_WorksheetStatusLook~",
                        column: x => x.WorksheetStatusLookupId,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcessorCapability",
                columns: table => new
                {
                    ProcessorCapability_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Id_Processor = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Processo__34B57F17D2CC7C01", x => x.ProcessorCapability_Id);
                    table.ForeignKey(
                        name: "FK_ProcessorCapability_Processor",
                        column: x => x.Id_Processor,
                        principalTable: "Processor",
                        principalColumn: "Processor_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetStatusLookupMap",
                columns: table => new
                {
                    WorksheetStatusLookupMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_FromStatus = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ToStatus = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Workflow = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__CAD389088D1F4B8D", x => x.WorksheetStatusLookupMap_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetStatusLookupMap_WorkflowStatusLookup_Child",
                        column: x => x.Id_ToStatus,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetStatusLookupMap_WorkflowStatusLookup_Parent",
                        column: x => x.Id_FromStatus,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetStatusLookupMap_WorksheetWorkflow",
                        column: x => x.Id_Workflow,
                        principalTable: "WorksheetWorkflow",
                        principalColumn: "WorksheetWorkflow_Id");
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Address_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Street = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    StreetNo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ZipCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    City = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    Country = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EMailAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DateOfCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Organisation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__03BDEBBAF4BCE223", x => x.Address_Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    AppUser_ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(56)", maxLength: 56, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ContactName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsAktive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    MailVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MailVerifiedCounter = table.Column<byte>(type: "smallint", nullable: false),
                    AllowUpdates = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", maxLength: 64, nullable: false),
                    NeedPasswordReset = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AllowFeatureRedeeming = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsTestUser = table.Column<bool>(type: "boolean", nullable: false),
                    Id_Address = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Country = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RowState = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__FD254C17FA38945C", x => x.AppUser_ID);
                    table.ForeignKey(
                        name: "FK_Users_Address",
                        column: x => x.Id_Address,
                        principalTable: "Address",
                        principalColumn: "Address_Id");
                    table.ForeignKey(
                        name: "FK_Users_Country",
                        column: x => x.Id_Country,
                        principalTable: "PromisedFeatureRegion",
                        principalColumn: "PromisedFeatureRegion_Id");
                });

            migrationBuilder.CreateTable(
                name: "Organisation",
                columns: table => new
                {
                    Organisation_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Address = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    SharedId = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Id_ParentOrganisation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__0BAD4DFC23CB7ED0", x => x.Organisation_Id);
                    table.ForeignKey(
                        name: "FK_Organisation_Address",
                        column: x => x.Id_Address,
                        principalTable: "Address",
                        principalColumn: "Address_Id");
                    table.ForeignKey(
                        name: "FK_Organisation_OrganisationParent",
                        column: x => x.Id_ParentOrganisation,
                        principalTable: "Organisation",
                        principalColumn: "Organisation_Id");
                });

            migrationBuilder.CreateTable(
                name: "AppNumberRange",
                columns: table => new
                {
                    AppNumberRange_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    Counter = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Id_User = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AppNumbe__BE7026E5F9C438C6", x => x.AppNumberRange_Id);
                    table.ForeignKey(
                        name: "FK_AppNumberRange_AppUser",
                        column: x => x.Id_User,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "AssosiationInvitation",
                columns: table => new
                {
                    AssosiationInvitation_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidOnce = table.Column<bool>(type: "boolean", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokeReason = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: true),
                    Id_RequestingUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_UserAssosiatedRoleLookup = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__6ABA25EFAB3F095E", x => x.AssosiationInvitation_Id);
                    table.ForeignKey(
                        name: "FK_AssosiationInvitation_AppUser",
                        column: x => x.Id_RequestingUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_AssosiationInvitation_UserAssosiatedRoleLookup",
                        column: x => x.Id_UserAssosiatedRoleLookup,
                        principalTable: "UserAssosiatedRoleLookup",
                        principalColumn: "UserAssosiatedRoleLookup_Id");
                });

            migrationBuilder.CreateTable(
                name: "LoginToken",
                columns: table => new
                {
                    LoginToken_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemoteIp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoginTok__F559EA3AB1F090CD", x => x.LoginToken_Id);
                    table.ForeignKey(
                        name: "FK_LoginToken_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "MailAccount",
                columns: table => new
                {
                    MailAccount_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EMailAddress = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Protocol = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ServerAddress = table.Column<string>(type: "text", nullable: false),
                    ServerPort = table.Column<int>(type: "integer", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MailAcco__5322B5A5B44D49B6", x => x.MailAccount_Id);
                    table.ForeignKey(
                        name: "FK_MailAccount_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "NEngineTemplate",
                columns: table => new
                {
                    NEngineTemplate_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    FileNameTemplate = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    UsedDataSource = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UsedFormattingEngine = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: "Morestachio"),
                    FileExtention = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__32ECFC437F14191A", x => x.NEngineTemplate_Id);
                    table.ForeignKey(
                        name: "FK_NEngineTemplate_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "OutgoingWebhook",
                columns: table => new
                {
                    OutgoingWebhook_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CallingUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Id_OutgoingWebhookCase = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeactivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    NumberRangeEntry = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__C4C1E8C55C66A95A", x => x.OutgoingWebhook_Id);
                    table.ForeignKey(
                        name: "FK_OutgoingWebhook_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_OutgoingWebhook_OutgoingWebhookCase",
                        column: x => x.Id_OutgoingWebhookCase,
                        principalTable: "OutgoingWebhookCase",
                        principalColumn: "OutgoingWebhookCase_Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentInfo",
                columns: table => new
                {
                    PaymentInfo_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PaymentDisclaimer = table.Column<string>(type: "text", nullable: false),
                    PaymentTarget = table.Column<int>(type: "integer", nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__CA91502AEBBA0D3C", x => x.PaymentInfo_Id);
                    table.ForeignKey(
                        name: "FK_PaymentInfo_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrder",
                columns: table => new
                {
                    Order_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOrderDone = table.Column<bool>(type: "boolean", nullable: false),
                    OrderResolveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrderCreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsOrderSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    OrderError = table.Column<string>(type: "text", nullable: true),
                    TransactionInfos = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_PaymentProvider = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_PromisedFeatureContent = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__F1E4607BB99445FF", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK_Order_Appuser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_Order_PaymentProvider",
                        column: x => x.Id_PaymentProvider,
                        principalTable: "PaymentProvider",
                        principalColumn: "PaymentProvider_Id");
                    table.ForeignKey(
                        name: "FK_Order_PromisedFeatureContent",
                        column: x => x.Id_PromisedFeatureContent,
                        principalTable: "PromisedFeatureContent",
                        principalColumn: "PromisedFeatureContent_Id");
                });

            migrationBuilder.CreateTable(
                name: "PriorityQueueItem",
                columns: table => new
                {
                    PriorityQueueItem_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Parent = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "(NULL)"),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionKey = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    DataArguments = table.Column<string>(type: "xml", nullable: false),
                    Version = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Level = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfCreation = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DateOfCreationOffset = table.Column<short>(type: "smallint", nullable: false),
                    DateOfDone = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DateOfDoneOffset = table.Column<short>(type: "smallint", nullable: true),
                    Done = table.Column<bool>(type: "boolean", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Priority__88D7B9C7E30554FF", x => x.PriorityQueueItem_Id);
                    table.ForeignKey(
                        name: "FK_PriorityQueueItem_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_PriorityQueueItem_PriorityQueueItem",
                        column: x => x.Id_Parent,
                        principalTable: "PriorityQueueItem",
                        principalColumn: "PriorityQueueItem_Id");
                });

            migrationBuilder.CreateTable(
                name: "RemoteStorage",
                columns: table => new
                {
                    RemoteStorage_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessKey = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    LastTimeSinceHeatbeat = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SessionKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RemoteSt__5E6829A4ECC895E3", x => x.RemoteStorage_Id);
                    table.ForeignKey(
                        name: "FK_RemoteStorage_ToTable",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "SettingsGroup",
                columns: table => new
                {
                    SettingsGroup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Key = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__D1E0B49FB3A6A27A", x => x.SettingsGroup_Id);
                    table.ForeignKey(
                        name: "FK_SettingsGroup_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "StorageProvider",
                columns: table => new
                {
                    StorageProvider_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StorageKey = table.Column<string>(type: "text", nullable: false),
                    IsDefaultProvider = table.Column<bool>(type: "boolean", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__64324A8A0F48E341", x => x.StorageProvider_Id);
                    table.ForeignKey(
                        name: "FK_StorageProvider_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "UserAction",
                columns: table => new
                {
                    UserAction_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserActionType = table.Column<byte>(type: "smallint", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActionIP = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PcName = table.Column<string>(type: "text", nullable: false),
                    Id_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserActi__3C1ED4D1224DB7C8", x => x.UserAction_Id);
                    table.ForeignKey(
                        name: "FK_UserAction_User",
                        column: x => x.Id_User,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "UserActivity",
                columns: table => new
                {
                    UserActivity_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: true),
                    HeaderHtml = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    BodyHtml = table.Column<string>(type: "text", nullable: false),
                    FooterHtml = table.Column<string>(type: "text", nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SystemActivityTypeKey = table.Column<string>(type: "text", nullable: false),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    Activated = table.Column<bool>(type: "boolean", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__A41D3D638E7F5914", x => x.UserActivity_Id);
                    table.ForeignKey(
                        name: "FK_UserActivity_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "UserDocumentCache",
                columns: table => new
                {
                    UserDocumentCache_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Link = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Id_User = table.Column<Guid>(type: "uuid", nullable: false),
                    HostenOn = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FileType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserDocu__DDFC5080D1E8002C", x => x.UserDocumentCache_Id);
                    table.ForeignKey(
                        name: "FK_UserDocumentCache_User",
                        column: x => x.Id_User,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "UserQuota",
                columns: table => new
                {
                    UserQuota_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotaUnlimited = table.Column<bool>(type: "boolean", nullable: false),
                    QuotaValue = table.Column<int>(type: "integer", nullable: false),
                    QuotaMax = table.Column<int>(type: "integer", nullable: false),
                    QuotaMin = table.Column<int>(type: "integer", nullable: false),
                    QuotaType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__1928E770E6C54627", x => x.UserQuota_Id);
                    table.ForeignKey(
                        name: "FK_UserScota_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMap",
                columns: table => new
                {
                    UserRoleMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Role = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__5CE0326DE294FA89", x => x.UserRoleMap_Id);
                    table.ForeignKey(
                        name: "FK_UserRoleMap_Role",
                        column: x => x.Id_Role,
                        principalTable: "Role",
                        principalColumn: "Role_Id");
                    table.ForeignKey(
                        name: "FK_UserRoleMap_User",
                        column: x => x.Id_User,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetGeoFence",
                columns: table => new
                {
                    WorksheetGeoFence_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__B06775C0721FE514", x => x.WorksheetGeoFence_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetGeoFence_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetItemStatusLookup",
                columns: table => new
                {
                    WorksheetItemStatusLookup_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Action = table.Column<string>(type: "text", nullable: true),
                    ActionMeta = table.Column<string>(type: "text", nullable: true),
                    IsPersitent = table.Column<bool>(type: "boolean", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__0E5846B077316D57", x => x.WorksheetItemStatusLookup_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetItemStatusLookup_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "OrganisationUserMap",
                columns: table => new
                {
                    OrganisationUserMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Organisation = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Relation = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organisa__D6FA8A88E4987241", x => x.OrganisationUserMap_Id);
                    table.ForeignKey(
                        name: "FK_OrganisationUserMap_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_OrganisationUserMap_Organisation",
                        column: x => x.Id_Organisation,
                        principalTable: "Organisation",
                        principalColumn: "Organisation_Id");
                    table.ForeignKey(
                        name: "FK_OrganisationUserMap_OrganisationRoleLookup",
                        column: x => x.Id_Relation,
                        principalTable: "OrganisationRoleLookup",
                        principalColumn: "OrganisationRoleLookup_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetWorkflowDataMap",
                columns: table => new
                {
                    WorksheetWorkflowDataMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_WorksheetWorkflow = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupKey = table.Column<string>(type: "text", nullable: true),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_SharedWithOrganisation = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__527DBE028EC614C7", x => x.WorksheetWorkflowDataMap_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowDataMap_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowDataMap_Organisation",
                        column: x => x.Id_SharedWithOrganisation,
                        principalTable: "Organisation",
                        principalColumn: "Organisation_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowDataMap_Workflow",
                        column: x => x.Id_WorksheetWorkflow,
                        principalTable: "WorksheetWorkflow",
                        principalColumn: "WorksheetWorkflow_Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAssoisiatedUserMap",
                columns: table => new
                {
                    UserAssoisiatedUserMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ParentUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ChildUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_UserRelation = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Invite = table.Column<Guid>(type: "uuid", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAsso__100198A57A2BDA46", x => x.UserAssoisiatedUserMap_Id);
                    table.ForeignKey(
                        name: "FK_UserAssoisiatedUserMap_Child_AppUser",
                        column: x => x.Id_ChildUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_UserAssoisiatedUserMap_Invite",
                        column: x => x.Id_Invite,
                        principalTable: "AssosiationInvitation",
                        principalColumn: "AssosiationInvitation_Id");
                    table.ForeignKey(
                        name: "FK_UserAssoisiatedUserMap_Parent_AppUser",
                        column: x => x.Id_ParentUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_UserAssoisiatedUserMap_UserRelation",
                        column: x => x.Id_UserRelation,
                        principalTable: "UserAssosiatedRoleLookup",
                        principalColumn: "UserAssosiatedRoleLookup_Id");
                });

            migrationBuilder.CreateTable(
                name: "MailAccountUserMap",
                columns: table => new
                {
                    MailAccountUserMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_MailAccount = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    CanEdit = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MailAcco__3C3D8FED53258EBE", x => x.MailAccountUserMap_Id);
                    table.ForeignKey(
                        name: "FK_MailAccountUserMap_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_MailAccountUserMap_MailAccount",
                        column: x => x.Id_MailAccount,
                        principalTable: "MailAccount",
                        principalColumn: "MailAccount_Id");
                });

            migrationBuilder.CreateTable(
                name: "OutgoingWebhookActionLog",
                columns: table => new
                {
                    OutgoingWebhookActionLog_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    ReturnCode = table.Column<int>(type: "integer", nullable: false),
                    DateOfAction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    InitiatorIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Id_OutgoingWebhook = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__8CAA548A18FD584D", x => x.OutgoingWebhookActionLog_Id);
                    table.ForeignKey(
                        name: "FK_OutgoingWebhookActionLog_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_OutgoingWebhookActionLog_OutgoingWebhook",
                        column: x => x.Id_OutgoingWebhook,
                        principalTable: "OutgoingWebhook",
                        principalColumn: "OutgoingWebhook_Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentInfoContent",
                columns: table => new
                {
                    PaymentInfoContent_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FieldValue = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Id_PaymentInfo = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentI__C36490BBA67A6C62", x => x.PaymentInfoContent_Id);
                    table.ForeignKey(
                        name: "FK_PaymentInfoContent_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_PaymentInfoContent_PaymentInfo",
                        column: x => x.Id_PaymentInfo,
                        principalTable: "PaymentInfo",
                        principalColumn: "PaymentInfo_Id");
                });

            migrationBuilder.CreateTable(
                name: "PromisedFeatureToAppUserMap",
                columns: table => new
                {
                    PromisedFeatureToAppUserMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Order = table.Column<Guid>(type: "uuid", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Feature = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__96FCDA194163B3E0", x => x.PromisedFeatureToAppUserMap_Id);
                    table.ForeignKey(
                        name: "FK_PromisedFeatureToAppUserMap_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_PromisedFeatureToAppUserMap_Order",
                        column: x => x.Id_Order,
                        principalTable: "PaymentOrder",
                        principalColumn: "Order_Id");
                });

            migrationBuilder.CreateTable(
                name: "SettingsValue",
                columns: table => new
                {
                    SettingsValue_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    RowState = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    Id_SettingsGroup = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__930AD0B52472DBEA", x => x.SettingsValue_Id);
                    table.ForeignKey(
                        name: "FK_SettingsValue_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_SettingsValue_SettingsGroup",
                        column: x => x.Id_SettingsGroup,
                        principalTable: "SettingsGroup",
                        principalColumn: "SettingsGroup_Id");
                });

            migrationBuilder.CreateTable(
                name: "StorageEntry",
                columns: table => new
                {
                    StorageEntry_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ThumbnailOf = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Id_StorageType = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_StorageProvider = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__881100EF220D283F", x => x.StorageEntry_Id);
                    table.ForeignKey(
                        name: "FK_StorageEntry_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_StorageEntry_StorageEntry",
                        column: x => x.ThumbnailOf,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                    table.ForeignKey(
                        name: "FK_StorageEntry_StorageProvider",
                        column: x => x.Id_StorageProvider,
                        principalTable: "StorageProvider",
                        principalColumn: "StorageProvider_Id");
                    table.ForeignKey(
                        name: "FK_StorageEntry_StorageType",
                        column: x => x.Id_StorageType,
                        principalTable: "StorageType",
                        principalColumn: "StorageType_Id");
                });

            migrationBuilder.CreateTable(
                name: "StorageProviderData",
                columns: table => new
                {
                    StorageProviderData_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_StorageProvider = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__5084260F7572CBB8", x => x.StorageProviderData_Id);
                    table.ForeignKey(
                        name: "FK_StorageProviderData_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_StorageProviderData_StorageProvider",
                        column: x => x.Id_StorageProvider,
                        principalTable: "StorageProvider",
                        principalColumn: "StorageProvider_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetGeoFenceLocation",
                columns: table => new
                {
                    WorksheetGeoFence_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(10,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(10,6)", nullable: false),
                    FenceGroup = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Id_WorksheetGeoFence = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__B06775C035ABBBFD", x => x.WorksheetGeoFence_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetGeoFenceLocation_WorksheetGeoFence",
                        column: x => x.Id_WorksheetGeoFence,
                        principalTable: "WorksheetGeoFence",
                        principalColumn: "WorksheetGeoFence_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetGeoFenceWiFi",
                columns: table => new
                {
                    WorksheetGeoFenceWiFi_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Id_WorksheetGeoFence = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__7A2BCB18EDF891B4", x => x.WorksheetGeoFenceWiFi_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetGeoFenceWiFi_WorksheetGeoFence",
                        column: x => x.Id_WorksheetGeoFence,
                        principalTable: "WorksheetGeoFence",
                        principalColumn: "WorksheetGeoFence_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetWorkflowData",
                columns: table => new
                {
                    WorksheetWorkflowData_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Id_WorksheetWorkflowMap = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__253274EE1B08BC36", x => x.WorksheetWorkflowData_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowData_WorkflowMap",
                        column: x => x.Id_WorksheetWorkflowMap,
                        principalTable: "WorksheetWorkflowDataMap",
                        principalColumn: "WorksheetWorkflowDataMap_Id");
                });

            migrationBuilder.CreateTable(
                name: "MailSend",
                columns: table => new
                {
                    MailSend_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_MailAccount = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Content = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Attachment = table.Column<Guid>(type: "uuid", nullable: true),
                    SendAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Recipients = table.Column<string>(type: "text", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    ResendCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__BA32164C420AC836", x => x.MailSend_Id);
                    table.ForeignKey(
                        name: "FK_MailSend_Attachment",
                        column: x => x.Id_Attachment,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                    table.ForeignKey(
                        name: "FK_MailSend_Content",
                        column: x => x.Id_Content,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                    table.ForeignKey(
                        name: "FK_MailSend_MailAccount",
                        column: x => x.Id_MailAccount,
                        principalTable: "MailAccount",
                        principalColumn: "MailAccount_Id");
                });

            migrationBuilder.CreateTable(
                name: "MustachioTemplateFormatter",
                columns: table => new
                {
                    MustachioTemplateFormatter_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "character varying(350)", maxLength: 350, nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_StorageEntry = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mustachi__10197881F21C7051", x => x.MustachioTemplateFormatter_Id);
                    table.ForeignKey(
                        name: "FK_MustachioTemplateFormatter_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_MustachioTemplateFormatter_StorageEntry",
                        column: x => x.Id_StorageEntry,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                });

            migrationBuilder.CreateTable(
                name: "NEngineRunningTask",
                columns: table => new
                {
                    NEngineRunningTask_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDone = table.Column<bool>(type: "boolean", nullable: false),
                    IsFaulted = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorText = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Id_StoreageEntry = table.Column<Guid>(type: "uuid", nullable: true),
                    ArgumentsRepresentation = table.Column<string>(type: "text", nullable: true),
                    Id_NEngineTemplate = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Processor = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPreview = table.Column<bool>(type: "boolean", nullable: false),
                    IsObsolete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__757D801D6DA41FE6", x => x.NEngineRunningTask_Id);
                    table.ForeignKey(
                        name: "FK_NEngineRunningTask_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_NEngineRunningTask_NEngineTemplate",
                        column: x => x.Id_NEngineTemplate,
                        principalTable: "NEngineTemplate",
                        principalColumn: "NEngineTemplate_Id");
                    table.ForeignKey(
                        name: "FK_NEngineRunningTask_Processor",
                        column: x => x.Id_Processor,
                        principalTable: "Processor",
                        principalColumn: "Processor_Id");
                    table.ForeignKey(
                        name: "FK_NEngineRunningTask_Storeage",
                        column: x => x.Id_StoreageEntry,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                });

            migrationBuilder.CreateTable(
                name: "OvertimeAccount",
                columns: table => new
                {
                    OvertimeAccount_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OvertimeValue = table.Column<decimal>(type: "numeric(25,8)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Overtime__A1EAD810CEF0D8B7", x => x.OvertimeAccount_Id);
                    table.ForeignKey(
                        name: "FK_OvertimeAccount_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                });

            migrationBuilder.CreateTable(
                name: "OvertimeTransaction",
                columns: table => new
                {
                    OvertimeTransaction_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Withdraw = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(25,8)", nullable: false),
                    DateOfAction = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DateOfActionOffset = table.Column<short>(type: "smallint", nullable: false),
                    Id_OvertimeAccount = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Overtime__3A7C842763D76B2E", x => x.OvertimeTransaction_Id);
                    table.ForeignKey(
                        name: "FK_OvertimeTransaction_OvertimeAccount",
                        column: x => x.Id_OvertimeAccount,
                        principalTable: "OvertimeAccount",
                        principalColumn: "OvertimeAccount_Id");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Project_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    BookToOvertimeAccount = table.Column<bool>(type: "boolean", nullable: false),
                    Id_WorksheetWorkflow = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_WorksheetWorkflowDataMap = table.Column<Guid>(type: "uuid", nullable: true),
                    UserOrderNo = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    NumberRangeEntry = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    ProjectReference = table.Column<string>(type: "text", nullable: true),
                    Id_PaymentCondition = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_BillingFrame = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Organisation = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_DefaultRate = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__1CB92E03305327D1", x => x.Project_Id);
                    table.ForeignKey(
                        name: "FK_Project_AppUser_Creator",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_Project_BillingFrame",
                        column: x => x.Id_BillingFrame,
                        principalTable: "BillingFrameLookup",
                        principalColumn: "BillingFrameLookup_Id");
                    table.ForeignKey(
                        name: "FK_Project_Organisation",
                        column: x => x.Id_Organisation,
                        principalTable: "Organisation",
                        principalColumn: "Organisation_Id");
                    table.ForeignKey(
                        name: "FK_Project_PaymentCondition",
                        column: x => x.Id_PaymentCondition,
                        principalTable: "PaymentInfo",
                        principalColumn: "PaymentInfo_Id");
                    table.ForeignKey(
                        name: "FK_Project_WorksheetWorkflow",
                        column: x => x.Id_WorksheetWorkflow,
                        principalTable: "WorksheetWorkflow",
                        principalColumn: "WorksheetWorkflow_Id");
                    table.ForeignKey(
                        name: "FK_Project_WorksheetWorkflowData",
                        column: x => x.Id_WorksheetWorkflowDataMap,
                        principalTable: "WorksheetWorkflowDataMap",
                        principalColumn: "WorksheetWorkflowDataMap_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectAddressMap",
                columns: table => new
                {
                    ProjectAddressMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Address = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ProjectAddressMapLookup = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectA__011D5FB2C2E8ED75", x => x.ProjectAddressMap_Id);
                    table.ForeignKey(
                        name: "FK_ProjectAddressMap_Address",
                        column: x => x.Id_Address,
                        principalTable: "Address",
                        principalColumn: "Address_Id");
                    table.ForeignKey(
                        name: "FK_ProjectAddressMap_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                    table.ForeignKey(
                        name: "FK_ProjectAddressMap_ProjectAddressMapLookup",
                        column: x => x.Id_ProjectAddressMapLookup,
                        principalTable: "ProjectAddressMapLookup",
                        principalColumn: "ProjectAddressMapLookup_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectAssosiatedUserMap",
                columns: table => new
                {
                    ProjectAssosiatedUser_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Read = table.Column<bool>(type: "boolean", nullable: false),
                    Write = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProjectA__9E0770E318EB069E", x => x.ProjectAssosiatedUser_Id);
                    table.ForeignKey(
                        name: "FK_ProjectAssosiatedUserMap_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_ProjectAssosiatedUserMap_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectBudget",
                columns: table => new
                {
                    ProjectBudget_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeadlineOffset = table.Column<short>(type: "smallint", nullable: true),
                    TotalTimeBudget = table.Column<int>(type: "integer", nullable: true),
                    TotalBudget = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    TimeConsumed = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    BugetConsumed = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ValidFromOffset = table.Column<short>(type: "smallint", nullable: true),
                    AllowOverbooking = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__F93118CCEE442FF9", x => x.ProjectBudget_Id);
                    table.ForeignKey(
                        name: "FK_ProjectBudget_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_ProjectBudget_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectItemRate",
                columns: table => new
                {
                    ProjectItemRate_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    CurrencyType = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ProjectChargeRate = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__1B007FB152A27875", x => x.ProjectItemRate_Id);
                    table.ForeignKey(
                        name: "FK_ProjectItemRate_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_ProjectItemRate_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                    table.ForeignKey(
                        name: "FK_ProjectItemRate_ProjectChargeRate",
                        column: x => x.Id_ProjectChargeRate,
                        principalTable: "ProjectChargeRate",
                        principalColumn: "ProjectChargeRate_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectShareKey",
                columns: table => new
                {
                    ProjectShareKey_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AllowPrinting = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAfter = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllowNonSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__EDCFEEAC0DE3C204", x => x.ProjectShareKey_Id);
                    table.ForeignKey(
                        name: "FK_ProjectShareKey_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWorkload",
                columns: table => new
                {
                    UserWorkload_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkTimeMode = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    DayWorkTimeMonday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeTuesday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeWednesday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeThursday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeFriday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeSaturday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    DayWorkTimeSunday = table.Column<decimal>(type: "numeric(18,0)", nullable: true),
                    WeekWorktime = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    MonthWorktime = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Organisation = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserWork__5240926C91EB3ED3", x => x.UserWorkload_Id);
                    table.ForeignKey(
                        name: "FK_UserWorkload_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_UserWorkload_Organisation",
                        column: x => x.Id_Organisation,
                        principalTable: "Organisation",
                        principalColumn: "Organisation_Id");
                    table.ForeignKey(
                        name: "FK_UserWorkload_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                });

            migrationBuilder.CreateTable(
                name: "Worksheet",
                columns: table => new
                {
                    Worksheet_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartTimeOffset = table.Column<short>(type: "smallint", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndTimeOffset = table.Column<short>(type: "smallint", nullable: true),
                    No = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ServiceDescription = table.Column<string>(type: "text", nullable: true),
                    InvoiceDueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    InvoiceDueDateOffset = table.Column<short>(type: "smallint", nullable: true),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    NumberRangeEntry = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Id_CurrentStatus = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_WorksheetWorkflow = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_WorksheetWorkflowDataMap = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__342F12B7AC1FF5A2", x => x.Worksheet_Id);
                    table.ForeignKey(
                        name: "FK_Worksheet_AppUser_Creator",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_Worksheet_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                    table.ForeignKey(
                        name: "FK_Worksheet_Status",
                        column: x => x.Id_CurrentStatus,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_Worksheet_WorkflowMode",
                        column: x => x.Id_WorksheetWorkflow,
                        principalTable: "WorksheetWorkflow",
                        principalColumn: "WorksheetWorkflow_Id");
                    table.ForeignKey(
                        name: "FK_Worksheet_WorkflowMode_Data",
                        column: x => x.Id_WorksheetWorkflowDataMap,
                        principalTable: "WorksheetWorkflowDataMap",
                        principalColumn: "WorksheetWorkflowDataMap_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetAssert",
                columns: table => new
                {
                    WorksheetAssert_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(25,5)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(25,5)", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Worksheet = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_Project = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__DD325CE5FE0C0C79", x => x.WorksheetAssert_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetAssert_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetAssert_Project",
                        column: x => x.Id_Project,
                        principalTable: "Project",
                        principalColumn: "Project_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetAssert_Worksheet",
                        column: x => x.Id_Worksheet,
                        principalTable: "Worksheet",
                        principalColumn: "Worksheet_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetItem",
                columns: table => new
                {
                    WorksheetItem_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Worksheet = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfAction = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateOfActionOffset = table.Column<short>(type: "smallint", nullable: false),
                    FromTime = table.Column<int>(type: "integer", nullable: false),
                    ToTime = table.Column<int>(type: "integer", nullable: false),
                    Hidden = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ProjectItemRate = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__B5DF312B2D8164C7", x => x.WorksheetItem_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetItem_AppUser_Creator",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetItem_ProjectItemRate",
                        column: x => x.Id_ProjectItemRate,
                        principalTable: "ProjectItemRate",
                        principalColumn: "ProjectItemRate_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetItem_Worksheet",
                        column: x => x.Id_Worksheet,
                        principalTable: "Worksheet",
                        principalColumn: "Worksheet_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetStatusHistory",
                columns: table => new
                {
                    WorksheetStatusHistory_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfAction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    SystemComment = table.Column<string>(type: "text", nullable: true),
                    Id_Worksheet = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_PreState = table.Column<Guid>(type: "uuid", nullable: true),
                    Id_PostState = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ChangeUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__D8707A34A3F8A99F", x => x.WorksheetStatusHistory_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetStatusHistory_AppUser",
                        column: x => x.Id_ChangeUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetStatusHistory_PostState_WorksheetStatus",
                        column: x => x.Id_PostState,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetStatusHistory_PreState_WorksheetStatus",
                        column: x => x.Id_PreState,
                        principalTable: "WorksheetStatusLookup",
                        principalColumn: "WorksheetStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetStatusHistory_Worksheet",
                        column: x => x.Id_Worksheet,
                        principalTable: "Worksheet",
                        principalColumn: "Worksheet_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetTrack",
                columns: table => new
                {
                    WorksheetTrack_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Worksheet = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_ProjectItemRate = table.Column<Guid>(type: "uuid", nullable: false),
                    DateStarted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateStartedOffset = table.Column<short>(type: "smallint", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__0B2A1EB7938FC005", x => x.WorksheetTrack_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetTrack_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetTrack_ProjectItemRate",
                        column: x => x.Id_ProjectItemRate,
                        principalTable: "ProjectItemRate",
                        principalColumn: "ProjectItemRate_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetTrack_Worksheet",
                        column: x => x.Id_Worksheet,
                        principalTable: "Worksheet",
                        principalColumn: "Worksheet_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetAssertsFilesMap",
                columns: table => new
                {
                    WorksheetAssertsFilesMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_WorksheetAssert = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_StorageEntry = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__BE74C751C0D3E8C5", x => x.WorksheetAssertsFilesMap_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetAssertFilesMap_StorageEntry",
                        column: x => x.Id_StorageEntry,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetAssertFilesMap_WorksheetAssert",
                        column: x => x.Id_WorksheetAssert,
                        principalTable: "WorksheetAssert",
                        principalColumn: "WorksheetAssert_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetItemStatus",
                columns: table => new
                {
                    WorksheetItemStatus_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Worksheet = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_WorksheetItemStatusLookup = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfAction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WorksheetItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tmp_ms_x__0A3540BAB40ADE3A", x => x.WorksheetItemStatus_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetItemStatus_AppUser",
                        column: x => x.Id_Creator,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetItemStatus_Worksheet",
                        column: x => x.Id_Worksheet,
                        principalTable: "Worksheet",
                        principalColumn: "Worksheet_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetItemStatus_WorksheetItemStatusLookup",
                        column: x => x.Id_WorksheetItemStatusLookup,
                        principalTable: "WorksheetItemStatusLookup",
                        principalColumn: "WorksheetItemStatusLookup_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetItemStatus_WorksheetItem_WorksheetItemId",
                        column: x => x.WorksheetItemId,
                        principalTable: "WorksheetItem",
                        principalColumn: "WorksheetItem_Id");
                });

            migrationBuilder.CreateTable(
                name: "WorksheetWorkflowStorageMap",
                columns: table => new
                {
                    WorksheetWorkflowStorageMap_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_StorageEntry = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_WorksheetStatusHistory = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_AppUser = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Workshee__FE3EDF7CDF7C4CA5", x => x.WorksheetWorkflowStorageMap_Id);
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowStorageMap_AppUser",
                        column: x => x.Id_AppUser,
                        principalTable: "AppUser",
                        principalColumn: "AppUser_ID");
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowStorageMap_StorageEntry",
                        column: x => x.Id_StorageEntry,
                        principalTable: "StorageEntry",
                        principalColumn: "StorageEntry_Id");
                    table.ForeignKey(
                        name: "FK_WorksheetWorkflowStorageMap_WorksheetStatusHistory",
                        column: x => x.Id_WorksheetStatusHistory,
                        principalTable: "WorksheetStatusHistory",
                        principalColumn: "WorksheetStatusHistory_Id");
                });

            migrationBuilder.InsertData(
                table: "OrganisationRoleLookup",
                columns: new[] { "OrganisationRoleLookup_Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), "Administrator" },
                    { new Guid("00000000-0000-0000-0001-000000000002"), "Customer" },
                    { new Guid("00000000-0000-0000-0001-000000000003"), "Creator" },
                    { new Guid("00000000-0000-0000-0001-000000000004"), "ProjectManager" }
                });

            migrationBuilder.InsertData(
                table: "OutgoingWebhookCase",
                columns: new[] { "OutgoingWebhookCase_Id", "DescriptionHtml", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0006-000000000001"), "Worksheet has changed", "Worksheet" },
                    { new Guid("00000000-0000-0000-0006-000000000002"), "Project changed", "Project" },
                    { new Guid("00000000-0000-0000-0006-000000000003"), "Worksheet Item changed", "Worksheet Item" }
                });

            migrationBuilder.InsertData(
                table: "PromisedFeatureRegion",
                columns: new[] { "PromisedFeatureRegion_Id", "Currency", "IsActive", "RegionName", "RegionShortName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0007-000000000001"), "€", true, "Germany", "de" },
                    { new Guid("00000000-0000-0000-0007-000000000002"), "$", true, "United States of America", "us" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Role_Id", "RoleName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Administrator" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Kunde" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Visitor" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "WorksheetAdmin" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "WorksheetUser" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "WorksheetActionsUser" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "OrganisationAdmin" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "SettingsUsers" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "OnDemandUser" },
                    { new Guid("00000000-0000-0000-0000-00000000000a"), "ProjectManager" }
                });

            migrationBuilder.InsertData(
                table: "StorageProvider",
                columns: new[] { "StorageProvider_Id", "Id_AppUser", "IsDefaultProvider", "Name", "StorageKey" },
                values: new object[] { new Guid("00000000-0000-0000-0005-000000000001"), null, false, "My-Worksheet Hosted", "LocalProvider" });

            migrationBuilder.InsertData(
                table: "StorageType",
                columns: new[] { "StorageType_Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0002-000000000001"), "Report" },
                    { new Guid("00000000-0000-0000-0002-000000000002"), "WorksheetAssert" },
                    { new Guid("00000000-0000-0000-0002-000000000003"), "Test" },
                    { new Guid("00000000-0000-0000-0002-000000000004"), "WorksheetDocument" },
                    { new Guid("00000000-0000-0000-0002-000000000005"), "Thumbnail" },
                    { new Guid("00000000-0000-0000-0002-000000000006"), "Email" }
                });

            migrationBuilder.InsertData(
                table: "UserAssosiatedRoleLookup",
                columns: new[] { "UserAssosiatedRoleLookup_Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0008-000000000001"), "S", "Self" },
                    { new Guid("00000000-0000-0000-0008-000000000002"), "WH", "WorksheetHolder" },
                    { new Guid("00000000-0000-0000-0008-000000000003"), "SA", "Support Admin" },
                    { new Guid("00000000-0000-0000-0008-000000000004"), "AD", "Administrator" }
                });

            migrationBuilder.InsertData(
                table: "WorksheetStatusLookup",
                columns: new[] { "WorksheetStatusLookup_Id", "AllowModifications", "DescriptionKey", "DisplayKey" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), false, "", "Workflow.Manual/StatusType.Invalid" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), true, "", "Workflow.Manual/StatusType.Created" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), false, "", "Workflow.Manual/StatusType.Submitted" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), false, "", "Workflow.Manual/StatusType.AwaitingResponse" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), false, "", "Workflow.Manual/StatusType.Confirmed" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), false, "", "Workflow.Manual/StatusType.AwaitingPayment" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), false, "", "Workflow.Manual/StatusType.Rejected" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), false, "", "Workflow.Manual/StatusType.Payed" }
                });

            migrationBuilder.InsertData(
                table: "WorksheetWorkflow",
                columns: new[] { "WorksheetWorkflow_Id", "Comment", "DisplayKey", "Id_DefaultStep", "NeedsCustomData", "ProviderKey", "WorksheetStatusLookupId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "No Automatism. All states must be triggered manual on the Webpage", "Manual", new Guid("00000000-0000-0000-0000-000000000002"), false, "ManualWorkflowImpl", null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Includes E-Mail to be send by an E-Mail provider of yours.", "E-Mail Workflow", new Guid("00000000-0000-0000-0000-000000000002"), true, "MailWorkflow", null }
                });

            migrationBuilder.InsertData(
                table: "WorksheetStatusLookupMap",
                columns: new[] { "WorksheetStatusLookupMap_Id", "Id_FromStatus", "Id_ToStatus", "Id_Workflow" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0004-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-000000000008"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-000000000009"), new Guid("00000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-00000000000a"), new Guid("00000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-00000000000b"), new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-00000000000c"), new Guid("00000000-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000009"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0004-00000000000d"), new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-00000000000e"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-00000000000f"), new Guid("00000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-000000000010"), new Guid("00000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-000000000011"), new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000009"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-000000000012"), new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0004-000000000013"), new Guid("00000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Id_AppUser",
                table: "Address",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_Address_Id_Organisation",
                table: "Address",
                column: "Id_Organisation");

            migrationBuilder.CreateIndex(
                name: "IX_AppNumberRange_Id_User",
                table: "AppNumberRange",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Id_Address",
                table: "AppUser",
                column: "Id_Address");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Id_Country",
                table: "AppUser",
                column: "Id_Country");

            migrationBuilder.CreateIndex(
                name: "UQ__tmp_ms_x__536C85E47A31F292",
                table: "AppUser",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssosiationInvitation_Id_RequestingUser",
                table: "AssosiationInvitation",
                column: "Id_RequestingUser");

            migrationBuilder.CreateIndex(
                name: "IX_AssosiationInvitation_Id_UserAssosiatedRoleLookup",
                table: "AssosiationInvitation",
                column: "Id_UserAssosiatedRoleLookup");

            migrationBuilder.CreateIndex(
                name: "IX_ClientStructure_ParentRoute",
                table: "ClientStructure",
                column: "ParentRoute");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSturctureRight_Id_ClientStructure",
                table: "ClientSturctureRight",
                column: "Id_ClientStructure");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSturctureRight_Id_Role",
                table: "ClientSturctureRight",
                column: "Id_Role");

            migrationBuilder.CreateIndex(
                name: "IX_LicenceEntry_Id_LicenceGroup",
                table: "LicenceEntry",
                column: "Id_LicenceGroup");

            migrationBuilder.CreateIndex(
                name: "IX_LoginToken_Id_AppUser",
                table: "LoginToken",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_MailAccount_Id_AppUser",
                table: "MailAccount",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_MailAccountUserMap_Id_AppUser",
                table: "MailAccountUserMap",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_MailAccountUserMap_Id_MailAccount",
                table: "MailAccountUserMap",
                column: "Id_MailAccount");

            migrationBuilder.CreateIndex(
                name: "IX_MailSend_Id_Attachment",
                table: "MailSend",
                column: "Id_Attachment");

            migrationBuilder.CreateIndex(
                name: "IX_MailSend_Id_Content",
                table: "MailSend",
                column: "Id_Content");

            migrationBuilder.CreateIndex(
                name: "IX_MailSend_Id_MailAccount",
                table: "MailSend",
                column: "Id_MailAccount");

            migrationBuilder.CreateIndex(
                name: "IX_MustachioTemplateFormatter_Id_Creator",
                table: "MustachioTemplateFormatter",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_MustachioTemplateFormatter_Id_StorageEntry",
                table: "MustachioTemplateFormatter",
                column: "Id_StorageEntry");

            migrationBuilder.CreateIndex(
                name: "IX_NEngineRunningTask_Id_Creator",
                table: "NEngineRunningTask",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_NEngineRunningTask_Id_NEngineTemplate",
                table: "NEngineRunningTask",
                column: "Id_NEngineTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_NEngineRunningTask_Id_Processor",
                table: "NEngineRunningTask",
                column: "Id_Processor");

            migrationBuilder.CreateIndex(
                name: "IX_NEngineRunningTask_Id_StoreageEntry",
                table: "NEngineRunningTask",
                column: "Id_StoreageEntry");

            migrationBuilder.CreateIndex(
                name: "IX_NEngineTemplate_Id_Creator",
                table: "NEngineTemplate",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Id_Address",
                table: "Organisation",
                column: "Id_Address");

            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Id_ParentOrganisation",
                table: "Organisation",
                column: "Id_ParentOrganisation");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUserMap_Id_AppUser",
                table: "OrganisationUserMap",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUserMap_Id_Organisation",
                table: "OrganisationUserMap",
                column: "Id_Organisation");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationUserMap_Id_Relation",
                table: "OrganisationUserMap",
                column: "Id_Relation");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingWebhook_Id_Creator",
                table: "OutgoingWebhook",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingWebhook_Id_OutgoingWebhookCase",
                table: "OutgoingWebhook",
                column: "Id_OutgoingWebhookCase");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingWebhookActionLog_Id_AppUser",
                table: "OutgoingWebhookActionLog",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingWebhookActionLog_Id_OutgoingWebhook",
                table: "OutgoingWebhookActionLog",
                column: "Id_OutgoingWebhook");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeAccount_Id_AppUser",
                table: "OvertimeAccount",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeAccount_Id_Project",
                table: "OvertimeAccount",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeTransaction_Id_OvertimeAccount",
                table: "OvertimeTransaction",
                column: "Id_OvertimeAccount");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInfo_Id_AppUser",
                table: "PaymentInfo",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInfoContent_Id_AppUser",
                table: "PaymentInfoContent",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInfoContent_Id_PaymentInfo",
                table: "PaymentInfoContent",
                column: "Id_PaymentInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_Id_AppUser",
                table: "PaymentOrder",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_Id_PaymentProvider",
                table: "PaymentOrder",
                column: "Id_PaymentProvider");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrder_Id_PromisedFeatureContent",
                table: "PaymentOrder",
                column: "Id_PromisedFeatureContent");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderForRegionMap_Id_PaymentProvider",
                table: "PaymentProviderForRegionMap",
                column: "Id_PaymentProvider");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderForRegionMap_Region_Id",
                table: "PaymentProviderForRegionMap",
                column: "Region_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityQueueItem_Id_Creator",
                table: "PriorityQueueItem",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityQueueItem_Id_Parent",
                table: "PriorityQueueItem",
                column: "Id_Parent");

            migrationBuilder.CreateIndex(
                name: "IX_Processor_Id_Realm",
                table: "Processor",
                column: "Id_Realm");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessorCapability_Id_Processor",
                table: "ProcessorCapability",
                column: "Id_Processor");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_BillingFrame",
                table: "Project",
                column: "Id_BillingFrame");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_Creator",
                table: "Project",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_DefaultRate",
                table: "Project",
                column: "Id_DefaultRate");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_Organisation",
                table: "Project",
                column: "Id_Organisation");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_PaymentCondition",
                table: "Project",
                column: "Id_PaymentCondition");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_WorksheetWorkflow",
                table: "Project",
                column: "Id_WorksheetWorkflow");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Id_WorksheetWorkflowDataMap",
                table: "Project",
                column: "Id_WorksheetWorkflowDataMap");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAddressMap_Id_Address",
                table: "ProjectAddressMap",
                column: "Id_Address");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAddressMap_Id_Project",
                table: "ProjectAddressMap",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAddressMap_Id_ProjectAddressMapLookup",
                table: "ProjectAddressMap",
                column: "Id_ProjectAddressMapLookup");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssosiatedUserMap_Id_Project",
                table: "ProjectAssosiatedUserMap",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "ProjectToUserMap_NoDuplicated",
                table: "ProjectAssosiatedUserMap",
                columns: new[] { "Id_AppUser", "Id_Project" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_Id_AppUser",
                table: "ProjectBudget",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_Id_Project",
                table: "ProjectBudget",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItemRate_Id_Creator",
                table: "ProjectItemRate",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItemRate_Id_Project",
                table: "ProjectItemRate",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItemRate_Id_ProjectChargeRate",
                table: "ProjectItemRate",
                column: "Id_ProjectChargeRate");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectShareKey_Id_Project",
                table: "ProjectShareKey",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_PromisedFeatureContent_Id_PromisedFeature",
                table: "PromisedFeatureContent",
                column: "Id_PromisedFeature");

            migrationBuilder.CreateIndex(
                name: "IX_PromisedFeatureContent_Id_PromisedFeatureRegion",
                table: "PromisedFeatureContent",
                column: "Id_PromisedFeatureRegion");

            migrationBuilder.CreateIndex(
                name: "IX_PromisedFeatureToAppUserMap_Id_AppUser",
                table: "PromisedFeatureToAppUserMap",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_PromisedFeatureToAppUserMap_Id_Order",
                table: "PromisedFeatureToAppUserMap",
                column: "Id_Order");

            migrationBuilder.CreateIndex(
                name: "IX_RemoteStorage_Id_Creator",
                table: "RemoteStorage",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsGroup_Id_AppUser",
                table: "SettingsGroup",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsValue_Id_AppUser",
                table: "SettingsValue",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsValue_Id_SettingsGroup",
                table: "SettingsValue",
                column: "Id_SettingsGroup");

            migrationBuilder.CreateIndex(
                name: "IX_StorageEntry_Id_AppUser",
                table: "StorageEntry",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_StorageEntry_Id_StorageProvider",
                table: "StorageEntry",
                column: "Id_StorageProvider");

            migrationBuilder.CreateIndex(
                name: "IX_StorageEntry_Id_StorageType",
                table: "StorageEntry",
                column: "Id_StorageType");

            migrationBuilder.CreateIndex(
                name: "IX_StorageEntry_ThumbnailOf",
                table: "StorageEntry",
                column: "ThumbnailOf");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProvider_Id_AppUser",
                table: "StorageProvider",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProviderData_Id_AppUser",
                table: "StorageProviderData",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_StorageProviderData_Id_StorageProvider",
                table: "StorageProviderData",
                column: "Id_StorageProvider");

            migrationBuilder.CreateIndex(
                name: "IX_UserAction_Id_User",
                table: "UserAction",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_Id_AppUser",
                table: "UserActivity",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssoisiatedUserMap_Id_ChildUser",
                table: "UserAssoisiatedUserMap",
                column: "Id_ChildUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssoisiatedUserMap_Id_Invite",
                table: "UserAssoisiatedUserMap",
                column: "Id_Invite");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssoisiatedUserMap_Id_UserRelation",
                table: "UserAssoisiatedUserMap",
                column: "Id_UserRelation");

            migrationBuilder.CreateIndex(
                name: "UserToUser_NoDuplicated",
                table: "UserAssoisiatedUserMap",
                columns: new[] { "Id_ParentUser", "Id_ChildUser", "Id_UserRelation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDocumentCache_Id_User",
                table: "UserDocumentCache",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_UserQuota_Id_AppUser",
                table: "UserQuota",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMap_Id_Role",
                table: "UserRoleMap",
                column: "Id_Role");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMap_Id_User",
                table: "UserRoleMap",
                column: "Id_User");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkload_Id_AppUser",
                table: "UserWorkload",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkload_Id_Organisation",
                table: "UserWorkload",
                column: "Id_Organisation");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkload_Id_Project",
                table: "UserWorkload",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheet_Id_Creator",
                table: "Worksheet",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheet_Id_CurrentStatus",
                table: "Worksheet",
                column: "Id_CurrentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheet_Id_Project",
                table: "Worksheet",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheet_Id_WorksheetWorkflow",
                table: "Worksheet",
                column: "Id_WorksheetWorkflow");

            migrationBuilder.CreateIndex(
                name: "IX_Worksheet_Id_WorksheetWorkflowDataMap",
                table: "Worksheet",
                column: "Id_WorksheetWorkflowDataMap");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetAssert_Id_AppUser",
                table: "WorksheetAssert",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetAssert_Id_Project",
                table: "WorksheetAssert",
                column: "Id_Project");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetAssert_Id_Worksheet",
                table: "WorksheetAssert",
                column: "Id_Worksheet");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetAssertsFilesMap_Id_StorageEntry",
                table: "WorksheetAssertsFilesMap",
                column: "Id_StorageEntry");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetAssertsFilesMap_Id_WorksheetAssert",
                table: "WorksheetAssertsFilesMap",
                column: "Id_WorksheetAssert");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetGeoFence_Id_AppUser",
                table: "WorksheetGeoFence",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetGeoFenceLocation_Id_WorksheetGeoFence",
                table: "WorksheetGeoFenceLocation",
                column: "Id_WorksheetGeoFence");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetGeoFenceWiFi_Id_WorksheetGeoFence",
                table: "WorksheetGeoFenceWiFi",
                column: "Id_WorksheetGeoFence");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItem_Id_Creator",
                table: "WorksheetItem",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItem_Id_ProjectItemRate",
                table: "WorksheetItem",
                column: "Id_ProjectItemRate");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItem_Id_Worksheet",
                table: "WorksheetItem",
                column: "Id_Worksheet");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItemStatus_Id_Creator",
                table: "WorksheetItemStatus",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItemStatus_Id_Worksheet",
                table: "WorksheetItemStatus",
                column: "Id_Worksheet");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItemStatus_Id_WorksheetItemStatusLookup",
                table: "WorksheetItemStatus",
                column: "Id_WorksheetItemStatusLookup");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItemStatus_WorksheetItemId",
                table: "WorksheetItemStatus",
                column: "WorksheetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetItemStatusLookup_Id_AppUser",
                table: "WorksheetItemStatusLookup",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusHistory_Id_ChangeUser",
                table: "WorksheetStatusHistory",
                column: "Id_ChangeUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusHistory_Id_PostState",
                table: "WorksheetStatusHistory",
                column: "Id_PostState");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusHistory_Id_PreState",
                table: "WorksheetStatusHistory",
                column: "Id_PreState");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusHistory_Id_Worksheet",
                table: "WorksheetStatusHistory",
                column: "Id_Worksheet");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusLookupMap_Id_FromStatus",
                table: "WorksheetStatusLookupMap",
                column: "Id_FromStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusLookupMap_Id_ToStatus",
                table: "WorksheetStatusLookupMap",
                column: "Id_ToStatus");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetStatusLookupMap_Id_Workflow",
                table: "WorksheetStatusLookupMap",
                column: "Id_Workflow");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetTrack_Id_AppUser",
                table: "WorksheetTrack",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetTrack_Id_ProjectItemRate",
                table: "WorksheetTrack",
                column: "Id_ProjectItemRate");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetTrack_Id_Worksheet",
                table: "WorksheetTrack",
                column: "Id_Worksheet");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflow_Id_DefaultStep",
                table: "WorksheetWorkflow",
                column: "Id_DefaultStep");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflow_WorksheetStatusLookupId",
                table: "WorksheetWorkflow",
                column: "WorksheetStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowData_Id_WorksheetWorkflowMap",
                table: "WorksheetWorkflowData",
                column: "Id_WorksheetWorkflowMap");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowDataMap_Id_Creator",
                table: "WorksheetWorkflowDataMap",
                column: "Id_Creator");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowDataMap_Id_SharedWithOrganisation",
                table: "WorksheetWorkflowDataMap",
                column: "Id_SharedWithOrganisation");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowDataMap_Id_WorksheetWorkflow",
                table: "WorksheetWorkflowDataMap",
                column: "Id_WorksheetWorkflow");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowStorageMap_Id_AppUser",
                table: "WorksheetWorkflowStorageMap",
                column: "Id_AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowStorageMap_Id_StorageEntry",
                table: "WorksheetWorkflowStorageMap",
                column: "Id_StorageEntry");

            migrationBuilder.CreateIndex(
                name: "IX_WorksheetWorkflowStorageMap_Id_WorksheetStatusHistory",
                table: "WorksheetWorkflowStorageMap",
                column: "Id_WorksheetStatusHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Organisation",
                table: "Address",
                column: "Id_Organisation",
                principalTable: "Organisation",
                principalColumn: "Organisation_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_User",
                table: "Address",
                column: "Id_AppUser",
                principalTable: "AppUser",
                principalColumn: "AppUser_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_OvertimeAccount_Project",
                table: "OvertimeAccount",
                column: "Id_Project",
                principalTable: "Project",
                principalColumn: "Project_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AppUser_ProjectItemRate",
                table: "Project",
                column: "Id_DefaultRate",
                principalTable: "ProjectItemRate",
                principalColumn: "ProjectItemRate_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Organisation",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Organisation",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_WorksheetWorkflowDataMap_Organisation",
                table: "WorksheetWorkflowDataMap");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_User",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentInfo_AppUser",
                table: "PaymentInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_Creator",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItemRate_AppUser",
                table: "ProjectItemRate");

            migrationBuilder.DropForeignKey(
                name: "FK_WorksheetWorkflowDataMap_AppUser",
                table: "WorksheetWorkflowDataMap");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItemRate_Project",
                table: "ProjectItemRate");

            migrationBuilder.DropTable(
                name: "AppLoggerLog");

            migrationBuilder.DropTable(
                name: "AppNumberRange");

            migrationBuilder.DropTable(
                name: "ClientSturctureRight");

            migrationBuilder.DropTable(
                name: "CMSContent");

            migrationBuilder.DropTable(
                name: "ContactEntry");

            migrationBuilder.DropTable(
                name: "DashboardPlugin");

            migrationBuilder.DropTable(
                name: "HostedStorageBlob");

            migrationBuilder.DropTable(
                name: "LicenceEntry");

            migrationBuilder.DropTable(
                name: "LoginToken");

            migrationBuilder.DropTable(
                name: "MailAccountUserMap");

            migrationBuilder.DropTable(
                name: "MailBlacklist");

            migrationBuilder.DropTable(
                name: "MailSend");

            migrationBuilder.DropTable(
                name: "Maintainace");

            migrationBuilder.DropTable(
                name: "MustachioTemplateFormatter");

            migrationBuilder.DropTable(
                name: "NEngineRunningTask");

            migrationBuilder.DropTable(
                name: "OrganisationUserMap");

            migrationBuilder.DropTable(
                name: "OutgoingWebhookActionLog");

            migrationBuilder.DropTable(
                name: "OvertimeTransaction");

            migrationBuilder.DropTable(
                name: "PaymentInfoContent");

            migrationBuilder.DropTable(
                name: "PaymentProviderForRegionMap");

            migrationBuilder.DropTable(
                name: "PriorityQueueItem");

            migrationBuilder.DropTable(
                name: "ProcessorCapability");

            migrationBuilder.DropTable(
                name: "ProjectAddressMap");

            migrationBuilder.DropTable(
                name: "ProjectAssosiatedUserMap");

            migrationBuilder.DropTable(
                name: "ProjectBudget");

            migrationBuilder.DropTable(
                name: "ProjectShareKey");

            migrationBuilder.DropTable(
                name: "PromisedFeatureToAppUserMap");

            migrationBuilder.DropTable(
                name: "RemoteStorage");

            migrationBuilder.DropTable(
                name: "ReportingDataSource");

            migrationBuilder.DropTable(
                name: "SettingsValue");

            migrationBuilder.DropTable(
                name: "StorageProviderData");

            migrationBuilder.DropTable(
                name: "TextResource");

            migrationBuilder.DropTable(
                name: "UserAction");

            migrationBuilder.DropTable(
                name: "UserActivity");

            migrationBuilder.DropTable(
                name: "UserAssoisiatedUserMap");

            migrationBuilder.DropTable(
                name: "UserDocumentCache");

            migrationBuilder.DropTable(
                name: "UserQuota");

            migrationBuilder.DropTable(
                name: "UserRoleMap");

            migrationBuilder.DropTable(
                name: "UserWorkload");

            migrationBuilder.DropTable(
                name: "WorksheetAssertsFilesMap");

            migrationBuilder.DropTable(
                name: "WorksheetGeoFenceLocation");

            migrationBuilder.DropTable(
                name: "WorksheetGeoFenceWiFi");

            migrationBuilder.DropTable(
                name: "WorksheetItemStatus");

            migrationBuilder.DropTable(
                name: "WorksheetStatusLookupMap");

            migrationBuilder.DropTable(
                name: "WorksheetTrack");

            migrationBuilder.DropTable(
                name: "WorksheetWorkflowData");

            migrationBuilder.DropTable(
                name: "WorksheetWorkflowStorageMap");

            migrationBuilder.DropTable(
                name: "ClientStructure");

            migrationBuilder.DropTable(
                name: "LicenceGroup");

            migrationBuilder.DropTable(
                name: "MailAccount");

            migrationBuilder.DropTable(
                name: "NEngineTemplate");

            migrationBuilder.DropTable(
                name: "OrganisationRoleLookup");

            migrationBuilder.DropTable(
                name: "OutgoingWebhook");

            migrationBuilder.DropTable(
                name: "OvertimeAccount");

            migrationBuilder.DropTable(
                name: "Processor");

            migrationBuilder.DropTable(
                name: "ProjectAddressMapLookup");

            migrationBuilder.DropTable(
                name: "PaymentOrder");

            migrationBuilder.DropTable(
                name: "SettingsGroup");

            migrationBuilder.DropTable(
                name: "AssosiationInvitation");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "WorksheetAssert");

            migrationBuilder.DropTable(
                name: "WorksheetGeoFence");

            migrationBuilder.DropTable(
                name: "WorksheetItemStatusLookup");

            migrationBuilder.DropTable(
                name: "WorksheetItem");

            migrationBuilder.DropTable(
                name: "StorageEntry");

            migrationBuilder.DropTable(
                name: "WorksheetStatusHistory");

            migrationBuilder.DropTable(
                name: "OutgoingWebhookCase");

            migrationBuilder.DropTable(
                name: "Realm");

            migrationBuilder.DropTable(
                name: "PaymentProvider");

            migrationBuilder.DropTable(
                name: "PromisedFeatureContent");

            migrationBuilder.DropTable(
                name: "UserAssosiatedRoleLookup");

            migrationBuilder.DropTable(
                name: "StorageProvider");

            migrationBuilder.DropTable(
                name: "StorageType");

            migrationBuilder.DropTable(
                name: "Worksheet");

            migrationBuilder.DropTable(
                name: "PromisedFeature");

            migrationBuilder.DropTable(
                name: "Organisation");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "PromisedFeatureRegion");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "ProjectItemRate");

            migrationBuilder.DropTable(
                name: "BillingFrameLookup");

            migrationBuilder.DropTable(
                name: "PaymentInfo");

            migrationBuilder.DropTable(
                name: "WorksheetWorkflowDataMap");

            migrationBuilder.DropTable(
                name: "ProjectChargeRate");

            migrationBuilder.DropTable(
                name: "WorksheetWorkflow");

            migrationBuilder.DropTable(
                name: "WorksheetStatusLookup");
        }
    }
}
