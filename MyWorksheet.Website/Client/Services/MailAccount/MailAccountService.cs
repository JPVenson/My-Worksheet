using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.MailAccount;

[SingletonService()]
public class MailAccountService
{
    public MailAccountService(ICacheRepository<EMailAccountViewModel> mailAccount)
    {
        MailAccounts = mailAccount;
    }

    public ICacheRepository<EMailAccountViewModel> MailAccounts { get; set; }

    public static IEnumerable<EMailProtcol> GetProtocols()
    {
        //mirrored from ApplicationMailService
        yield return new EMailProtcol()
        {
            Id = 0,
            Name = "None"
        };
        yield return new EMailProtcol()
        {
            Id = 1,
            Name = "STARTTLS"
        };
        yield return new EMailProtcol()
        {
            Id = 2,
            Name = "SSL"
        };
    }
}