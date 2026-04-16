using System.Threading.Tasks;

using System;
namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos.Worksheet;
//public class BudgetHubInfo : HubAccess
//{
//	public override Type HubType { get; } = typeof(BudgetHub);

//	public void RegisterProjectBudgetChanged(string contextConnectionId, Guid projectId, int? userId = null)
//	{
//		HubContext.Groups.Add(contextConnectionId, "ProjectBudget_Changed" + projectId + "_" + userId ?? "ALL");
//	}

//	public void UnRegisterProjectBudgetChanged(string contextConnectionId, Guid projectId, int? userId = null)
//	{
//		HubContext.Groups.Remove(contextConnectionId, "ProjectBudget_Changed" + projectId + "_" + userId ?? "ALL");
//	}

//	public void SendProjectBudgetChanged(Guid projectId, Guid budgetId, int? userId = null)
//	{
//		HubContext.Clients.Group("ProjectBudget_Changed" + projectId + "_" + userId ?? "ALL")
//			.Changed(new
//			{
//				ProjectId = projectId,
//				BudgetId = budgetId
//			});
//	}
//}

public interface IBudgetHub
{
    Task Changed(Guid projectId, Guid budgetId);
}