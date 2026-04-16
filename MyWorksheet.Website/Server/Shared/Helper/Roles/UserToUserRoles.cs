using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Webpage.Helper.Roles;

public static class UserToUserRoles
{
    public const string SelfRoleName = "Self";
    public const string WorksheetHolderRoleName = "WorksheetHolder";
    public const string AdministratedByRoleName = "Administrator";

    public static RoleViewModel Self { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0002-000000000001"), SelfRoleName, "You");
    public static RoleViewModel WorksheetHolder { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0002-000000000002"), WorksheetHolderRoleName, "The Owner of Worksheets and Projects. This user can be selected to be Owner of certain Projects.");
    public static RoleViewModel Administrated { get; } = new RoleViewModel(new Guid("00000000-0000-0000-0002-000000000003"), AdministratedByRoleName, "The Adminstrator of this Account. This user can observe all changes and can manipulate all of your settings.");

    public static IEnumerable<RoleViewModel> YieldRoles()
    {
        yield return Self;
        yield return WorksheetHolder;
        yield return Administrated;
    }

    public static RoleViewModel Find(Guid userAssosiatedRoleLookupId)
    {
        return YieldRoles().FirstOrDefault(f => f.Id == userAssosiatedRoleLookupId);
    }
}