using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.MailDomainChecker;

namespace MyWorksheet.Website.Server.Services.ExternalDomainValidator;

public interface IExternalDomainValidator
{
    IBlacklistMailDomainService BlacklistMailDomainService { get; set; }
    Dictionary<string, ExternalDomainInfo> ExternalDomainInfos { get; set; }

    Task<bool> CanCallDomain(string domain, Guid? userId);
    Task<bool> TryCallDomain(string domain, Guid? userId);
    UrlValidationResult ValidateUrl(string url, params string[] expectedSchemas);
}