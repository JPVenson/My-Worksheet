using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.Services;
using ServiceLocator.Attributes;

namespace MyWorksheet.Webpage.Helper.Roles;

[SingletonService]
public class UserToUserRoles : RequireInit
{
    public const string SelfRoleName = "Self";
    public const string WorksheetHolderRoleName = "WorksheetHolder";
    public const string AdministratedByRoleName = "Administrator";

    public static RoleViewModel Self { get; private set; }
    public static RoleViewModel WorksheetHolder { get; private set; }
    public static RoleViewModel Administrated { get; private set; }

    public override async ValueTask InitAsync(IServiceProvider services)
    {
        await using var db = await services.GetRequiredService<IDbContextFactory<MyworksheetContext>>().CreateDbContextAsync().ConfigureAwait(false);
        var roleTypes = db.UserAssosiatedRoleLookups.ToArray();
        Self = new(roleTypes.First(e => e.Description == SelfRoleName).UserAssosiatedRoleLookupId, SelfRoleName, "You");
        WorksheetHolder = new(roleTypes.First(e => e.Description == WorksheetHolderRoleName).UserAssosiatedRoleLookupId, WorksheetHolderRoleName, "The Owner of Worksheets and Projects. This user can be selected to be Owner of certain Projects.");
        Administrated = new(roleTypes.First(e => e.Description == AdministratedByRoleName).UserAssosiatedRoleLookupId, AdministratedByRoleName, "The Adminstrator of this Account. This user can observe all changes and can manipulate all of your settings.");
    }

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