using System;
using System.Linq;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.NumberRangeService;
using MyWorksheet.Website.Server.Services.UserCounter;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Webpage.Helper;

public static class ProjectHelper
{

}

public static class AccountHelper
{
    public static Address CreateDefaultAddress()
    {
        var address = new Address();
        address.City = "N/A";
        address.FirstName = "N/A";
        address.LastName = "N/A";
        address.Phone = "N/A";
        address.Street = "N/A";
        address.StreetNo = "N/A";
        address.ZipCode = "N/A";
        return address;
    }

    public static AppUser CreateUser(MyworksheetContext db,
        AccountApiUserCreate model,
        Address address,
        string defaultRoles,
        IAppLogger logger,
        IUserQuotaService userQuotaService,
        INumberRangeService numberRangeService,
        bool isTestUser = false)
    {
        var country = db.PromisedFeatureRegions.Find(model.RegionId);
        if (country == null)
        {
            return null;
            //throw new ArgumentNullException(nameof(country));
        }

        AppUser user = null;
        using var transaction = db.Database.BeginTransaction();
        user = new AppUser();
        if (isTestUser)
        {
            user.AllowFeatureRedeeming = false;
            user.IsTestUser = true;
        }

        user.Username = model.Username;
        user.PasswordHash = ChallangeUtil.HashPassword(model.UserPlainTextPassword, user.Username);
        user.IdCountry = model.RegionId;
        user.IsAktive = true;
        user.NeedPasswordReset = model.NeedPasswordReset;
        user.CreateDate = DateTime.UtcNow;
        db.Add(user);

        address.Country = country.RegionName;
        address.DateOfCreation = DateTime.UtcNow;
        address.IdAppUserNavigation = user;
        db.Add(address);
        user.IdAddressNavigation = address;

        foreach (var roleId in defaultRoles.Split(','))
        {
            var hasRole = Roles.Roles.Yield().FirstOrDefault(e => e.Name == roleId.Trim());
            if (hasRole == null)
            {
                logger.LogWarning("Tried to add role with id " + roleId + " but there is no such role");
                continue;
            }

            db.Add(new UserRoleMap
            {
                IdRole = hasRole.Id,
                IdUser = user.AppUserId
            });
        }

        db.Add(new UserWorkload()
        {
            IdAppUser = user.AppUserId,
        });

        db.Add(new UserAssoisiatedUserMap
        {
            IdChildUser = user.AppUserId,
            IdParentUser = user.AppUserId,
            IdUserRelation = UserToUserRoles.Self.Id
        });

        userQuotaService.CreateDefaultQuotas(user.AppUserId, db);

        foreach (var numberRangeFactory in numberRangeService.NumberRangeFactories)
        {
            db.Add(new AppNumberRange()
            {
                IdUser = user.AppUserId,
                Counter = 0,
                Code = numberRangeFactory.Key,
                IsActive = true,
                Template = numberRangeFactory.Value.GetDefaultTemplate(),
            });
        }
        db.SaveChanges();
        transaction.Commit();
        return user;
    }

    public static void DeleteUser(AppUser tempUser, MyworksheetContext db,
        IBlobManagerService blobManagerService)
    {
        //delete Addresses First

        var storageEntries = db.StorageEntries
            .Where(f => f.IdAppUser == tempUser.AppUserId)
            .ToArray();

        foreach (var storageEntry in storageEntries)
        {
            blobManagerService.DeleteData(storageEntry.StorageEntryId, storageEntry.IdAppUser);
        }

        using var transaction = db.Database.BeginTransaction();
        //we have to set the CurrentAddress to null

        //delete UserQuotas

        db.UserQuota.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.Addresses.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.WorksheetTracks.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.PaymentOrders.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.DashboardPlugins.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();

        //delete Worksheet Structure
        db.UserWorkloads.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.ProjectBudgets.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.WorksheetStatusHistories.Where(e => e.IdChangeUser == tempUser.AppUserId).ExecuteDelete();
        db.ProjectItemRates.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();
        db.Worksheets.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();
        db.WorksheetItems.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();
        db.WorksheetItemStatusLookups.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.WorksheetItemStatuses.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();
        db.Projects.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();



        db.UserRoleMaps.Where(e => e.IdUser == tempUser.AppUserId).ExecuteDelete();
        db.UserAssoisiatedUserMaps.Where(e => e.IdUserRelation == tempUser.AppUserId).ExecuteDelete();
        db.UserDocumentCaches.Where(e => e.IdUser == tempUser.AppUserId).ExecuteDelete();
        db.LoginTokens.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.UserActions.Where(e => e.IdUser == tempUser.AppUserId).ExecuteDelete();
        db.OutgoingWebhookActionLogs.Where(e => e.IdAppUser == tempUser.AppUserId).ExecuteDelete();
        db.OutgoingWebhooks.Where(e => e.IdCreator == tempUser.AppUserId).ExecuteDelete();
        db.AssosiationInvitations.Where(e => e.IdRequestingUser == tempUser.AppUserId).ExecuteDelete();

        db.AppUsers.Remove(tempUser);
        db.SaveChanges();
        transaction.Commit();
    }
}