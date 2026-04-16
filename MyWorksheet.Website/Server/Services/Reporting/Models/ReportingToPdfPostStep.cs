namespace MyWorksheet.Website.Server.Services.Reporting.Models;

//public class ReportingToPdfPostStep : IReportingPostStep
//{
//	public ReportingToPdfPostStep()
//	{
//		Name = "Convert To Pdf";
//		Key = "convertToPdfFile";
//	}
//	public string Name { get; set; }
//	public string Key { get; set; }
//	public Stream Process(Stream input)
//	{
//		var pdfTemplateEngine = IoC.Resolve<IPdfTemplateEngine>();
//		return pdfTemplateEngine.GenerateTemplate(input).RenderTemplate();
//	}
//}