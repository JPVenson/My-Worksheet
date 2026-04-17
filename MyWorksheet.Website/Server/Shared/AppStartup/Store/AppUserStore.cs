using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.AppStartup.Store;

[ScopedService(typeof(IUserStore<AppUser>))]
public class AppUserStore : IUserStore<AppUser>, IUserPasswordStore<AppUser>, IUserRoleStore<AppUser>
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public AppUserStore(IDbContextFactory<MyworksheetContext> db)
    {
        _dbContextFactory = db;
    }
    public void Dispose()
    {
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
    {
        if (user.RowState == null || user.RowState.Length == 0)
        {
            user.RowState = Guid.NewGuid().ToByteArray();
        }

        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            db.Add(user);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            return db.AppUsers.Find(userId);
        }
    }

    public async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            var findByNameAsync = db.AppUsers
                .Where(f => f.Username.ToUpper() == normalizedUserName)
                .FirstOrDefault();
            return findByNameAsync;
        }
    }

    public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Username);
    }

    public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.AppUserId.ToString());
    }

    public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Username);
    }

    public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.Username = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
    {
        user.Username = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            db.Update(user);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return IdentityResult.Success;
    }

    public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash.ToDecHex());
    }

    public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
    {
        Task.FromResult(user.PasswordHash = ChallangeUtil.StringToByteArrayFastest(passwordHash));
        return Task.CompletedTask;
    }

    public async Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            var role = db.Roles
            .Where(f => f.RoleName == roleName)
            .FirstOrDefault();

            db.Add(new UserRoleMap()
            {
                IdRole = role.RoleId,
                IdUser = user.AppUserId
            });
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            await db.UserRoleMaps
            .Where(f => f.IdUser == user.AppUserId)
            .Where(f => f.IdRoleNavigation.RoleName == roleName)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            return await db.UserRoleMaps.Where(e => e.IdUser == user.AppUserId)
                .Select(e => e.IdRoleNavigation.RoleName).ToArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
    {
        var roles = await GetRolesAsync(user, cancellationToken).ConfigureAwait(false);
        return roles.Any(f => f == roleName);
    }

    public async Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await using (db.ConfigureAwait(false))
        {
            return await db.AppUsers
                .Where(e => e.UserRoleMaps.Any(f => f.IdRoleNavigation.RoleName == roleName)).ToArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}