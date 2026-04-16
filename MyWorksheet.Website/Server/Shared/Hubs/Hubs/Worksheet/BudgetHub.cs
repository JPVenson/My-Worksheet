//using System.Threading.Tasks;
//using MyWorksheet.Shared.WebApi;
//using MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//using MyWorksheet.Website.Server.Shared.Hubs.Hubs.Server;
//using MyWorksheet.Website.Server.Util.Auth;
//using Microsoft.AspNetCore.SignalR;

//namespace MyWorksheet.Website.Server.Shared.Hubs.Hubs.Worksheet
//{
//	[HubName("BudgetHub")]
//	[RevokableAuthorize]
//	public class BudgetHub : Hub<IBudgetHub>
//	{
//		public async Task RegisterProjectBudgetChanged(Guid projectId)
//		{
//			await Groups.AddToGroupAsync(Context.ConnectionId, "ProjectBudget_Changed" + projectId + "_" + Context.User?.GetUserId() ?? "ALL");
//		}

//		public async Task UnRegisterProjectBudgetChanged(Guid projectId)
//		{
//			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ProjectBudget_Changed" + projectId + "_" + Context.User?.GetUserId() ?? "ALL");
//		}

//		public async Task SendProjectBudgetChanged(Guid projectId, Guid budgetId, Guid userId)
//		{
//			await Clients
//				.Group("ProjectBudget_Changed" + projectId + "_" + userId ?? "ALL")
//				.Changed(projectId, budgetId);
//		}
//	}
//}