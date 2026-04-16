//using System;
//using MyWorksheet.Webpage.AppStartup;
//using MyWorksheet.Webpage.Entities.Manager;
//using MyWorksheet.Webpage.Entities.Poco;

//namespace MyWorksheet.Webpage.Services.Activity.Types
//{
//	public class ActivityProjectQuotaNearlyExceeded : ActivityType
//	{
//		public ActivityProjectQuotaNearlyExceeded() : base("quota_project_below20percent")
//		{
//		}

//		public string FormatKey(UserCounter ws)
//		{
//			return ws.UserCounterId.ToString();
//		}

//		public UserActivity CreateActivity(DbEntities db, UserCounter counter)
//		{
//			return new UserActivity()
//			{
//				ActivityType = TypeKey,
//				DateCreated = DateTime.UtcNow,
//				SystemActivityTypeKey = FormatKey(counter),
//				HeaderHtml = "Your Quota for Projects " + counter.ProjectCounter + " of " + counter.ProjectLengthCounter + " are low.",
//				BodyHtml = "The Quota for Projectes are below 20%.",
//				IdAppUser = counter.UserId
//			};
//		}

//		public bool CheckAndCreate(DbEntities db, UserCounter ws)
//		{
//			var pc = (ws.ProjectCounter / ws.ProjectLengthCounter) * 100;
//			if (pc > 80)
//			{
//				var activityService = IoC.Resolve<IActivityService>();
//				activityService.CreateActivity(CreateActivity(db, ws));
//				return true;
//			}
//			return false;
//		}
//	}
//}