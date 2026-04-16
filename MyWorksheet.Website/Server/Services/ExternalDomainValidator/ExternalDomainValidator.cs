using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.MailDomainChecker;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Shared.ViewModels;
using Microsoft.Extensions.Options;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.ExternalDomainValidator;

public class ExternalDomainInfo
{
    public ExternalDomainInfo(string domain)
    {
        Domain = domain;
        UserCalls = new ConcurrentDictionary<Guid, int>();
    }

    public string Domain { get; private set; }
    public IDictionary<Guid, int> UserCalls { get; set; }
    public int TotalCalls { get; set; }
    public int Threshhold { get; set; } = 10;
}

public class UrlValidationResult
{
    public bool IsValid { get; set; }
    public ServerProvidedTranslation[] Error { get; set; }

    public static UrlValidationResult Invalid(ServerProvidedTranslation error)
    {
        return new UrlValidationResult()
        {
            IsValid = false,
            Error = new ServerProvidedTranslation[] { error }
        };
    }

    public static UrlValidationResult Valid()
    {
        return new UrlValidationResult()
        {
            IsValid = true
        };
    }
}

[SingletonService(typeof(IExternalDomainValidator))]
public class ExternalDomainValidator : IExternalDomainValidator
{
    private readonly IOptions<AppServerSettings> _serverSettings;

    public ExternalDomainValidator(IBlacklistMailDomainService blacklistMailDomainService,
        IOptions<AppServerSettings> serverSettings)
    {
        _serverSettings = serverSettings;
        ExternalDomainInfos = [];
        BlacklistMailDomainService = blacklistMailDomainService;
    }

    public IBlacklistMailDomainService BlacklistMailDomainService { get; set; }

    public Dictionary<string, ExternalDomainInfo> ExternalDomainInfos { get; set; }

    private static readonly string[] _dns =
    {
        "localhost",
        "my-worksheet.com",
        "::1",
        "192.168.0.1",
        "127.0.0.1",
        "192.168.1.115"
    };

    public UrlValidationResult ValidateUrl(string url, params string[] expectedSchemas)
    {
        var errors = new List<ServerProvidedTranslation>();
        var allowRelativeUrls = _serverSettings.Value.External.UriRules.AllowRelative;
        var uriMode = allowRelativeUrls ? UriKind.RelativeOrAbsolute : UriKind.Absolute;

        if (!Uri.TryCreate(url, uriMode, out var uriResult))
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                return UrlValidationResult.Invalid("Url.Error/MustBeAbsolute");
            }

            return UrlValidationResult.Invalid("Url.Error/InvalidUrl");
        }

        if (expectedSchemas.Any() && !expectedSchemas.Contains(uriResult.Scheme.ToLower()))
        {
            errors.Add(new ServerProvidedTranslation()
            {
                Key = "Url.Error/InvalidSchema",
                Arguments = new[]
                {
                    expectedSchemas.Aggregate((e, f) => e + ", " + f)
                }
            });
        }

        if (_dns.Contains(uriResult.Host.ToLower()))
        {
            errors.Add(new ServerProvidedTranslation()
            {
                Key = "Url.Error/HostBlacklisted",
                Arguments = new[] { uriResult.Host }
            });
        }

        if (uriResult.IsLoopback && !_serverSettings.Value.External.UriRules.AllowLoopback)
        {
            errors.Add("Url.Error/LoopbackAddress");
        }
        if (uriResult.IsUnc && !_serverSettings.Value.External.UriRules.AllowSameNetwork)
        {
            errors.Add("Url.Error/LoopbackAddress");
        }

        if (errors.Any())
        {
            return new UrlValidationResult()
            {
                Error = errors.ToArray(),
                IsValid = false
            };
        }
        return UrlValidationResult.Valid();
    }

    public async Task<bool> CanCallDomain(string domain, Guid? userId)
    {
        ExternalDomainInfo domainInfo;
        var lowDomain = domain.ToLower();
        if (!ExternalDomainInfos.TryGetValue(lowDomain, out domainInfo))
        {
            return !await BlacklistMailDomainService.IsBlacklisted(lowDomain);
        }

        if (domainInfo.TotalCalls >= domainInfo.Threshhold)
        {
            return false;
        }

        if (userId.HasValue)
        {
            int userCallsToDomain;
            if (domainInfo.UserCalls.TryGetValue(userId.Value, out userCallsToDomain))
            {
                var setting = _serverSettings.Value.External.User.CallThreshold;
                if (userCallsToDomain >= setting)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<bool> TryCallDomain(string domain, Guid? userId)
    {
        ExternalDomainInfo domainInfo;
        var lowDomain = domain.ToLower();
        if (!ExternalDomainInfos.TryGetValue(lowDomain, out domainInfo))
        {
            var extDomainBlacklist = await BlacklistMailDomainService.IsBlacklisted(lowDomain);
            if (!extDomainBlacklist)
            {
                ExternalDomainInfos.Add(lowDomain, domainInfo = new ExternalDomainInfo(domain));
                if (userId.HasValue)
                {
                    domainInfo.UserCalls.Add(userId.Value, 1);
                }
            }
            return !extDomainBlacklist;
        }

        if (domainInfo.TotalCalls >= domainInfo.Threshhold)
        {
            return false;
        }

        if (userId.HasValue)
        {
            int userCallsToDomain;
            if (domainInfo.UserCalls.TryGetValue(userId.Value, out userCallsToDomain))
            {
                var setting = _serverSettings.Value.External.User.CallThreshold;
                if (userCallsToDomain >= setting)
                {
                    return false;
                }
                domainInfo.UserCalls[userId.Value] = userCallsToDomain + 1;
            }
            else
            {
                domainInfo.UserCalls.Add(userId.Value, 1);
            }

        }

        return true;
    }
}