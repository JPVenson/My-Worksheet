using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Workflow;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;

namespace MyWorksheet.Website.Client.Services.Repository;

//public interface ILocalRepository<TEntity>
//{
//	Task<ApiResult<TEntity>> Get(Guid id);
//	Task<ApiResult<TEntity[]>> Get(params int[] ids);
//}

//[SingletonService(typeof(ILocalRepository<OrganizationViewModel>))]
//public class OrganisationRepository : LocalRestRepository<OrganizationViewModel>
//{
//	public OrganisationRepository(HttpService httpService, ChangeTrackingService changeTrackingService)
//		: base(httpService, changeTrackingService)
//	{
//	}
//}

//public class LocalRestRepository<TEntity> : ILocalRepository<TEntity>
//{
//	public ChangeTrackingService TrackingService { get; }
//	public HttpService HttpService { get; }

//	private class EntityStore
//	{
//		public EntityStore(TEntity entity)
//		{
//			Entity = entity;
//		}

//		public TEntity Entity { get; set; }
//		public bool Stale { get; set; }
//	}

//	private IDictionary<int, EntityStore> Store { get; set; }

//	public LocalRestRepository(HttpService httpService, ChangeTrackingService changeTrackingService)
//	{
//		TrackingService = changeTrackingService;
//		HttpService = httpService;
//		Store = new Dictionary<int, EntityStore>();
//		TrackingService.RegisterTracking(typeof(TEntity), Changed);
//	}

//	private void Changed(ChangeEventTypes type, Guid id)
//	{
//		switch (type)
//		{
//			case ChangeEventTypes.Added:
//				break;
//			case ChangeEventTypes.Removed:
//				Store.Remove(id);
//				break;
//			case ChangeEventTypes.Changed:
//				if (Store.TryGetValue(id, out var store))
//				{
//					store.Stale = true;
//				}
//				break;
//		}
//	}

//	public virtual Task<ApiResult<TEntity>> Get(Guid id)
//	{
//		if (Store.TryGetValue(id, out var entity) && !entity.Stale)
//		{
//			return Task.FromResult(new ApiResult<TEntity>(HttpStatusCode.OK, true, entity.Entity, null));
//		}

//		return GetEntity(id);
//	}

//	private Task<ApiResult<TEntity>> GetEntity(Guid id)
//	{
//		return HttpService.For<TEntity>().GetSingle(id)
//			.AsTask()
//			.ContinueWith((t) =>
//			{
//				var apiResult = t.Result;
//				if (apiResult.Success)
//				{
//					Store[id] = new EntityStore(apiResult.Object);
//				}

//				return t.Result;
//			});
//	}

//	public virtual Task<ApiResult<TEntity[]>> Get(params int[] ids)
//	{
//		var elements = new List<TEntity>();
//		var toSearch = new List<int>();
//		foreach (var id in ids.Distinct())
//		{
//			if (Store.TryGetValue(id, out var entity) && !entity.Stale)
//			{
//				elements.Add(entity.Entity);
//			}
//			else
//			{
//				toSearch.Add(id);
//			}
//		}

//		if (toSearch.Any())
//		{
//			return HttpService.For<TEntity>().GetList(toSearch.ToArray())
//				.AsTask()
//				.ContinueWith((t) =>
//				{
//					var apiResult = t.Result;
//					if (!apiResult.Success)
//					{
//						return apiResult;
//					}

//					foreach (var entity in t.Result.Object)
//					{
//						Store[entity.GetId()] = new EntityStore(entity);
//					}

//					elements.AddRange(apiResult.Object);
//					return new ApiResult<TEntity[]>(apiResult.StatusCode, apiResult.Success, elements.ToArray(), null);
//				});
//		}

//		return Task.FromResult(new ApiResult<TEntity[]>(HttpStatusCode.OK, true, elements.ToArray(), null));
//	}
//}