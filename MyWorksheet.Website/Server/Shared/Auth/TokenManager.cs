using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Shared.Auth;

[SingletonService()]
public class TokenManager
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public TokenManager(IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        Tokens = new ConcurrentDictionary<string, TokenInfo>();
    }

    public ConcurrentDictionary<string, TokenInfo> Tokens { get; private set; }

    public void LogoutUser(Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();

        foreach (var keyValuePair in Tokens.Where(f => f.Value.LoginToken.IdAppUser == userId && f.Value.Valid && f.Value.LoginToken.LoginTokenId != Guid.Empty))
        {
            keyValuePair.Value.Valid = false;
            db.LoginTokens.Where(e => e.LoginTokenId == keyValuePair.Value.LoginToken.LoginTokenId).ExecuteDelete();
        }
    }

    public TokenInfo GetOrProduceTokenInfo(string tokenId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Tokens.GetOrAdd(tokenId, d =>
        {
            var token = db.LoginTokens.Where(f => f.Token == d).FirstOrDefault();
            if (token != null)
            {
                return new TokenInfo()
                {
                    LoginToken = token,
                    Valid = true
                };
            }
            return new TokenInfo()
            {
                Valid = false
            };
        });
    }

    public void LoginInfo(ClaimsIdentity claimsIdentity)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var token = new LoginToken()
        {
            IdAppUser = claimsIdentity.GetUserId(),
            ValidUntil = DateTime.Parse(claimsIdentity.FindFirst(Claims.ServerExpiresClaimId).Value),
            Token = claimsIdentity.FindFirst(Claims.ServerPasswordClaimId).Value,
        };
        db.Add(token);
        db.SaveChanges();
        var orProduceTokenInfo = GetOrProduceTokenInfo(token.Token);
        orProduceTokenInfo.Valid = true;
    }

    public bool AuthIdentity(ClaimsIdentity claimsIdentity)
    {
        var hasClaim = claimsIdentity.Claims.FirstOrDefault(f => f.Type == Claims.ServerPasswordClaimId);
        if (hasClaim == null)
        {
            return false;
        }

        var tokenInfo = GetOrProduceTokenInfo(hasClaim.Value);
        if (tokenInfo.LoginToken == null)
        {
            return false;
        }
        if (tokenInfo.LoginToken.ValidUntil < DateTime.UtcNow)
        {
            return false;
        }

        return tokenInfo.Valid;
    }
}

public class TokenInfo
{
    public LoginToken LoginToken { get; set; }
    public bool Valid { get; set; }
}