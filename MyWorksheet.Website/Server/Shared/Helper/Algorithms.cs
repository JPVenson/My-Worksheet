using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.MailDomainChecker;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.AspNetCore.Identity;

namespace MyWorksheet.Helper;

public static class Algorithms
{
    public static string Pepper { get; set; } = "02EB94B6-0ECF-4744-A882-E9A2047E3E30";

    public static async Task<QuestionableBoolean> CheckMail(string mailAddress, MyworksheetContext db,
        IBlacklistMailDomainService blacklistMailDomainService)
    {
        if (mailAddress == null)
        {
            return false.Because("No Mail-Address");
        }

        if (mailAddress.Count(f => f == '@') != 1)
        {
            return false.Because("Invalid Mail-Address");
        }
        var mail = mailAddress.Split('@');
        var domain = mail[1];
        var casedMail = mail[0];

        var isMailOk = new Regex("(?:[a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
        if (!isMailOk.IsMatch(mailAddress))
        {
            return false.Because("Invalid Mail-Address");
        }
        var hasMailRegistrated = db.AppUsers.Where(f => f.Email == mailAddress || f.Username == mailAddress).FirstOrDefault();

        if (hasMailRegistrated != null)
        {
            return false.Because("This mail or username is already registered.");
        }

        if (await blacklistMailDomainService.IsBlacklisted(domain))
        {
            return false.Because("Your domain is not allowed");
        }
        return true;
    }

    public static string GeneratePassword(UserManager<AppUser> userManager)
    {
        var options = userManager.Options.Password;

        int length = options.RequiredLength;

        bool nonAlphanumeric = options.RequireNonAlphanumeric;
        bool digit = options.RequireDigit;
        bool lowercase = options.RequireLowercase;
        bool uppercase = options.RequireUppercase;

        StringBuilder password = new StringBuilder();
        Random random = new Random();

        while (password.Length < length)
        {
            char c = (char)random.Next(32, 126);

            password.Append(c);

            if (char.IsDigit(c))
            {
                digit = false;
            }
            else if (char.IsLower(c))
            {
                lowercase = false;
            }
            else if (char.IsUpper(c))
            {
                uppercase = false;
            }
            else if (!char.IsLetterOrDigit(c))
            {
                nonAlphanumeric = false;
            }
        }

        if (nonAlphanumeric)
        {
            password.Append((char)random.Next(33, 48));
        }

        if (digit)
        {
            password.Append((char)random.Next(48, 58));
        }

        if (lowercase)
        {
            password.Append((char)random.Next(97, 123));
        }

        if (uppercase)
        {
            password.Append((char)random.Next(65, 91));
        }

        return password.ToString();
    }

    public static string GenerateUsername(string mail, MyworksheetContext db)
    {
        do
        {
            var username = LoginChallangeManager.GetRandNumber(1000000, 69999999, mail.GetHashCode()).ToString();
            if (db.AppUsers.Where(f => f.Username == username).FirstOrDefault() == null)
            {
                return username;
            }
        } while (true);
    }
}