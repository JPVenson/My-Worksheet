using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public interface INumberRangeService
{
    Task<string> GetNumberRangeAsync(MyworksheetContext db, string key, Guid userId, object additonalData);

    IDictionary<string, INumberRangeFactory> NumberRangeFactories { get; }
    Task<string> Test(string code, string template, long counter);
}