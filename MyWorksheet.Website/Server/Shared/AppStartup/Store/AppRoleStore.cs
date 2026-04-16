using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.AppStartup.Store;

[ScopedService(typeof(IRoleStore<Role>))]
public class AppRoleStore : IRoleStore<Role>
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public AppRoleStore(IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public void Dispose()
    {
    }

    public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            db.Roles.Add(role);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            return db.Roles.Find(roleId);
        }
    }

    public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            return db.Roles
                .FirstOrDefault(f => f.RoleName == normalizedRoleName);
        }
    }

    public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.RoleName);
    }

    public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.RoleId.ToString());
    }

    public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.RoleName);
    }

    public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
    {
        role.RoleName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
    {
        role.RoleName = roleName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            db.Roles.Update(role);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }
}