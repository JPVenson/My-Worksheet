using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.MailDomainChecker;

[SingletonService(typeof(IBlacklistMailDomainService))]
public class BlacklistMailDomainService : IBlacklistMailDomainService
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public BlacklistMailDomainService(IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<bool> IsBlacklisted(string domain)
    {
        await Task.CompletedTask;
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(domain.ToLower())).Select(e => e.ToString("X2")).Aggregate((e, f) => e + f).ToLower();
        return _dbContextFactory.CreateDbContext().MailBlacklists.Where(f => f.X2hash == hash).FirstOrDefault() != null;
    }
}