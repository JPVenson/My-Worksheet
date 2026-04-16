using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.FileSystem;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateProvider;

[SingletonService(typeof(ITemplateProviderService))]
public class TemplateProviderService : ITemplateProviderService
{
    private readonly ILocalFileProvider _localFileProvider;

    public TemplateProviderService(ILocalFileProvider localFileProvider)
    {
        _localFileProvider = localFileProvider;
        LocalTemplates = [];
        //var settingsService = IoC.Resolve<IServerSettingsService>();
        //var logger = IoC.Resolve<IAppLogger>();
        //var fileService = IoC.ResolveLater<ILocalFileProvider>();

        //fileService.ContinueWith(f =>
        //{


        //	//foreach (var localTemplate in
        //	//	settingsService.GetSetting<string>("appSettings.template.local.enumeration").Split(',')
        //	//		.Select(e => e.Trim()))
        //	//{
        //	//	var path = settingsService.GetSetting<string>("appSettings.template." + localTemplate + ".path");
        //	//	var id = settingsService.GetSetting<int>("appSettings.template." + localTemplate + ".id");

        //	//	if (string.IsNullOrWhiteSpace(path) || id == Guid.Empty)
        //	//	{
        //	//		logger.LogWarning("Invalid Configuration for LocalTemplate",
        //	//			LoggerCategories.Reporting.ToString(), new Dictionary<string, string>()
        //	//			{
        //	//				{"Name", localTemplate},
        //	//				{"Path", path},
        //	//				{"id", id.ToString()},
        //	//			});
        //	//		return;
        //	//	}

        //	//	if (!fileService.Result.Exists(path))
        //	//	{
        //	//		logger.LogWarning("The provided template does not exists under path",
        //	//			LoggerCategories.Reporting.ToString(), new Dictionary<string, string>()
        //	//			{
        //	//				{"Name", localTemplate},
        //	//				{"Path", path},
        //	//			});
        //	//	}

        //	//	LocalTemplates.Add(new LocalTemplate()
        //	//	{
        //	//		Id = id,
        //	//		Key = localTemplate,
        //	//		Path = path
        //	//	});
        //	//}
        //});
    }

    public List<LocalTemplate> LocalTemplates { get; set; }

    public async Task<Tuple<NengineTemplate, Stream>> FindTemplate(MyworksheetContext db, Guid templateId)
    {
        var nEngineTemplate = db.NengineTemplates.Find(templateId);
        var isLocalTemplate = LocalTemplates.FirstOrDefault(e => e.Id.Equals(templateId));
        if (isLocalTemplate != null)
        {
            var localTemplate = await _localFileProvider.ReadAllAsync(isLocalTemplate.Path);
            return new Tuple<NengineTemplate, Stream>(nEngineTemplate, localTemplate);
        }
        return new Tuple<NengineTemplate, Stream>(nEngineTemplate, new MemoryStream(Encoding.UTF8.GetBytes(nEngineTemplate.Template)));
    }
}