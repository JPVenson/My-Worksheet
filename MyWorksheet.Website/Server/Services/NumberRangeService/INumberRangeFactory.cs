using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public interface INumberRangeFactory
{
    Task<string> GetNumberEntry(object additonalData, string template, long counter);
    string Code { get; }
    string Description { get; }

    IObjectSchemaInfo GetSchema(MyworksheetContext db);
    string GetDefaultTemplate();
    object GetTestData();
}