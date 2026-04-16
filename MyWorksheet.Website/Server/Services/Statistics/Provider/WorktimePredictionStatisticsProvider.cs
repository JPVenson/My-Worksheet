//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Katana.CommonTasks.Unity;
//using MyWorksheet.Entities.Manager;
//using MyWorksheet.Entities.Poco;
//using MyWorksheet.Helper;
//using MyWorksheet.Private.Models.ObjectSchema;
//using MyWorksheet.Public.Models.ViewModel.ApiResultModels.Statistics;

//namespace MyWorksheet.Webpage.Services.Statistics.Provider
//{
//	public class WorktimePredictionStatisticsProvider : IStatisticsProvider
//	{
//		public string Display { get; } = "Worktime Prediction";

//		class StatisticsArguments : ArgumentsBase
//		{
//			public static StatisticsArguments Parse(IDictionary<string, object> arguments)
//			{
//				var statProvider = new StatisticsArguments();
//				statProvider.SetOrAbort(arguments, nameof(DateView));
//				statProvider.SetOrAbort(arguments, nameof(NoOfDays));
//				return statProvider.GetIfValid() as StatisticsArguments;
//			}

//			public DateTime DateView { get; set; }
//			public int NoOfDays { get; set; }
//		}

//		public IObjectSchema Arguments(DbEntities db)
//		{
//			return JsonHelper.JsonSchema(typeof(StatisticsArguments), db.Config, null);
//		}

//		public DataExport GenerateSchema(Guid appUserId, 
//			IDictionary<string, object> arg, 
//			IEnumerable<int> projects, 
//			AggregationStrategy aggregateStrategy)
//		{
//			var arguments = StatisticsArguments.Parse(arg);

//			if (arguments == null)
//			{
//				return null;
//			}

//			var dbEntities = IoC.Resolve<DbEntities>();
//			var worksheets = dbEntities.WorksheetItems
//				.Where
//				.Column(f => f.IdCreator).Is.EqualsTo(appUserId)
//				.ToArray();

//			//take months that are the same as requested
//			var worksheetItemsInSameMonth = worksheets.Where(e => e.DateOfAction.Month == arguments.DateView.Month);

//			var orderByDay = worksheetItemsInSameMonth.GroupBy(e => e.DateOfAction.Day);

//			var export = new DataExport();
//			var worktimeDay = new DataDimension(new LabelOversight("Worktime"));
//			export.DataDimensions.Add(worktimeDay);

//		}
//	}
//}