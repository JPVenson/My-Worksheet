using System;
using System.Collections.Generic;

namespace MyWorksheet.Webpage.Helper.Roles;

public static class UserToOrgRoles
{
    public const string AdminRoleName = "Administrator";
    public const string CustomerRoleName = "Customer";
    public const string CreatorRoleName = "Creator";
    public const string ProjectManagerRoleName = "ProjectManager";

    public static RoleViewModel Administrator { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0001-000000000001"), AdminRoleName, "Administrator");
    public static RoleViewModel Customer { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0001-000000000002"), CustomerRoleName, "Can bill Projects on this Organisation");
    public static RoleViewModel Creator { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0001-000000000003"), CreatorRoleName, "Has created this Organisation");
    public static RoleViewModel ProjectManager { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0001-000000000004"), ProjectManagerRoleName, "Can create a Project for this Organisation and alter its properties");

    public static IEnumerable<RoleViewModel> Yield()
    {
        yield return Administrator;
        yield return Customer;
        yield return Creator;
        yield return ProjectManager;
    }
}

public static class Roles
{
    public const string AdminRoleName = "Administrator";
    public const string WorksheetUserRoleName = "WorksheetUser";
    public const string WorksheeActiontUserRoleName = "WorksheetActionsUser";
    public const string WorksheetAdminRoleName = "WorksheetAdmin";
    public const string OrganisationAdminName = "OrganisationAdmin";
    public const string SettingsChangerName = "SettingsUsers";
    public const string UserRoleName = "Kunde";
    public const string VisitorRoleName = "Visitor";
    public const string OnDemandUserRoleName = "OnDemandUser";
    public const string ProjectManagerRoleName = "ProjectManager";

    public static RoleViewModel Administrator { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000001"), AdminRoleName, "Administrator");
    public static RoleViewModel Kunde { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000002"), UserRoleName, "User of My-Worksheet");
    public static RoleViewModel Visitor { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000003"), VisitorRoleName, "Visitor of My-Worksheet");
    public static RoleViewModel WorksheetAdmin { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000004"), WorksheetAdminRoleName, "Can Administrate Worksheets for this account");
    public static RoleViewModel WorksheetUser { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000005"), WorksheetUserRoleName, "Can Display and enter new Times inside a Worksheet");
    public static RoleViewModel WorksheetActionUser { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000006"), WorksheeActiontUserRoleName, "Can create new Worksheet Actions");
    public static RoleViewModel OrganisationAdmin { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000007"), OrganisationAdminName, "Can create new Organisations");
    public static RoleViewModel SettingsChanger { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000008"), SettingsChangerName, "Can modify its own Settings");
    public static RoleViewModel OnDemandUser { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-000000000009"), OnDemandUserRoleName, "This user has directly registrated on the webseite and was not created by another user");
    public static RoleViewModel ProjectManager { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0000-00000000000a"), ProjectManagerRoleName, "This user can create projects");

    public static IEnumerable<RoleViewModel> Yield()
    {
        yield return Administrator;
        yield return Kunde;
        yield return Visitor;
        yield return WorksheetAdmin;
        yield return WorksheetUser;
        yield return WorksheetActionUser;
        yield return OrganisationAdmin;
        yield return SettingsChanger;
        yield return OnDemandUser;
        yield return ProjectManager;
    }
}
