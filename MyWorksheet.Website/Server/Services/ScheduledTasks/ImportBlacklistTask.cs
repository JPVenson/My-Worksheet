using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Shared.TaskScheduling;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.ScheduledTasks;

[ScheduleTaskAt(6, 23, 0, 0)]
public class ImportBlacklistTask : BaseTask
{
    public ImportBlacklistTask(IDbContextFactory<MyworksheetContext> dbContextFactory) : base(dbContextFactory)
    {
        BlacklistProvider = new Dictionary<string, Func<Task<MailBlacklist[]>>>
        {
            {
                "Mailinator",
                async () =>
        {
            var result = new List<MailBlacklist>();
            using (var client = new HttpClient())
            {
                var domains = await client.GetStringAsync(
                    "https://raw.githubusercontent.com/GeroldSetz/Mailinator-Domains/master/mailinator_domains_from_bdea.cc.txt");

                result.AddRange(domains.Split(_lb).Select(e => new MailBlacklist()
                {
                    X2hash = e
                }));
            }
            return result.ToArray();
        }
            },
            {
                "adamloving",
                async () =>
            {
                var result = new List<MailBlacklist>();
                using (var client = new HttpClient())
                {
                    var domains = await client.GetStringAsync(
                        "https://gist.githubusercontent.com/adamloving/4401361/raw/db901ef28d20af8aa91bf5082f5197d27926dea4/temporary-email-address-domains");

                    result.AddRange(domains.Split(_lb).Select(e => new MailBlacklist()
                    {
                        X2hash = HashIt(e),
                        ClearName = e
                    }));
                }
                return result.ToArray();
            }
            }
        };
    }

    private string HashIt(string domain)
    {
        return SHA1.HashData(Encoding.ASCII.GetBytes(domain.ToLower())).Select(e => e.ToString("X2")).Aggregate((e, f) => e + f).ToLower();
    }

    private static readonly char[] _lb = new[] { '\r', '\n' };

    public IDictionary<string, Func<Task<MailBlacklist[]>>> BlacklistProvider { get; set; }

    public override async Task DoWorkAsync(TaskContext context)
    {
        using var db = DbContectFactory.CreateDbContext();
        foreach (var func in BlacklistProvider)
        {
            context.Logger.LogInformation("Load Blacklist from " + func.Key);
            MailBlacklist[] loadList;
            try
            {
                loadList = await func.Value();
            }
            catch (Exception e)
            {
                context.Logger.LogError("Load Blacklist from " + func.Key + " Failed", null, new Dictionary<string, string>()
                {
                    {"Exception", e.ToString()}
                });
                continue;
            }
            context.Logger.LogInformation("Loaded " + loadList.Length + " from " + func.Key);
            var imported = 0;
            foreach (var item in loadList)
            {
                var firstOrDefault = db.MailBlacklists.Where(f => f.X2hash == item.X2hash).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    item.X2hash = item.X2hash.ToLower();
                    db.Add(item);
                    imported++;
                }
            }
            db.SaveChanges();
            context.Logger.LogInformation("Imported " + imported + " from " + func.Key + "." + (loadList.Length - imported) + " Duplicates");
        }
    }

    public override string NamedTask { get; protected set; } = "Import Mail-Blacklist";
}