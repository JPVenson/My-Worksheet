using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.UserCounter;

public interface IUserQuotaService
{
    IDictionary<Quotas, UserQuotaPart> UserQuotaParts { get; set; }

    Task<bool> Add(Guid userId, long value, Quotas quota);
    bool Allowed(Guid userId, Quotas quota);
    Task<bool> Subtract(Guid userId, long value, Quotas quota);
    void CreateDefaultQuotas(Guid userId, MyworksheetContext db);
    Task Expand(Guid userId, long value, Quotas quota);
    Task Reduce(Guid userId, long value, Quotas quota);
}