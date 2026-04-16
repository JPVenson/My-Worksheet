using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services.MailDomainChecker;

public interface IBlacklistMailDomainService
{
    Task<bool> IsBlacklisted(string domain);
}