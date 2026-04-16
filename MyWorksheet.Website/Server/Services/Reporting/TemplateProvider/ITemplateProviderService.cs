using System;
using System.IO;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateProvider;

public interface ITemplateProviderService
{
    Task<Tuple<NengineTemplate, Stream>> FindTemplate(MyworksheetContext db, Guid templateId);
}