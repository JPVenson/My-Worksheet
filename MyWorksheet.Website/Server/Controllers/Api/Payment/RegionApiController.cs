// using System.Collections.Generic;
// using System.Globalization;
// using System.Linq;
// using MyWorksheet.Entities.Poco;
// using MyWorksheet.Shared.WebApi;
// using MyWorksheet.Webpage.Helper.Roles;
// using MyWorksheet.Website.Server.Services.Mapping;
// using MyWorksheet.Website.Server.Services.Monetary;
// using MyWorksheet.Website.Server.Util.Auth;
// using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
// using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace MyWorksheet.Website.Server.Controllers.Api.Payment;

// public class RegionCurrencyComparer : IEqualityComparer<RegionInfo>
// {
// 	public bool Equals(RegionInfo x, RegionInfo y)
// 	{
// 		if (ReferenceEquals(x, y))
// 		{
// 			return true;
// 		}

// 		if (ReferenceEquals(x, null))
// 		{
// 			return false;
// 		}

// 		if (ReferenceEquals(y, null))
// 		{
// 			return false;
// 		}

// 		if (x.GetType() != y.GetType())
// 		{
// 			return false;
// 		}

// 		return x.CurrencySymbol == y.CurrencySymbol;
// 	}

// 	public int GetHashCode(RegionInfo obj)
// 	{
// 		return obj.CurrencySymbol.GetHashCode();
// 	}
// }

// [Route("api/RegionApi")]
// public class RegionApiControllerBase : RestApiControllerBase<PromisedFeatureRegion, FeatureRegionViewModel>
// {
// 	private readonly IMapperService _mapper;
// 	private readonly IDbContextFactory<MyworksheetContext> _dbFactory;
// 	private readonly IValueExchangeService _valueExchangeService;

// 	public RegionApiControllerBase(IMapperService mapper, 
// 		IDbContextFactory<MyworksheetContext> dbFactory, 
// 		IValueExchangeService valueExchangeService) : base(dbFactory, mapper)
// 	{
// 		_mapper = mapper;
// 		_dbFactory = dbFactory;
// 		_valueExchangeService = valueExchangeService;
// 	}

// 	[HttpGet]
// 	[Route("AllCurrencys")]
// 	[Authorize]
// 	public IActionResult GetAllCurrencys(string filter = null)
// 	{
// 		var regions = _valueExchangeService.RegionInfos.Distinct(new RegionCurrencyComparer());
// 		if (filter != null)
// 		{
// 			regions = regions.Where(e => e.Name.Contains(filter) || e.CurrencySymbol.Contains(filter) ||
// 			                             e.ISOCurrencySymbol.Contains(filter)).ToArray();
// 		}

// 		return Data(_mapper.ViewModelMapper.Map<CurrencyLookup[]>(regions));
// 	}

// 	[HttpGet]
// 	[Route("GetUsersRegion")]
// 	[Authorize]
// 	public IActionResult GetUsersRegion()
// 	{
// 		var db = _dbFactory.GetDatabase();
// 		var appUser = db.AppUsers.Find(User.GetUserId());
// 		return Data(_mapper.ViewModelMapper.Map<FeatureRegionViewModel>(db.PromisedFeatureRegions.Find(appUser.IdCountry)));
// 	}

// 	// [HttpGet]
// 	// [Route("All")]
// 	// public IActionResult GetAllRegions()
// 	// {
// 	// 	var db = _dbFactory.GetDatabase();
// 	// 	return Data(_mapper.ViewModelMapper.Map<FeatureRegionViewModel[]>(db.PromisedFeatureRegions.Where(f => f.IsActive == true).ToArray()));
// 	// }


// 	// [HttpGet]
// 	// [Route("Single")]
// 	// public IActionResult GetRegion(Guid regionId)
// 	// {
// 	// 	var db = _dbFactory.GetDatabase();
// 	// 	return Data(_mapper.ViewModelMapper.Map<PromisedFeatureRegion>(db.PromisedFeatureRegions.Find(regionId)));
// 	// }

// 	[HttpGet]
// 	[Route("Admin/All")]
// 	[RevokableAuthorize(Roles = Roles.AdminRoleName)]
// 	public IActionResult GetAllRegionsWithInactives(int page, int take)
// 	{
// 		var db = _dbFactory.GetDatabase();
// 		return Data(_mapper.ViewModelMapper.Map<PageResultSet<FeatureRegionViewModel>>(db.PromisedFeatureRegions.Order.By(f => f.RegionName).ForPagedResult(page, take)));
// 	}

// 	[HttpPost]
// 	[Route("Admin/Create")]
// 	[RevokableAuthorize(Roles = Roles.AdminRoleName)]
// 	public IActionResult CreateRegion(FeatureRegionViewModel region)
// 	{
// 		var db = _dbFactory.GetDatabase();
// 		region.PromisedFeatureRegionId = 0;
// 		var model = _mapper.ViewModelMapper.Map<PromisedFeatureRegion>(region);
// 		model = db.InsertWithSelect(model);
// 		return Data(_mapper.ViewModelMapper.Map<FeatureRegionViewModel>(model));
// 	}

// 	[HttpPost]
// 	[Route("Admin/Update")]
// 	[RevokableAuthorize(Roles = Roles.AdminRoleName)]
// 	public IActionResult UpdateRegion(FeatureRegionViewModel region)
// 	{
// 		var db = _dbFactory.GetDatabase();
// 		db.Update(_mapper.ViewModelMapper.Map<PromisedFeatureRegion>(region));
// 		return Data(region);
// 	}

// 	[HttpPost]
// 	[Route("Admin/Delete")]
// 	[RevokableAuthorize(Roles = Roles.AdminRoleName)]
// 	public IActionResult DeleteRegion(int region)
// 	{
// 		var db = _dbFactory.GetDatabase();
// 		var promisedFeatureRegion = db.PromisedFeatureRegions.Find(region);
// 		promisedFeatureRegion.IsActive = promisedFeatureRegion.IsActive;
// 		db.Update(promisedFeatureRegion);
// 		return Data(region);
// 	}
// }