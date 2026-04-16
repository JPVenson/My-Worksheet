namespace MyWorksheet.Website.Server.Shared.Hubs.HubInfos;

//public class GeoFenceHubInfo : HubAccess
//{
//	/// <inheritdoc />
//	public override Type HubType { get; } = typeof(GeoFenceHub);

//	public void RegisterGeoFenceChanged(string connectionId, Guid userId, int? geoFenceId)
//	{
//		HubContext.Groups.Add(connectionId, userId + "_all");
//		if (geoFenceId.HasValue)
//		{
//			HubContext.Groups.Add(connectionId, userId + "_" + geoFenceId);
//		}
//	}

//	public void UnRegisterGeoFenceChanged(string connectionId, Guid userId, int? geoFenceId)
//	{
//		HubContext.Groups.Remove(connectionId, userId + "_all");
//		if (geoFenceId.HasValue)
//		{
//			HubContext.Groups.Remove(connectionId, userId + "_" + geoFenceId);
//		}
//	}

//	public void TriggerGeoFenceChanged(Guid userId, Guid geoFenceId)
//	{
//		HubContext.Clients.Group(userId + "_" + geoFenceId).GeoFenceChanged(geoFenceId);
//		HubContext.Clients.Group(userId + "_all").GeoFenceChanged(geoFenceId);
//	}

//	public void TriggerGeoFenceCreated(Guid userId, Guid geoFenceId)
//	{
//		HubContext.Clients.Group(userId + "_" + geoFenceId).GeoFenceCreated(geoFenceId);
//		HubContext.Clients.Group(userId + "_all").GeoFenceCreated(geoFenceId);
//	}

//	public void TriggerGeoFenceDeleted(Guid userId, Guid geoFenceId)
//	{
//		HubContext.Clients.Group(userId + "_" + geoFenceId).GeoFenceDeleted(geoFenceId);
//		HubContext.Clients.Group(userId + "_all").GeoFenceDeleted(geoFenceId);
//	}
//}