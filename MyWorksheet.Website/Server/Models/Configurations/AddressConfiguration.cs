using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyWorksheet.Website.Server.Models.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> entity)
    {
        entity.HasKey(e => e.AddressId).HasName("PK__tmp_ms_x__03BDEBBAF4BCE223");

        entity.ToTable("Address");

        entity.Property(e => e.AddressId).HasColumnName("Address_Id");
        entity.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.CompanyName).HasMaxLength(600);
        entity.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.DateOfCreation).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.EmailAddress)
            .HasMaxLength(250)
            .HasColumnName("EMailAddress");
        entity.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdOrganisation).HasColumnName("Id_Organisation");
        entity.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.Phone)
            .IsRequired()
            .HasMaxLength(30);
        entity.Property(e => e.Street)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.StreetNo).HasMaxLength(150);
        entity.Property(e => e.ZipCode)
            .IsRequired()
            .HasMaxLength(15);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.Addresses)
            .HasForeignKey(d => d.IdAppUser)
            .HasConstraintName("FK_Address_User");

        entity.HasOne(d => d.IdOrganisationNavigation).WithMany(p => p.Addresses)
            .HasForeignKey(d => d.IdOrganisation)
            .HasConstraintName("FK_Address_Organisation");
    }
}

public class AppLoggerLogConfiguration : IEntityTypeConfiguration<AppLoggerLog>
{
    public void Configure(EntityTypeBuilder<AppLoggerLog> entity)
    {
        entity.HasKey(e => e.AppLoggerLogId).HasName("PK__AppLogge__63EB1FEC99B56109");

        entity.ToTable("AppLoggerLog");

        entity.Property(e => e.AppLoggerLogId).HasColumnName("AppLoggerLog_Id");
        entity.Property(e => e.Category).IsRequired();
        entity.Property(e => e.Level).IsRequired();
        entity.Property(e => e.Message).IsRequired();
    }
}
// public class AppMigrationConfiguration : IEntityTypeConfiguration<AppMigration>
// {
//     public void Configure(EntityTypeBuilder<AppMigration> entity)
//     {
//         entity.HasKey(e => e.AppMigrationId).HasName("PK__AppMigra__F75DD0D3C96939A9");

//         entity.ToTable("AppMigration");

//         entity.Property(e => e.AppMigrationId).HasColumnName("AppMigration_Id");
//         entity.Property(e => e.Name).IsRequired();
//     }
// }
public class AppNumberRangeConfiguration : IEntityTypeConfiguration<AppNumberRange>
{
    public void Configure(EntityTypeBuilder<AppNumberRange> entity)
    {
        entity.HasKey(e => e.AppNumberRangeId).HasName("PK__AppNumbe__BE7026E5F9C438C6");

        entity.ToTable("AppNumberRange");

        entity.Property(e => e.AppNumberRangeId).HasColumnName("AppNumberRange_Id");
        entity.Property(e => e.Code).IsRequired();
        entity.Property(e => e.IdUser).HasColumnName("Id_User");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.Template).IsRequired();

        entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.AppNumberRanges)
            .HasForeignKey(d => d.IdUser)
            .HasConstraintName("FK_AppNumberRange_AppUser");
    }
}
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.HasKey(e => e.AppUserId).HasName("PK__tmp_ms_x__FD254C17FA38945C");

        entity.ToTable("AppUser");

        entity.HasIndex(e => e.Username, "UQ__tmp_ms_x__536C85E47A31F292").IsUnique();

        entity.Property(e => e.AppUserId).HasColumnName("AppUser_ID");
        entity.Property(e => e.AllowFeatureRedeeming).HasDefaultValue(true);
        entity.Property(e => e.ContactName).HasMaxLength(250);
        entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
        entity.Property(e => e.IdCountry).HasColumnName("Id_Country");
        entity.Property(e => e.IsAktive).HasDefaultValue(true);
        entity.Property(e => e.NeedPasswordReset).HasDefaultValue(true);
        entity.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(64);
        entity.Property(e => e.RowState)
            .IsRequired()
            .IsConcurrencyToken()
            .ValueGeneratedNever();
        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(56);

        entity.HasOne(d => d.IdAddressNavigation).WithMany(p => p.AppUsers)
            .HasForeignKey(d => d.IdAddress)
            .HasConstraintName("FK_Users_Address");

        entity.HasOne(d => d.IdCountryNavigation).WithMany(p => p.AppUsers)
            .HasForeignKey(d => d.IdCountry)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Country");
    }
}
public class AssosiationInvitationConfiguration : IEntityTypeConfiguration<AssosiationInvitation>
{
    public void Configure(EntityTypeBuilder<AssosiationInvitation> entity)
    {
        entity.HasKey(e => e.AssosiationInvitationId).HasName("PK__tmp_ms_x__6ABA25EFAB3F095E");

        entity.ToTable("AssosiationInvitation");

        entity.Property(e => e.AssosiationInvitationId).HasColumnName("AssosiationInvitation_Id");
        entity.Property(e => e.ExternalId).IsRequired();
        entity.Property(e => e.IdRequestingUser).HasColumnName("Id_RequestingUser");
        entity.Property(e => e.IdUserAssosiatedRoleLookup).HasColumnName("Id_UserAssosiatedRoleLookup");
        entity.Property(e => e.RevokeReason).HasMaxLength(350);

        entity.HasOne(d => d.IdRequestingUserNavigation).WithMany(p => p.AssosiationInvitations)
            .HasForeignKey(d => d.IdRequestingUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_AssosiationInvitation_AppUser");

        entity.HasOne(d => d.IdUserAssosiatedRoleLookupNavigation).WithMany(p => p.AssosiationInvitations)
            .HasForeignKey(d => d.IdUserAssosiatedRoleLookup)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_AssosiationInvitation_UserAssosiatedRoleLookup");
    }
}

public class BillingFrameLookupConfiguration : IEntityTypeConfiguration<BillingFrameLookup>
{
    public void Configure(EntityTypeBuilder<BillingFrameLookup> entity)
    {
        entity.HasKey(e => e.BillingFrameLookupId).HasName("PK__BillingF__70B27A955F9709F1");

        entity.ToTable("BillingFrameLookup");

        entity.Property(e => e.BillingFrameLookupId).HasColumnName("BillingFrameLookup_Id");
        entity.Property(e => e.Code).IsRequired();
        entity.Property(e => e.DisplayKey).IsRequired();
    }
}
public class ClientStructureConfiguration : IEntityTypeConfiguration<ClientStructure>
{
    public void Configure(EntityTypeBuilder<ClientStructure> entity)
    {
        entity.HasKey(e => e.ClientStructureId).HasName("PK__ClientSt__A0DF204E2323EB4A");

        entity.ToTable("ClientStructure");

        entity.Property(e => e.ClientStructureId).HasColumnName("ClientStructure_Id");
        entity.Property(e => e.DisplayRoute).HasMaxLength(350);

        entity.HasOne(d => d.ParentRouteNavigation).WithMany(p => p.InverseParentRouteNavigation)
            .HasForeignKey(d => d.ParentRoute)
            .HasConstraintName("FK_ClientStructure_ClientStructure");
    }
}
public class ClientSturctureRightConfiguration : IEntityTypeConfiguration<ClientSturctureRight>
{
    public void Configure(EntityTypeBuilder<ClientSturctureRight> entity)
    {
        entity.HasKey(e => e.ClientSturctureRightId).HasName("PK__ClientSt__9E26344370E84D35");

        entity.ToTable("ClientSturctureRight");

        entity.Property(e => e.ClientSturctureRightId).HasColumnName("ClientSturctureRight_Id");
        entity.Property(e => e.IdClientStructure).HasColumnName("Id_ClientStructure");
        entity.Property(e => e.IdRole).HasColumnName("Id_Role");

        entity.HasOne(d => d.IdClientStructureNavigation).WithMany(p => p.ClientSturctureRights)
            .HasForeignKey(d => d.IdClientStructure)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ClientSturctureRight_ClientStructure");

        entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.ClientSturctureRights)
            .HasForeignKey(d => d.IdRole)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ClientSturctureRight_Role");
    }
}
public class CmscontentConfiguration : IEntityTypeConfiguration<Cmscontent>
{
    public void Configure(EntityTypeBuilder<Cmscontent> entity)
    {
        entity.HasKey(e => e.CmscontnetId).HasName("PK__CMSConte__CF54045F91475EAE");

        entity.ToTable("CMSContent");

        entity.Property(e => e.CmscontnetId).HasColumnName("CMSContnetID");
        entity.Property(e => e.ContentId)
            .HasMaxLength(200)
            .HasColumnName("Content_ID");
        entity.Property(e => e.ContentLang)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("DE")
            .HasColumnName("Content_Lang");
        entity.Property(e => e.ContentTemplate).HasColumnName("Content_Template");
        entity.Property(e => e.IsJsonblob).HasColumnName("IsJSONBlob");
    }
}
public class ContactEntryConfiguration : IEntityTypeConfiguration<ContactEntry>
{
    public void Configure(EntityTypeBuilder<ContactEntry> entity)
    {
        entity.HasKey(e => e.ContactEntryId).HasName("PK__ContactE__837C92F2C0FD6F2F");

        entity.ToTable("ContactEntry");

        entity.Property(e => e.ContactEntryId).HasColumnName("ContactEntry_ID");
        entity.Property(e => e.ContactType).HasMaxLength(50);
        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("EMail");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.SenderIp)
            .IsRequired()
            .HasMaxLength(15)
            .HasColumnName("SenderIP");
    }
}
public class DashboardPluginConfiguration : IEntityTypeConfiguration<DashboardPlugin>
{
    public void Configure(EntityTypeBuilder<DashboardPlugin> entity)
    {
        entity.HasKey(e => e.DashboardPluginId).HasName("PK__Dashboar__C4DB2EB59F8746C6");

        entity.ToTable("DashboardPlugin");

        entity.Property(e => e.DashboardPluginId).HasColumnName("DashboardPlugin_Id");
        entity.Property(e => e.ArgumentsQuery).HasMaxLength(1800);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
    }
}
public class LicenceEntryConfiguration : IEntityTypeConfiguration<LicenceEntry>
{
    public void Configure(EntityTypeBuilder<LicenceEntry> entity)
    {
        entity.HasKey(e => e.LicenceEntryId).HasName("PK__LicenceE__9A97FEC4779E583D");

        entity.ToTable("LicenceEntry");

        entity.Property(e => e.LicenceEntryId).HasColumnName("LicenceEntry_Id");
        entity.Property(e => e.Descriptor).IsRequired();
        entity.Property(e => e.IdLicenceGroup).HasColumnName("Id_LicenceGroup");
        entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.ProgramKey)
            .IsRequired()
            .HasDefaultValueSql("md5(random()::text || clock_timestamp()::text)::uuid::text");
        entity.Property(e => e.Username).IsRequired();

        entity.HasOne(d => d.IdLicenceGroupNavigation).WithMany(p => p.LicenceEntries)
            .HasForeignKey(d => d.IdLicenceGroup)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_LicenceEntry_LicenceGroup");
    }
}
public class LicenceGroupConfiguration : IEntityTypeConfiguration<LicenceGroup>
{
    public void Configure(EntityTypeBuilder<LicenceGroup> entity)
    {
        entity.HasKey(e => e.LicenceGroupId).HasName("PK__LicenceG__A1ED2FB4F63CF692");

        entity.ToTable("LicenceGroup");

        entity.Property(e => e.LicenceGroupId).HasColumnName("LicenceGroup_Id");
        entity.Property(e => e.Descriptor).IsRequired();
    }
}
public class LoginTokenConfiguration : IEntityTypeConfiguration<LoginToken>
{
    public void Configure(EntityTypeBuilder<LoginToken> entity)
    {
        entity.HasKey(e => e.LoginTokenId).HasName("PK__LoginTok__F559EA3AB1F090CD");

        entity.ToTable("LoginToken");

        entity.Property(e => e.LoginTokenId).HasColumnName("LoginToken_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.RemoteIp).HasMaxLength(20);
        entity.Property(e => e.Token).IsRequired();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.LoginTokens)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_LoginToken_AppUser");
    }
}


public class MailAccountConfiguration : IEntityTypeConfiguration<MailAccount>
{
    public void Configure(EntityTypeBuilder<MailAccount> entity)
    {
        entity.HasKey(e => e.MailAccountId).HasName("PK__MailAcco__5322B5A5B44D49B6");

        entity.ToTable("MailAccount");

        entity.Property(e => e.MailAccountId).HasColumnName("MailAccount_Id");
        entity.Property(e => e.EmailAddress)
            .IsRequired()
            .HasMaxLength(320)
            .HasColumnName("EMailAddress");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(255);
        entity.Property(e => e.ServerAddress).IsRequired();
        entity.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(255);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.MailAccounts)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailAccount_AppUser");
    }
}

public class MailAccountUserMapConfiguration : IEntityTypeConfiguration<MailAccountUserMap>
{
    public void Configure(EntityTypeBuilder<MailAccountUserMap> entity)
    {
        entity.HasKey(e => e.MailAccountUserMapId).HasName("PK__MailAcco__3C3D8FED53258EBE");

        entity.ToTable("MailAccountUserMap");

        entity.Property(e => e.MailAccountUserMapId).HasColumnName("MailAccountUserMap_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdMailAccount).HasColumnName("Id_MailAccount");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.MailAccountUserMaps)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailAccountUserMap_AppUser");

        entity.HasOne(d => d.IdMailAccountNavigation).WithMany(p => p.MailAccountUserMaps)
            .HasForeignKey(d => d.IdMailAccount)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailAccountUserMap_MailAccount");
    }
}

public class MailBlacklistConfiguration : IEntityTypeConfiguration<MailBlacklist>
{
    public void Configure(EntityTypeBuilder<MailBlacklist> entity)
    {
        entity.HasKey(e => e.MailBlacklistId).HasName("PK__MailBlac__CDF7559396AB43BF");

        entity.ToTable("MailBlacklist");

        entity.Property(e => e.MailBlacklistId).HasColumnName("MailBlacklist_Id");
        entity.Property(e => e.ClearName).HasMaxLength(600);
        entity.Property(e => e.X2hash)
            .IsRequired()
            .HasMaxLength(70)
            .HasColumnName("X2Hash");
    }
}
public class MailSendConfiguration : IEntityTypeConfiguration<MailSend>
{
    public void Configure(EntityTypeBuilder<MailSend> entity)
    {
        entity.HasKey(e => e.MailSendId).HasName("PK__tmp_ms_x__BA32164C420AC836");

        entity.ToTable("MailSend");

        entity.Property(e => e.MailSendId).HasColumnName("MailSend_Id");
        entity.Property(e => e.IdAttachment).HasColumnName("Id_Attachment");
        entity.Property(e => e.IdContent).HasColumnName("Id_Content");
        entity.Property(e => e.IdMailAccount).HasColumnName("Id_MailAccount");
        entity.Property(e => e.Recipients).IsRequired();

        entity.HasOne(d => d.IdAttachmentNavigation).WithMany(p => p.MailSendIdAttachmentNavigations)
            .HasForeignKey(d => d.IdAttachment)
            .HasConstraintName("FK_MailSend_Attachment");

        entity.HasOne(d => d.IdContentNavigation).WithMany(p => p.MailSendIdContentNavigations)
            .HasForeignKey(d => d.IdContent)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailSend_Content");

        entity.HasOne(d => d.IdMailAccountNavigation).WithMany(p => p.MailSends)
            .HasForeignKey(d => d.IdMailAccount)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MailSend_MailAccount");
    }
}
public class MaintainaceConfiguration : IEntityTypeConfiguration<Maintainace>
{
    public void Configure(EntityTypeBuilder<Maintainace> entity)
    {
        entity.HasKey(e => e.MaintainaceId).HasName("PK__Maintain__2625DEFA4D224C0B");

        entity.ToTable("Maintainace");

        entity.Property(e => e.MaintainaceId).HasColumnName("Maintainace_Id");
        entity.Property(e => e.CallerIp).IsRequired();
        entity.Property(e => e.CompiledView).IsRequired();
        entity.Property(e => e.Completed).HasDefaultValue(true);
        entity.Property(e => e.Reason).IsRequired();
    }
}
public class MustachioTemplateFormatterConfiguration : IEntityTypeConfiguration<MustachioTemplateFormatter>
{
    public void Configure(EntityTypeBuilder<MustachioTemplateFormatter> entity)
    {
        entity.HasKey(e => e.MustachioTemplateFormatterId).HasName("PK__Mustachi__10197881F21C7051");

        entity.ToTable("MustachioTemplateFormatter");

        entity.Property(e => e.MustachioTemplateFormatterId).HasColumnName("MustachioTemplateFormatter_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdStorageEntry).HasColumnName("Id_StorageEntry");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(350);

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.MustachioTemplateFormatters)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MustachioTemplateFormatter_AppUser");

        entity.HasOne(d => d.IdStorageEntryNavigation).WithMany(p => p.MustachioTemplateFormatters)
            .HasForeignKey(d => d.IdStorageEntry)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MustachioTemplateFormatter_StorageEntry");
    }
}
public class NengineRunningTaskConfiguration : IEntityTypeConfiguration<NengineRunningTask>
{
    public void Configure(EntityTypeBuilder<NengineRunningTask> entity)
    {
        entity.HasKey(e => e.NengineRunningTaskId).HasName("PK__tmp_ms_x__757D801D6DA41FE6");

        entity.ToTable("NEngineRunningTask");

        entity.Property(e => e.NengineRunningTaskId).HasColumnName("NEngineRunningTask_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdNengineTemplate).HasColumnName("Id_NEngineTemplate");
        entity.Property(e => e.IdProcessor).HasColumnName("Id_Processor");
        entity.Property(e => e.IdStoreageEntry).HasColumnName("Id_StoreageEntry");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.NengineRunningTasks)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_NEngineRunningTask_AppUser");

        entity.HasOne(d => d.IdNengineTemplateNavigation).WithMany(p => p.NengineRunningTasks)
            .HasForeignKey(d => d.IdNengineTemplate)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_NEngineRunningTask_NEngineTemplate");

        entity.HasOne(d => d.IdProcessorNavigation).WithMany(p => p.NengineRunningTasks)
            .HasForeignKey(d => d.IdProcessor)
            .HasConstraintName("FK_NEngineRunningTask_Processor");

        entity.HasOne(d => d.IdStoreageEntryNavigation).WithMany(p => p.NengineRunningTasks)
            .HasForeignKey(d => d.IdStoreageEntry)
            .HasConstraintName("FK_NEngineRunningTask_Storeage");
    }
}
public class NengineTemplateConfiguration : IEntityTypeConfiguration<NengineTemplate>
{
    public void Configure(EntityTypeBuilder<NengineTemplate> entity)
    {
        entity.HasKey(e => e.NengineTemplateId).HasName("PK__tmp_ms_x__32ECFC437F14191A");

        entity.ToTable("NEngineTemplate");

        entity.Property(e => e.NengineTemplateId).HasColumnName("NEngineTemplate_Id");
        entity.Property(e => e.FileExtention)
            .IsRequired()
            .HasMaxLength(255);
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.Purpose).HasMaxLength(200);
        entity.Property(e => e.Template).IsRequired();
        entity.Property(e => e.UsedDataSource)
            .IsRequired()
            .HasMaxLength(255);
        entity.Property(e => e.UsedFormattingEngine)
            .IsRequired()
            .HasMaxLength(255)
            .HasDefaultValue("Morestachio");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.NengineTemplates)
            .HasForeignKey(d => d.IdCreator)
            .HasConstraintName("FK_NEngineTemplate_AppUser");
    }
}
public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> entity)
    {
        entity.HasKey(e => e.OrganisationId).HasName("PK__tmp_ms_x__0BAD4DFC23CB7ED0");

        entity.ToTable("Organisation");

        entity.Property(e => e.OrganisationId).HasColumnName("Organisation_Id");
        entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
        entity.Property(e => e.IdParentOrganisation).HasColumnName("Id_ParentOrganisation");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(450);
        entity.Property(e => e.SharedId)
            .IsRequired()
            .HasMaxLength(25);

        entity.HasOne(d => d.IdAddressNavigation).WithMany(p => p.Organisations)
            .HasForeignKey(d => d.IdAddress)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Organisation_Address");

        entity.HasOne(d => d.IdParentOrganisationNavigation).WithMany(p => p.InverseIdParentOrganisationNavigation)
            .HasForeignKey(d => d.IdParentOrganisation)
            .HasConstraintName("FK_Organisation_OrganisationParent");
    }
}
public class OrganisationUserMapConfiguration : IEntityTypeConfiguration<OrganisationUserMap>
{
    public void Configure(EntityTypeBuilder<OrganisationUserMap> entity)
    {
        entity.HasKey(e => e.OrganisationUserMapId).HasName("PK__Organisa__D6FA8A88E4987241");

        entity.ToTable("OrganisationUserMap");

        entity.Property(e => e.OrganisationUserMapId).HasColumnName("OrganisationUserMap_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdOrganisation).HasColumnName("Id_Organisation");
        entity.Property(e => e.IdRelation).HasColumnName("Id_Relation");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.OrganisationUserMaps)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OrganisationUserMap_AppUser");

        entity.HasOne(d => d.IdOrganisationNavigation).WithMany(p => p.OrganisationUserMaps)
            .HasForeignKey(d => d.IdOrganisation)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OrganisationUserMap_Organisation");

        entity.HasOne(d => d.IdRelationNavigation).WithMany(p => p.OrganisationUserMaps)
            .HasForeignKey(d => d.IdRelation)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OrganisationUserMap_OrganisationRoleLookup");
    }
}
public class OutgoingWebhookActionLogConfiguration : IEntityTypeConfiguration<OutgoingWebhookActionLog>
{
    public void Configure(EntityTypeBuilder<OutgoingWebhookActionLog> entity)
    {
        entity.HasKey(e => e.OutgoingWebhookActionLogId).HasName("PK__tmp_ms_x__8CAA548A18FD584D");

        entity.ToTable("OutgoingWebhookActionLog");

        entity.Property(e => e.OutgoingWebhookActionLogId).HasColumnName("OutgoingWebhookActionLog_Id");
        entity.Property(e => e.DateOfAction).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdOutgoingWebhook).HasColumnName("Id_OutgoingWebhook");
        entity.Property(e => e.InitiatorIp)
            .IsRequired()
            .HasMaxLength(100);
        entity.Property(e => e.Success).HasDefaultValue(true);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.OutgoingWebhookActionLogs)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OutgoingWebhookActionLog_AppUser");

        entity.HasOne(d => d.IdOutgoingWebhookNavigation).WithMany(p => p.OutgoingWebhookActionLogs)
            .HasForeignKey(d => d.IdOutgoingWebhook)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OutgoingWebhookActionLog_OutgoingWebhook");
    }
}
public class OvertimeAccountConfiguration : IEntityTypeConfiguration<OvertimeAccount>
{
    public void Configure(EntityTypeBuilder<OvertimeAccount> entity)
    {
        entity.HasKey(e => e.OvertimeAccountId).HasName("PK__Overtime__A1EAD810CEF0D8B7");

        entity.ToTable("OvertimeAccount");

        entity.Property(e => e.OvertimeAccountId).HasColumnName("OvertimeAccount_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.OvertimeValue).HasColumnType("decimal(25, 8)");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.OvertimeAccounts)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OvertimeAccount_AppUser");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.OvertimeAccounts)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OvertimeAccount_Project");
    }
}
public class OvertimeTransactionConfiguration : IEntityTypeConfiguration<OvertimeTransaction>
{
    public void Configure(EntityTypeBuilder<OvertimeTransaction> entity)
    {
        entity.HasKey(e => e.OvertimeTransactionId).HasName("PK__Overtime__3A7C842763D76B2E");

        entity.ToTable("OvertimeTransaction");

        entity.Property(e => e.OvertimeTransactionId).HasColumnName("OvertimeTransaction_Id");
        entity.Property(e => e.DateOfAction).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.IdOvertimeAccount).HasColumnName("Id_OvertimeAccount");
        entity.Property(e => e.Value).HasColumnType("decimal(25, 8)");

        entity.HasOne(d => d.IdOvertimeAccountNavigation).WithMany(p => p.OvertimeTransactions)
            .HasForeignKey(d => d.IdOvertimeAccount)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_OvertimeTransaction_OvertimeAccount");
    }
}
public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
{
    public void Configure(EntityTypeBuilder<PaymentInfo> entity)
    {
        entity.HasKey(e => e.PaymentInfoId).HasName("PK__tmp_ms_x__CA91502AEBBA0D3C");

        entity.ToTable("PaymentInfo");

        entity.Property(e => e.PaymentInfoId).HasColumnName("PaymentInfo_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.PaymentDisclaimer).IsRequired();
        entity.Property(e => e.PaymentType)
            .IsRequired()
            .HasMaxLength(150);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.PaymentInfos)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaymentInfo_AppUser");
    }
}
public class PaymentInfoContentConfiguration : IEntityTypeConfiguration<PaymentInfoContent>
{
    public void Configure(EntityTypeBuilder<PaymentInfoContent> entity)
    {
        entity.HasKey(e => e.PaymentInfoContentId).HasName("PK__PaymentI__C36490BBA67A6C62");

        entity.ToTable("PaymentInfoContent");

        entity.Property(e => e.PaymentInfoContentId).HasColumnName("PaymentInfoContent_Id");
        entity.Property(e => e.FieldName)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.FieldValue)
            .IsRequired()
            .HasMaxLength(250);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdPaymentInfo).HasColumnName("Id_PaymentInfo");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.PaymentInfoContents)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaymentInfoContent_AppUser");

        entity.HasOne(d => d.IdPaymentInfoNavigation).WithMany(p => p.PaymentInfoContents)
            .HasForeignKey(d => d.IdPaymentInfo)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaymentInfoContent_PaymentInfo");
    }
}
public class PaymentOrderConfiguration : IEntityTypeConfiguration<PaymentOrder>
{
    public void Configure(EntityTypeBuilder<PaymentOrder> entity)
    {
        entity.HasKey(e => e.OrderId).HasName("PK__tmp_ms_x__F1E4607BB99445FF");

        entity.ToTable("PaymentOrder");

        entity.Property(e => e.OrderId).HasColumnName("Order_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdPaymentProvider).HasColumnName("Id_PaymentProvider");
        entity.Property(e => e.IdPromisedFeatureContent).HasColumnName("Id_PromisedFeatureContent");
        entity.Property(e => e.OrderCreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.PaymentOrders)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Order_Appuser");

        entity.HasOne(d => d.IdPaymentProviderNavigation).WithMany(p => p.PaymentOrders)
            .HasForeignKey(d => d.IdPaymentProvider)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Order_PaymentProvider");

        entity.HasOne(d => d.IdPromisedFeatureContentNavigation).WithMany(p => p.PaymentOrders)
            .HasForeignKey(d => d.IdPromisedFeatureContent)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Order_PromisedFeatureContent");
    }
}
public class PaymentProviderConfiguration : IEntityTypeConfiguration<PaymentProvider>
{
    public void Configure(EntityTypeBuilder<PaymentProvider> entity)
    {
        entity.HasKey(e => e.PaymentProviderId).HasName("PK__PaymentP__97C6F421B69212DE");

        entity.ToTable("PaymentProvider");

        entity.Property(e => e.PaymentProviderId).HasColumnName("PaymentProvider_Id");
    }
}
public class PaymentProviderForRegionMapConfiguration : IEntityTypeConfiguration<PaymentProviderForRegionMap>
{
    public void Configure(EntityTypeBuilder<PaymentProviderForRegionMap> entity)
    {
        entity.HasKey(e => e.PaymentProviderForRegionMapId).HasName("PK__PaymentP__A9D4368BAA30115F");

        entity.ToTable("PaymentProviderForRegionMap");

        entity.Property(e => e.PaymentProviderForRegionMapId).HasColumnName("PaymentProviderForRegionMap_Id");
        entity.Property(e => e.IdPaymentProvider).HasColumnName("Id_PaymentProvider");
        entity.Property(e => e.RegionId).HasColumnName("Region_Id");

        entity.HasOne(d => d.IdPaymentProviderNavigation).WithMany(p => p.PaymentProviderForRegionMaps)
            .HasForeignKey(d => d.IdPaymentProvider)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaymentProviderForRegionMap_PaymentProvider");

        entity.HasOne(d => d.Region).WithMany(p => p.PaymentProviderForRegionMaps)
            .HasForeignKey(d => d.RegionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaymentProviderForRegionMap_Region");
    }
}
public class PriorityQueueItemConfiguration : IEntityTypeConfiguration<PriorityQueueItem>
{
    public void Configure(EntityTypeBuilder<PriorityQueueItem> entity)
    {
        entity.HasKey(e => e.PriorityQueueItemId).HasName("PK__Priority__88D7B9C7E30554FF");

        entity.ToTable("PriorityQueueItem");

        entity.Property(e => e.PriorityQueueItemId).HasColumnName("PriorityQueueItem_Id");
        entity.Property(e => e.ActionKey)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.DataArguments)
            .IsRequired()
            .HasColumnType("xml");
        entity.Property(e => e.DateOfCreation).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdParent)
            .HasDefaultValueSql("(NULL)")
            .HasColumnName("Id_Parent");
        entity.Property(e => e.Level)
            .IsRequired()
            .HasMaxLength(100);
        entity.Property(e => e.Version).HasMaxLength(100);

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.PriorityQueueItems)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PriorityQueueItem_AppUser");

        entity.HasOne(d => d.IdParentNavigation).WithMany(p => p.InverseIdParentNavigation)
            .HasForeignKey(d => d.IdParent)
            .HasConstraintName("FK_PriorityQueueItem_PriorityQueueItem");
    }
}
public class ProcessorConfiguration : IEntityTypeConfiguration<Processor>
{
    public void Configure(EntityTypeBuilder<Processor> entity)
    {
        entity.HasKey(e => e.ProcessorId).HasName("PK__tmp_ms_x__8B9681ECB719FA44");

        entity.ToTable("Processor");

        entity.Property(e => e.ProcessorId).HasColumnName("Processor_Id");
        entity.Property(e => e.AuthKey).IsRequired();
        entity.Property(e => e.ExternalIdentity).IsRequired();
        entity.Property(e => e.IdRealm).HasColumnName("Id_Realm");
        entity.Property(e => e.IpOrHostname).IsRequired();
        entity.Property(e => e.Role).IsRequired();

        entity.HasOne(d => d.IdRealmNavigation).WithMany(p => p.Processors)
            .HasForeignKey(d => d.IdRealm)
            .HasConstraintName("FK_Processor_Realm");
    }
}
public class ProcessorCapabilityConfiguration : IEntityTypeConfiguration<ProcessorCapability>
{
    public void Configure(EntityTypeBuilder<ProcessorCapability> entity)
    {
        entity.HasKey(e => e.ProcessorCapabilityId).HasName("PK__Processo__34B57F17D2CC7C01");

        entity.ToTable("ProcessorCapability");

        entity.Property(e => e.ProcessorCapabilityId).HasColumnName("ProcessorCapability_Id");
        entity.Property(e => e.IdProcessor).HasColumnName("Id_Processor");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.Value).IsRequired();

        entity.HasOne(d => d.IdProcessorNavigation).WithMany(p => p.ProcessorCapabilities)
            .HasForeignKey(d => d.IdProcessor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProcessorCapability_Processor");
    }
}
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
    {
        entity.HasKey(e => e.ProjectId).HasName("PK__tmp_ms_x__1CB92E03305327D1");

        entity.ToTable("Project");

        entity.Property(e => e.ProjectId).HasColumnName("Project_Id");
        entity.Property(e => e.IdBillingFrame).HasColumnName("Id_BillingFrame");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdDefaultRate).HasColumnName("Id_DefaultRate");
        entity.Property(e => e.IdOrganisation).HasColumnName("Id_Organisation");
        entity.Property(e => e.IdPaymentCondition).HasColumnName("Id_PaymentCondition");
        entity.Property(e => e.IdWorksheetWorkflow).HasColumnName("Id_WorksheetWorkflow");
        entity.Property(e => e.IdWorksheetWorkflowDataMap).HasColumnName("Id_WorksheetWorkflowDataMap");
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.NumberRangeEntry).HasMaxLength(400);
        entity.Property(e => e.UserOrderNo).HasMaxLength(50);

        entity.HasOne(d => d.IdBillingFrameNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdBillingFrame)
            .HasConstraintName("FK_Project_BillingFrame");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Project_AppUser_Creator");

        entity.HasOne(d => d.IdDefaultRateNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdDefaultRate)
            .HasConstraintName("FK_Project_AppUser_ProjectItemRate");

        entity.HasOne(d => d.IdOrganisationNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdOrganisation)
            .HasConstraintName("FK_Project_Organisation");

        entity.HasOne(d => d.IdPaymentConditionNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdPaymentCondition)
            .HasConstraintName("FK_Project_PaymentCondition");

        entity.HasOne(d => d.IdWorksheetWorkflowNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdWorksheetWorkflow)
            .HasConstraintName("FK_Project_WorksheetWorkflow");

        entity.HasOne(d => d.IdWorksheetWorkflowDataMapNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.IdWorksheetWorkflowDataMap)
            .HasConstraintName("FK_Project_WorksheetWorkflowData");
    }
}
public class ProjectAddressMapConfiguration : IEntityTypeConfiguration<ProjectAddressMap>
{
    public void Configure(EntityTypeBuilder<ProjectAddressMap> entity)
    {
        entity.HasKey(e => e.ProjectAddressMapId).HasName("PK__ProjectA__011D5FB2C2E8ED75");

        entity.ToTable("ProjectAddressMap");

        entity.Property(e => e.ProjectAddressMapId).HasColumnName("ProjectAddressMap_Id");
        entity.Property(e => e.IdAddress).HasColumnName("Id_Address");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdProjectAddressMapLookup).HasColumnName("Id_ProjectAddressMapLookup");

        entity.HasOne(d => d.IdAddressNavigation).WithMany(p => p.ProjectAddressMaps)
            .HasForeignKey(d => d.IdAddress)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAddressMap_Address");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.ProjectAddressMaps)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAddressMap_Project");

        entity.HasOne(d => d.IdProjectAddressMapLookupNavigation).WithMany(p => p.ProjectAddressMaps)
            .HasForeignKey(d => d.IdProjectAddressMapLookup)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAddressMap_ProjectAddressMapLookup");
    }
}
public class ProjectAddressMapLookupConfiguration : IEntityTypeConfiguration<ProjectAddressMapLookup>
{
    public void Configure(EntityTypeBuilder<ProjectAddressMapLookup> entity)
    {
        entity.HasKey(e => e.ProjectAddressMapLookupId).HasName("PK__ProjectA__DA4D65583947D8B1");

        entity.ToTable("ProjectAddressMapLookup");

        entity.Property(e => e.ProjectAddressMapLookupId).HasColumnName("ProjectAddressMapLookup_Id");
        entity.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(250);
        entity.Property(e => e.DescriptionKey)
            .IsRequired()
            .HasMaxLength(350);
        entity.Property(e => e.DisplayKey)
            .IsRequired()
            .HasMaxLength(250);
    }
}
public class ProjectAssosiatedUserMapConfiguration : IEntityTypeConfiguration<ProjectAssosiatedUserMap>
{
    public void Configure(EntityTypeBuilder<ProjectAssosiatedUserMap> entity)
    {
        entity.HasKey(e => e.ProjectAssosiatedUserId).HasName("PK__ProjectA__9E0770E318EB069E");

        entity.ToTable("ProjectAssosiatedUserMap");

        entity.HasIndex(e => new { e.IdAppUser, e.IdProject }, "ProjectToUserMap_NoDuplicated").IsUnique();

        entity.Property(e => e.ProjectAssosiatedUserId).HasColumnName("ProjectAssosiatedUser_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.ProjectAssosiatedUserMaps)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssosiatedUserMap_AppUser");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.ProjectAssosiatedUserMaps)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssosiatedUserMap_Project");
    }
}
public class ProjectBudgetConfiguration : IEntityTypeConfiguration<ProjectBudget>
{
    public void Configure(EntityTypeBuilder<ProjectBudget> entity)
    {
        entity.HasKey(e => e.ProjectBudgetId).HasName("PK__tmp_ms_x__F93118CCEE442FF9");

        entity.ToTable("ProjectBudget");

        entity.Property(e => e.ProjectBudgetId).HasColumnName("ProjectBudget_Id");
        entity.Property(e => e.BugetConsumed).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.RowVersion)
            .IsRequired()
            .IsConcurrencyToken()
            .ValueGeneratedNever();
        entity.Property(e => e.TimeConsumed).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.TotalBudget).HasColumnType("decimal(18, 0)");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.ProjectBudgets)
            .HasForeignKey(d => d.IdAppUser)
            .HasConstraintName("FK_ProjectBudget_AppUser");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.ProjectBudgets)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectBudget_Project");
    }
}
public class ProjectChargeRateConfiguration : IEntityTypeConfiguration<ProjectChargeRate>
{
    public void Configure(EntityTypeBuilder<ProjectChargeRate> entity)
    {
        entity.HasKey(e => e.ProjectChargeRateId).HasName("PK__ProjectC__318A3D87E517D337");

        entity.ToTable("ProjectChargeRate");

        entity.Property(e => e.ProjectChargeRateId).HasColumnName("ProjectChargeRate_Id");
        entity.Property(e => e.Code).IsRequired();
        entity.Property(e => e.DisplayKey).IsRequired();

        entity.HasData([
            new ProjectChargeRate(){Code = "PER_HOUR", DisplayKey = "ProjectChargeRate/PerHour", ProjectChargeRateId = new Guid("00000000-0000-0000-0009-000000000001")},
            new ProjectChargeRate(){Code = "PER_MINUTE", DisplayKey = "ProjectChargeRate/PerMinute", ProjectChargeRateId = new Guid("00000000-0000-0000-0009-000000000002")},
            new ProjectChargeRate(){Code = "PER_QUARTER_MINUTE", DisplayKey = "ProjectChargeRate/PerQuarterMinute", ProjectChargeRateId = new Guid("00000000-0000-0000-0009-000000000003")},
            new ProjectChargeRate(){Code = "PER_STARTED_HOUR", DisplayKey = "ProjectChargeRate/PerStartedHour", ProjectChargeRateId = new Guid("00000000-0000-0000-0009-000000000004")},
            new ProjectChargeRate(){Code = "PER_DAY", DisplayKey = "ProjectChargeRate/PerDay", ProjectChargeRateId = new Guid("00000000-0000-0000-0009-000000000005")},
        ]);
    }
}
public class ProjectItemRateConfiguration : IEntityTypeConfiguration<ProjectItemRate>
{
    public void Configure(EntityTypeBuilder<ProjectItemRate> entity)
    {
        entity.HasKey(e => e.ProjectItemRateId).HasName("PK__tmp_ms_x__1B007FB152A27875");

        entity.ToTable("ProjectItemRate");

        entity.Property(e => e.ProjectItemRateId).HasColumnName("ProjectItemRate_Id");
        entity.Property(e => e.CurrencyType)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("EUR");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdProjectChargeRate).HasColumnName("Id_ProjectChargeRate");
        entity.Property(e => e.Name).IsRequired();
        entity.Property(e => e.Rate).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 0)");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.ProjectItemRates)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectItemRate_AppUser");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.ProjectItemRates)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectItemRate_Project");

        entity.HasOne(d => d.IdProjectChargeRateNavigation).WithMany(p => p.ProjectItemRates)
            .HasForeignKey(d => d.IdProjectChargeRate)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectItemRate_ProjectChargeRate");
    }
}
public class ProjectShareKeyConfiguration : IEntityTypeConfiguration<ProjectShareKey>
{
    public void Configure(EntityTypeBuilder<ProjectShareKey> entity)
    {
        entity.HasKey(e => e.ProjectShareKeyId).HasName("PK__tmp_ms_x__EDCFEEAC0DE3C204");

        entity.ToTable("ProjectShareKey");

        entity.Property(e => e.ProjectShareKeyId).HasColumnName("ProjectShareKey_Id");
        entity.Property(e => e.Description).IsRequired();
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.Key).IsRequired();
        entity.Property(e => e.Name).IsRequired();

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.ProjectShareKeys)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectShareKey_Project");
    }
}
public class PromisedFeatureConfiguration : IEntityTypeConfiguration<PromisedFeature>
{
    public void Configure(EntityTypeBuilder<PromisedFeature> entity)
    {
        entity.HasKey(e => e.PromisedFeatureId).HasName("PK__tmp_ms_x__9288D982D9D4CC2D");

        entity.ToTable("PromisedFeature");

        entity.Property(e => e.PromisedFeatureId).HasColumnName("PromisedFeature_Id");
        entity.Property(e => e.DisplayKey)
            .IsRequired()
            .HasMaxLength(250);
        entity.Property(e => e.OrderNumber).HasDefaultValue(0);
    }
}
public class PromisedFeatureToAppUserMapConfiguration : IEntityTypeConfiguration<PromisedFeatureToAppUserMap>
{
    public void Configure(EntityTypeBuilder<PromisedFeatureToAppUserMap> entity)
    {
        entity.HasKey(e => e.PromisedFeatureToAppUserMapId).HasName("PK__tmp_ms_x__96FCDA194163B3E0");

        entity.ToTable("PromisedFeatureToAppUserMap");

        entity.Property(e => e.PromisedFeatureToAppUserMapId).HasColumnName("PromisedFeatureToAppUserMap_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdFeature).HasColumnName("Id_Feature");
        entity.Property(e => e.IdOrder).HasColumnName("Id_Order");
        entity.Property(e => e.ValidFrom).HasColumnType("timestamp with time zone");
        entity.Property(e => e.ValidUntil).HasColumnType("timestamp with time zone");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.PromisedFeatureToAppUserMaps)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PromisedFeatureToAppUserMap_AppUser");

        entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.PromisedFeatureToAppUserMaps)
            .HasForeignKey(d => d.IdOrder)
            .HasConstraintName("FK_PromisedFeatureToAppUserMap_Order");
    }
}
public class RealmConfiguration : IEntityTypeConfiguration<Realm>
{
    public void Configure(EntityTypeBuilder<Realm> entity)
    {
        entity.HasKey(e => e.RealmId).HasName("PK__Realm__AAE78C15B8122F4D");

        entity.ToTable("Realm");

        entity.Property(e => e.RealmId).HasColumnName("Realm_Id");
        entity.Property(e => e.Named).HasMaxLength(35);
    }
}
public class RemoteStorageConfiguration : IEntityTypeConfiguration<RemoteStorage>
{
    public void Configure(EntityTypeBuilder<RemoteStorage> entity)
    {
        entity.HasKey(e => e.RemoteStorageId).HasName("PK__RemoteSt__5E6829A4ECC895E3");

        entity.ToTable("RemoteStorage");

        entity.Property(e => e.RemoteStorageId).HasColumnName("RemoteStorage_Id");
        entity.Property(e => e.AccessKey).IsRequired();
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.Password).IsRequired();

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.RemoteStorages)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RemoteStorage_ToTable");
    }
}
public class ReportingDataSourceConfiguration : IEntityTypeConfiguration<ReportingDataSource>
{
    public void Configure(EntityTypeBuilder<ReportingDataSource> entity)
    {
        entity.HasKey(e => e.ReportingDataSourceId).HasName("PK__Reportin__42E76A8D52184CA9");

        entity.ToTable("ReportingDataSource");

        entity.Property(e => e.ReportingDataSourceId).HasColumnName("ReportingDataSource_Id");
    }
}
public class SettingsGroupConfiguration : IEntityTypeConfiguration<SettingsGroup>
{
    public void Configure(EntityTypeBuilder<SettingsGroup> entity)
    {
        entity.HasKey(e => e.SettingsGroupId).HasName("PK__tmp_ms_x__D1E0B49FB3A6A27A");

        entity.ToTable("SettingsGroup");

        entity.Property(e => e.SettingsGroupId).HasColumnName("SettingsGroup_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.Key).HasMaxLength(250);
        entity.Property(e => e.Name).HasMaxLength(250);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.SettingsGroups)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SettingsGroup_AppUser");
    }
}
public class SettingsValueConfiguration : IEntityTypeConfiguration<SettingsValue>
{
    public void Configure(EntityTypeBuilder<SettingsValue> entity)
    {
        entity.HasKey(e => e.SettingsValueId).HasName("PK__tmp_ms_x__930AD0B52472DBEA");

        entity.ToTable("SettingsValue");

        entity.Property(e => e.SettingsValueId).HasColumnName("SettingsValue_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdSettingsGroup).HasColumnName("Id_SettingsGroup");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(250);
        entity.Property(e => e.RowState)
            .IsRequired()
            .IsRowVersion()
            .IsConcurrencyToken();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.SettingsValues)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SettingsValue_AppUser");

        entity.HasOne(d => d.IdSettingsGroupNavigation).WithMany(p => p.SettingsValues)
            .HasForeignKey(d => d.IdSettingsGroup)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SettingsValue_SettingsGroup");
    }
}
public class StorageEntryConfiguration : IEntityTypeConfiguration<StorageEntry>
{
    public void Configure(EntityTypeBuilder<StorageEntry> entity)
    {
        entity.HasKey(e => e.StorageEntryId).HasName("PK__tmp_ms_x__881100EF220D283F");

        entity.ToTable("StorageEntry");

        entity.Property(e => e.StorageEntryId).HasColumnName("StorageEntry_Id");
        entity.Property(e => e.ContentType).HasMaxLength(30);
        entity.Property(e => e.FileName).HasMaxLength(255);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdStorageProvider).HasColumnName("Id_StorageProvider");
        entity.Property(e => e.IdStorageType).HasColumnName("Id_StorageType");
        entity.Property(e => e.StorageKey).IsRequired();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.StorageEntries)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_StorageEntry_AppUser");

        entity.HasOne(d => d.IdStorageProviderNavigation).WithMany(p => p.StorageEntries)
            .HasForeignKey(d => d.IdStorageProvider)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_StorageEntry_StorageProvider");

        entity.HasOne(d => d.IdStorageTypeNavigation).WithMany(p => p.StorageEntries)
            .HasForeignKey(d => d.IdStorageType)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_StorageEntry_StorageType");

        entity.HasOne(d => d.ThumbnailOfNavigation).WithMany(p => p.InverseThumbnailOfNavigation)
            .HasForeignKey(d => d.ThumbnailOf)
            .HasConstraintName("FK_StorageEntry_StorageEntry");
    }
}
public class StorageProviderDatumConfiguration : IEntityTypeConfiguration<StorageProviderData>
{
    public void Configure(EntityTypeBuilder<StorageProviderData> entity)
    {
        entity.HasKey(e => e.StorageProviderDataId).HasName("PK__tmp_ms_x__5084260F7572CBB8");

        entity.Property(e => e.StorageProviderDataId).HasColumnName("StorageProviderData_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdStorageProvider).HasColumnName("Id_StorageProvider");
        entity.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(300);
        entity.Property(e => e.Value).IsRequired();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.StorageProviderData)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_StorageProviderData_AppUser");

        entity.HasOne(d => d.IdStorageProviderNavigation).WithMany(p => p.StorageProviderData)
            .HasForeignKey(d => d.IdStorageProvider)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_StorageProviderData_StorageProvider");
    }
}
public class TextResourceConfiguration : IEntityTypeConfiguration<TextResource>
{
    public void Configure(EntityTypeBuilder<TextResource> entity)
    {
        entity.HasKey(e => e.IdTextResource).HasName("PK__TextReso__2E280A4EB48D1CF1");

        entity.ToTable("TextResource");

        entity.Property(e => e.IdTextResource).HasColumnName("Id_TextResource");
        entity.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(250);
        entity.Property(e => e.Lang)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("DE");
        entity.Property(e => e.Text).IsRequired();
    }
}
public class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
{
    public void Configure(EntityTypeBuilder<UserAction> entity)
    {
        entity.HasKey(e => e.UserActionId).HasName("PK__UserActi__3C1ED4D1224DB7C8");

        entity.ToTable("UserAction");

        entity.Property(e => e.UserActionId).HasColumnName("UserAction_Id");
        entity.Property(e => e.ActionIp)
            .IsRequired()
            .HasMaxLength(32)
            .HasColumnName("ActionIP");
        entity.Property(e => e.IdUser).HasColumnName("Id_User");
        entity.Property(e => e.PcName).IsRequired();
        entity.Property(e => e.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserActions)
            .HasForeignKey(d => d.IdUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserAction_User");
    }
}
public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> entity)
    {
        entity.HasKey(e => e.UserActivityId).HasName("PK__tmp_ms_x__A41D3D638E7F5914");

        entity.ToTable("UserActivity");

        entity.Property(e => e.UserActivityId).HasColumnName("UserActivity_Id");
        entity.Property(e => e.ActivityType)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.BodyHtml).IsRequired();
        entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.HeaderHtml)
            .IsRequired()
            .HasMaxLength(150);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.SystemActivityTypeKey).IsRequired();

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.UserActivities)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserActivity_AppUser");
    }
}
public class UserAssoisiatedUserMapConfiguration : IEntityTypeConfiguration<UserAssoisiatedUserMap>
{
    public void Configure(EntityTypeBuilder<UserAssoisiatedUserMap> entity)
    {
        entity.HasKey(e => e.UserAssoisiatedUserMapId).HasName("PK__UserAsso__100198A57A2BDA46");

        entity.ToTable("UserAssoisiatedUserMap");

        entity.HasIndex(e => new { e.IdParentUser, e.IdChildUser, e.IdUserRelation }, "UserToUser_NoDuplicated").IsUnique();

        entity.Property(e => e.UserAssoisiatedUserMapId).HasColumnName("UserAssoisiatedUserMap_Id");
        entity.Property(e => e.IdChildUser).HasColumnName("Id_ChildUser");
        entity.Property(e => e.IdInvite)
            .HasDefaultValueSql("(NULL)")
            .HasColumnName("Id_Invite");
        entity.Property(e => e.IdParentUser).HasColumnName("Id_ParentUser");
        entity.Property(e => e.IdUserRelation).HasColumnName("Id_UserRelation");

        entity.HasOne(d => d.IdChildUserNavigation).WithMany(p => p.UserAssoisiatedUserMapIdChildUserNavigations)
            .HasForeignKey(d => d.IdChildUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserAssoisiatedUserMap_Child_AppUser");

        entity.HasOne(d => d.IdInviteNavigation).WithMany(p => p.UserAssoisiatedUserMaps)
            .HasForeignKey(d => d.IdInvite)
            .HasConstraintName("FK_UserAssoisiatedUserMap_Invite");

        entity.HasOne(d => d.IdParentUserNavigation).WithMany(p => p.UserAssoisiatedUserMapIdParentUserNavigations)
            .HasForeignKey(d => d.IdParentUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserAssoisiatedUserMap_Parent_AppUser");

        entity.HasOne(d => d.IdUserRelationNavigation).WithMany(p => p.UserAssoisiatedUserMaps)
            .HasForeignKey(d => d.IdUserRelation)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserAssoisiatedUserMap_UserRelation");
    }
}
public class UserDocumentCacheConfiguration : IEntityTypeConfiguration<UserDocumentCache>
{
    public void Configure(EntityTypeBuilder<UserDocumentCache> entity)
    {
        entity.HasKey(e => e.UserDocumentCacheId).HasName("PK__UserDocu__DDFC5080D1E8002C");

        entity.ToTable("UserDocumentCache");

        entity.Property(e => e.UserDocumentCacheId).HasColumnName("UserDocumentCache_Id");
        entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.FileType).HasMaxLength(50);
        entity.Property(e => e.HostenOn).HasMaxLength(50);
        entity.Property(e => e.IdUser).HasColumnName("Id_User");
        entity.Property(e => e.Link)
            .IsRequired()
            .HasMaxLength(200);
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserDocumentCaches)
            .HasForeignKey(d => d.IdUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserDocumentCache_User");
    }
}
public class UserQuotumConfiguration : IEntityTypeConfiguration<UserQuota>
{
    public void Configure(EntityTypeBuilder<UserQuota> entity)
    {
        entity.HasKey(e => e.UserQuotaId).HasName("PK__tmp_ms_x__1928E770E6C54627");

        entity.Property(e => e.UserQuotaId).HasColumnName("UserQuota_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.UserQuota)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserScota_AppUser");
    }
}
public class UserRoleMapConfiguration : IEntityTypeConfiguration<UserRoleMap>
{
    public void Configure(EntityTypeBuilder<UserRoleMap> entity)
    {
        entity.HasKey(e => e.UserRoleMapId).HasName("PK__UserRole__5CE0326DE294FA89");

        entity.ToTable("UserRoleMap");

        entity.Property(e => e.UserRoleMapId).HasColumnName("UserRoleMap_Id");
        entity.Property(e => e.IdRole).HasColumnName("Id_Role");
        entity.Property(e => e.IdUser).HasColumnName("Id_User");

        entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.UserRoleMaps)
            .HasForeignKey(d => d.IdRole)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserRoleMap_Role");

        entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserRoleMaps)
            .HasForeignKey(d => d.IdUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserRoleMap_User");
    }
}
public class UserWorkloadConfiguration : IEntityTypeConfiguration<UserWorkload>
{
    public void Configure(EntityTypeBuilder<UserWorkload> entity)
    {
        entity.HasKey(e => e.UserWorkloadId).HasName("PK__UserWork__5240926C91EB3ED3");

        entity.ToTable("UserWorkload");

        entity.Property(e => e.UserWorkloadId).HasColumnName("UserWorkload_Id");
        entity.Property(e => e.DayWorkTimeFriday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeMonday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeSaturday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeSunday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeThursday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeTuesday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.DayWorkTimeWednesday).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdOrganisation).HasColumnName("Id_Organisation");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.MonthWorktime).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.WeekWorktime).HasColumnType("decimal(18, 0)");
        entity.Property(e => e.WorkTimeMode).HasDefaultValue(1);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.UserWorkloads)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserWorkload_AppUser");

        entity.HasOne(d => d.IdOrganisationNavigation).WithMany(p => p.UserWorkloads)
            .HasForeignKey(d => d.IdOrganisation)
            .HasConstraintName("FK_UserWorkload_Organisation");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.UserWorkloads)
            .HasForeignKey(d => d.IdProject)
            .HasConstraintName("FK_UserWorkload_Project");
    }
}
public class WorksheetConfiguration : IEntityTypeConfiguration<Worksheet>
{
    public void Configure(EntityTypeBuilder<Worksheet> entity)
    {
        entity.HasKey(e => e.WorksheetId).HasName("PK__tmp_ms_x__342F12B7AC1FF5A2");

        entity.ToTable("Worksheet");

        entity.Property(e => e.WorksheetId).HasColumnName("Worksheet_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdCurrentStatus)
            .HasColumnName("Id_CurrentStatus");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdWorksheetWorkflow).HasColumnName("Id_WorksheetWorkflow");
        entity.Property(e => e.IdWorksheetWorkflowDataMap).HasColumnName("Id_WorksheetWorkflowDataMap");
        entity.Property(e => e.No).HasMaxLength(100);
        entity.Property(e => e.NumberRangeEntry).HasMaxLength(400);

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.Worksheets)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Worksheet_AppUser_Creator");

        entity.HasOne(d => d.IdCurrentStatusNavigation).WithMany(p => p.Worksheets)
            .HasForeignKey(d => d.IdCurrentStatus)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Worksheet_Status");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.Worksheets)
            .HasForeignKey(d => d.IdProject)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Worksheet_Project");

        entity.HasOne(d => d.IdWorksheetWorkflowNavigation).WithMany(p => p.Worksheets)
            .HasForeignKey(d => d.IdWorksheetWorkflow)
            .HasConstraintName("FK_Worksheet_WorkflowMode");

        entity.HasOne(d => d.IdWorksheetWorkflowDataMapNavigation).WithMany(p => p.Worksheets)
            .HasForeignKey(d => d.IdWorksheetWorkflowDataMap)
            .HasConstraintName("FK_Worksheet_WorkflowMode_Data");
    }
}
public class WorksheetAssertConfiguration : IEntityTypeConfiguration<WorksheetAssert>
{
    public void Configure(EntityTypeBuilder<WorksheetAssert> entity)
    {
        entity.HasKey(e => e.WorksheetAssertId).HasName("PK__Workshee__DD325CE5FE0C0C79");

        entity.ToTable("WorksheetAssert");

        entity.Property(e => e.WorksheetAssertId).HasColumnName("WorksheetAssert_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(600);
        entity.Property(e => e.Tax).HasColumnType("decimal(25, 5)");
        entity.Property(e => e.Value).HasColumnType("decimal(25, 5)");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.WorksheetAsserts)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetAssert_AppUser");

        entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.WorksheetAsserts)
            .HasForeignKey(d => d.IdProject)
            .HasConstraintName("FK_WorksheetAssert_Project");

        entity.HasOne(d => d.IdWorksheetNavigation).WithMany(p => p.WorksheetAsserts)
            .HasForeignKey(d => d.IdWorksheet)
            .HasConstraintName("FK_WorksheetAssert_Worksheet");
    }
}
public class WorksheetAssertsFilesMapConfiguration : IEntityTypeConfiguration<WorksheetAssertsFilesMap>
{
    public void Configure(EntityTypeBuilder<WorksheetAssertsFilesMap> entity)
    {
        entity.HasKey(e => e.WorksheetAssertsFilesMapId).HasName("PK__Workshee__BE74C751C0D3E8C5");

        entity.ToTable("WorksheetAssertsFilesMap");

        entity.Property(e => e.WorksheetAssertsFilesMapId).HasColumnName("WorksheetAssertsFilesMap_Id");
        entity.Property(e => e.IdStorageEntry).HasColumnName("Id_StorageEntry");
        entity.Property(e => e.IdWorksheetAssert).HasColumnName("Id_WorksheetAssert");

        entity.HasOne(d => d.IdStorageEntryNavigation).WithMany(p => p.WorksheetAssertsFilesMaps)
            .HasForeignKey(d => d.IdStorageEntry)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetAssertFilesMap_StorageEntry");

        entity.HasOne(d => d.IdWorksheetAssertNavigation).WithMany(p => p.WorksheetAssertsFilesMaps)
            .HasForeignKey(d => d.IdWorksheetAssert)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetAssertFilesMap_WorksheetAssert");
    }
}
public class WorksheetGeoFenceConfiguration : IEntityTypeConfiguration<WorksheetGeoFence>
{
    public void Configure(EntityTypeBuilder<WorksheetGeoFence> entity)
    {
        entity.HasKey(e => e.WorksheetGeoFenceId).HasName("PK__Workshee__B06775C0721FE514");

        entity.ToTable("WorksheetGeoFence");

        entity.Property(e => e.WorksheetGeoFenceId).HasColumnName("WorksheetGeoFence_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProject).HasColumnName("Id_Project");
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(300);

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.WorksheetGeoFences)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetGeoFence_AppUser");
    }
}
public class WorksheetGeoFenceLocationConfiguration : IEntityTypeConfiguration<WorksheetGeoFenceLocation>
{
    public void Configure(EntityTypeBuilder<WorksheetGeoFenceLocation> entity)
    {
        entity.HasKey(e => e.WorksheetGeoFenceId).HasName("PK__tmp_ms_x__B06775C035ABBBFD");

        entity.ToTable("WorksheetGeoFenceLocation");

        entity.Property(e => e.WorksheetGeoFenceId).HasColumnName("WorksheetGeoFence_Id");
        entity.Property(e => e.FenceGroup).HasDefaultValue(1);
        entity.Property(e => e.IdWorksheetGeoFence).HasColumnName("Id_WorksheetGeoFence");
        entity.Property(e => e.Latitude).HasColumnType("decimal(10, 6)");
        entity.Property(e => e.Longitude).HasColumnType("decimal(10, 6)");

        entity.HasOne(d => d.IdWorksheetGeoFenceNavigation).WithMany(p => p.WorksheetGeoFenceLocations)
            .HasForeignKey(d => d.IdWorksheetGeoFence)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetGeoFenceLocation_WorksheetGeoFence");
    }
}
public class WorksheetGeoFenceWiFiConfiguration : IEntityTypeConfiguration<WorksheetGeoFenceWiFi>
{
    public void Configure(EntityTypeBuilder<WorksheetGeoFenceWiFi> entity)
    {
        entity.HasKey(e => e.WorksheetGeoFenceWiFiId).HasName("PK__Workshee__7A2BCB18EDF891B4");

        entity.ToTable("WorksheetGeoFenceWiFi");

        entity.Property(e => e.WorksheetGeoFenceWiFiId).HasColumnName("WorksheetGeoFenceWiFi_Id");
        entity.Property(e => e.IdWorksheetGeoFence).HasColumnName("Id_WorksheetGeoFence");

        entity.HasOne(d => d.IdWorksheetGeoFenceNavigation).WithMany(p => p.WorksheetGeoFenceWiFis)
            .HasForeignKey(d => d.IdWorksheetGeoFence)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetGeoFenceWiFi_WorksheetGeoFence");
    }
}
public class WorksheetItemConfiguration : IEntityTypeConfiguration<WorksheetItem>
{
    public void Configure(EntityTypeBuilder<WorksheetItem> entity)
    {
        entity.HasKey(e => e.WorksheetItemId).HasName("PK__tmp_ms_x__B5DF312B2D8164C7");

        entity.ToTable("WorksheetItem");

        entity.Property(e => e.WorksheetItemId).HasColumnName("WorksheetItem_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdProjectItemRate).HasColumnName("Id_ProjectItemRate");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.WorksheetItems)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItem_AppUser_Creator");

        entity.HasOne(d => d.IdProjectItemRateNavigation).WithMany(p => p.WorksheetItems)
            .HasForeignKey(d => d.IdProjectItemRate)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItem_ProjectItemRate");

        entity.HasOne(d => d.IdWorksheetNavigation).WithMany(p => p.WorksheetItems)
            .HasForeignKey(d => d.IdWorksheet)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItem_Worksheet");
    }
}
public class WorksheetItemStatusConfiguration : IEntityTypeConfiguration<WorksheetItemStatus>
{
    public void Configure(EntityTypeBuilder<WorksheetItemStatus> entity)
    {
        entity.HasKey(e => e.WorksheetItemStatusId).HasName("PK__tmp_ms_x__0A3540BAB40ADE3A");

        entity.ToTable("WorksheetItemStatus");

        entity.Property(e => e.WorksheetItemStatusId).HasColumnName("WorksheetItemStatus_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");
        entity.Property(e => e.IdWorksheetItemStatusLookup).HasColumnName("Id_WorksheetItemStatusLookup");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.WorksheetItemStatuses)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItemStatus_AppUser");

        entity.HasOne(d => d.IdWorksheetNavigation).WithMany(p => p.WorksheetItemStatuses)
            .HasForeignKey(d => d.IdWorksheet)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItemStatus_Worksheet");

        entity.HasOne(d => d.IdWorksheetItemStatusLookupNavigation).WithMany(p => p.WorksheetItemStatuses)
            .HasForeignKey(d => d.IdWorksheetItemStatusLookup)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetItemStatus_WorksheetItemStatusLookup");
    }
}
public class WorksheetItemStatusLookupConfiguration : IEntityTypeConfiguration<WorksheetItemStatusLookup>
{
    public void Configure(EntityTypeBuilder<WorksheetItemStatusLookup> entity)
    {
        entity.HasKey(e => e.WorksheetItemStatusLookupId).HasName("PK__Workshee__0E5846B077316D57");

        entity.ToTable("WorksheetItemStatusLookup");

        entity.Property(e => e.WorksheetItemStatusLookupId).HasColumnName("WorksheetItemStatusLookup_Id");
        entity.Property(e => e.Description).HasMaxLength(250);
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.WorksheetItemStatusLookups)
            .HasForeignKey(d => d.IdAppUser)
            .HasConstraintName("FK_WorksheetItemStatusLookup_AppUser");
    }
}
public class WorksheetStatusHistoryConfiguration : IEntityTypeConfiguration<WorksheetStatusHistory>
{
    public void Configure(EntityTypeBuilder<WorksheetStatusHistory> entity)
    {
        entity.HasKey(e => e.WorksheetStatusHistoryId).HasName("PK__tmp_ms_x__D8707A34A3F8A99F");

        entity.ToTable("WorksheetStatusHistory");

        entity.Property(e => e.WorksheetStatusHistoryId).HasColumnName("WorksheetStatusHistory_Id");
        entity.Property(e => e.DateOfAction).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(e => e.IdChangeUser).HasColumnName("Id_ChangeUser");
        entity.Property(e => e.IdPostState).HasColumnName("Id_PostState");
        entity.Property(e => e.IdPreState).HasColumnName("Id_PreState");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");

        entity.HasOne(d => d.IdChangeUserNavigation).WithMany(p => p.WorksheetStatusHistories)
            .HasForeignKey(d => d.IdChangeUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusHistory_AppUser");

        entity.HasOne(d => d.IdPostStateNavigation).WithMany(p => p.WorksheetStatusHistoryIdPostStateNavigations)
            .HasForeignKey(d => d.IdPostState)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusHistory_PostState_WorksheetStatus");

        entity.HasOne(d => d.IdPreStateNavigation).WithMany(p => p.WorksheetStatusHistoryIdPreStateNavigations)
            .HasForeignKey(d => d.IdPreState)
            .HasConstraintName("FK_WorksheetStatusHistory_PreState_WorksheetStatus");

        entity.HasOne(d => d.IdWorksheetNavigation).WithMany(p => p.WorksheetStatusHistories)
            .HasForeignKey(d => d.IdWorksheet)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetStatusHistory_Worksheet");
    }
}
public class WorksheetTrackConfiguration : IEntityTypeConfiguration<WorksheetTrack>
{
    public void Configure(EntityTypeBuilder<WorksheetTrack> entity)
    {
        entity.HasKey(e => e.WorksheetTrackId).HasName("PK__tmp_ms_x__0B2A1EB7938FC005");

        entity.ToTable("WorksheetTrack");

        entity.Property(e => e.WorksheetTrackId).HasColumnName("WorksheetTrack_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdProjectItemRate).HasColumnName("Id_ProjectItemRate");
        entity.Property(e => e.IdWorksheet).HasColumnName("Id_Worksheet");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.WorksheetTracks)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetTrack_AppUser");

        entity.HasOne(d => d.IdProjectItemRateNavigation).WithMany(p => p.WorksheetTracks)
            .HasForeignKey(d => d.IdProjectItemRate)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetTrack_ProjectItemRate");

        entity.HasOne(d => d.IdWorksheetNavigation).WithMany(p => p.WorksheetTracks)
            .HasForeignKey(d => d.IdWorksheet)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetTrack_Worksheet");
    }
}
public class WorksheetWorkflowDataMapConfiguration : IEntityTypeConfiguration<WorksheetWorkflowDataMap>
{
    public void Configure(EntityTypeBuilder<WorksheetWorkflowDataMap> entity)
    {
        entity.HasKey(e => e.WorksheetWorkflowDataMapId).HasName("PK__Workshee__527DBE028EC614C7");

        entity.ToTable("WorksheetWorkflowDataMap");

        entity.Property(e => e.WorksheetWorkflowDataMapId).HasColumnName("WorksheetWorkflowDataMap_Id");
        entity.Property(e => e.IdCreator).HasColumnName("Id_Creator");
        entity.Property(e => e.IdSharedWithOrganisation).HasColumnName("Id_SharedWithOrganisation");
        entity.Property(e => e.IdWorksheetWorkflow).HasColumnName("Id_WorksheetWorkflow");

        entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.WorksheetWorkflowDataMaps)
            .HasForeignKey(d => d.IdCreator)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowDataMap_AppUser");

        entity.HasOne(d => d.IdSharedWithOrganisationNavigation).WithMany(p => p.WorksheetWorkflowDataMaps)
            .HasForeignKey(d => d.IdSharedWithOrganisation)
            .HasConstraintName("FK_WorksheetWorkflowDataMap_Organisation");

        entity.HasOne(d => d.IdWorksheetWorkflowNavigation).WithMany(p => p.WorksheetWorkflowDataMaps)
            .HasForeignKey(d => d.IdWorksheetWorkflow)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowDataMap_Workflow");
    }
}
public class WorksheetWorkflowDatumConfiguration : IEntityTypeConfiguration<WorksheetWorkflowData>
{
    public void Configure(EntityTypeBuilder<WorksheetWorkflowData> entity)
    {
        entity.HasKey(e => e.WorksheetWorkflowDataId).HasName("PK__Workshee__253274EE1B08BC36");

        entity.Property(e => e.WorksheetWorkflowDataId).HasColumnName("WorksheetWorkflowData_Id");
        entity.Property(e => e.IdWorksheetWorkflowMap).HasColumnName("Id_WorksheetWorkflowMap");
        entity.Property(e => e.Key).IsRequired();
        entity.Property(e => e.Value).IsRequired();

        entity.HasOne(d => d.IdWorksheetWorkflowMapNavigation).WithMany(p => p.WorksheetWorkflowData)
            .HasForeignKey(d => d.IdWorksheetWorkflowMap)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowData_WorkflowMap");
    }
}
public class Configuration : IEntityTypeConfiguration<WorksheetWorkflowStorageMap>
{
    public void Configure(EntityTypeBuilder<WorksheetWorkflowStorageMap> entity)
    {
        entity.HasKey(e => e.WorksheetWorkflowStorageMapId).HasName("PK__Workshee__FE3EDF7CDF7C4CA5");

        entity.ToTable("WorksheetWorkflowStorageMap");

        entity.Property(e => e.WorksheetWorkflowStorageMapId).HasColumnName("WorksheetWorkflowStorageMap_Id");
        entity.Property(e => e.IdAppUser).HasColumnName("Id_AppUser");
        entity.Property(e => e.IdStorageEntry).HasColumnName("Id_StorageEntry");
        entity.Property(e => e.IdWorksheetStatusHistory).HasColumnName("Id_WorksheetStatusHistory");

        entity.HasOne(d => d.IdAppUserNavigation).WithMany(p => p.WorksheetWorkflowStorageMaps)
            .HasForeignKey(d => d.IdAppUser)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowStorageMap_AppUser");

        entity.HasOne(d => d.IdStorageEntryNavigation).WithMany(p => p.WorksheetWorkflowStorageMaps)
            .HasForeignKey(d => d.IdStorageEntry)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowStorageMap_StorageEntry");

        entity.HasOne(d => d.IdWorksheetStatusHistoryNavigation).WithMany(p => p.WorksheetWorkflowStorageMaps)
            .HasForeignKey(d => d.IdWorksheetStatusHistory)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorksheetWorkflowStorageMap_WorksheetStatusHistory");
    }
}
