using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Shared.Services.Activation;
using Microsoft.EntityFrameworkCore;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

[SingletonService(typeof(INumberRangeService))]
public class NumberRangeService : INumberRangeService
{
    private readonly ActivatorService _activatorService;

    public NumberRangeService(ActivatorService activatorService)
    {
        _activatorService = activatorService;
        NumberRangeFactories = new Dictionary<string, INumberRangeFactory>
        {
            { InvoiceNumberRangeFactory.NrCode, _activatorService.ActivateType<InvoiceNumberRangeFactory>() },
            { ProjectNumberRangeFactory.NrCode, _activatorService.ActivateType<ProjectNumberRangeFactory>() },
            { WorksheetNumberRangeFactory.NrCode, _activatorService.ActivateType<WorksheetNumberRangeFactory>() },
            { WebhookNumberRangeFactory.NrCode, _activatorService.ActivateType<WebhookNumberRangeFactory>() }
        };
    }

    public IDictionary<string, INumberRangeFactory> NumberRangeFactories { get; set; }

    public async Task<string> Test(string code, string template, long counter)
    {
        if (NumberRangeFactories.TryGetValue(code, out var nrRangeFactory))
        {
            return await nrRangeFactory.GetNumberEntry(nrRangeFactory.GetTestData(), template, counter);
        }

        return null;
    }

    public async Task<string> GetNumberRangeAsync(MyworksheetContext db, string key, Guid userId, object additonalData)
    {
        var nrRanges = db.AppNumberRanges
            .Where(e => e.IdUser == userId)
            .Where(f => f.Code == key)
            .Where(f => f.IsActive == true)
            .ToArray();

        if (nrRanges.Length == 0)
        {
            return null;
        }

        var nrRange = nrRanges.FirstOrDefault(e => e.IdUser == userId);

        if (NumberRangeFactories.TryGetValue(key, out var nrRangeFactory) && nrRange != null)
        {
            var numberEntry = await nrRangeFactory.GetNumberEntry(additonalData, nrRange.Template, nrRange.Counter);
            if (numberEntry != null)
            {
                db.AppNumberRanges.Where(e => e.AppNumberRangeId == nrRange.AppNumberRangeId)
                    .ExecuteUpdate(f => f.SetProperty(e => e.Counter, w => w.Counter + 1));
            }
            return numberEntry;
        }

        return null;
    }
}