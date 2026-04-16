using System.Linq;
using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Payment;

[ApiController]
[Route("api/FeatureApi")]
public class FeatureApiControllerBase : ApiControllerBase
{
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;

    public FeatureApiControllerBase(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    [Route("Products")]
    [HttpGet]
    public IActionResult GetAllFeatures(Guid region)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var features = db.PromisedFeatureContents
            .Where(f => f.IsActive && f.IdPromisedFeatureRegion == region)
            .ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetFeature[]>(features));
    }

    [Route("PaymentProvider")]
    [HttpGet]
    public IActionResult GetAllPaymentProvider(Guid region)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var regionMaps = db.PaymentProviderForRegionMaps
            .Where(f => f.RegionId == region)
            .ToArray();
        if (!regionMaps.Any())
        {
            return Data(new PaymentProviderViewModel[0]);
        }

        var providerIds = regionMaps.Select(f => f.IdPaymentProvider).ToArray();
        var providers = db.PaymentProviders
            .Where(f => providerIds.Contains(f.PaymentProviderId))
            .ToArray();
        return Data(_mapper.ViewModelMapper.Map<PaymentProviderViewModel[]>(providers));
    }

    [Route("Admin/PaymentProvider")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAllPaymentProviderAdmin()
    {
        using var db = _dbContextFactory.CreateDbContext();
        return Data(_mapper.ViewModelMapper.Map<PaymentProviderViewModel[]>(db.PaymentProviders.ToArray()));
    }

    [Route("Admin/PaymentProvider/Country/Link")]
    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult CreatePaymentProviderLink(Guid paymentProvider, Guid countryId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var link = new PaymentProviderForRegionMap
        {
            IdPaymentProvider = paymentProvider,
            RegionId = countryId
        };
        db.PaymentProviderForRegionMaps.Add(link);
        db.SaveChanges();
        return Data(link);
    }

    [Route("Admin/PaymentProvider/Country/UnLink")]
    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult RemovePaymentProviderLink(Guid paymentProvider, Guid countryId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        db.PaymentProviderForRegionMaps
            .Where(f => f.IdPaymentProvider == paymentProvider && f.RegionId == countryId)
            .ExecuteDelete();
        return Data();
    }

    [Route("Admin/Products")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAllFeaturesAdmin(Guid region)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var query = db.PromisedFeatureContents.AsQueryable();
        if (region != Guid.Empty)
        {
            query = query.Where(f => f.IdPromisedFeatureRegion == region);
        }
        return Data(_mapper.ViewModelMapper.Map<PostFeature[]>(query.ToArray()));
    }

    [Route("ProductsMeta")]
    [HttpGet]
    public IActionResult GetPublicAllFeaturesMetaData()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var features = db.PromisedFeatures.ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetFeatureMetaPublic[]>(features));
    }

    [Route("Admin/ProductsMeta")]
    [HttpGet]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult GetAllFeaturesMetaData()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var features = db.PromisedFeatures.ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetFeatureMeta[]>(features));
    }

    [Route("Admin/CreateProducts")]
    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult LinkFeature(PostFeature feature, Guid featureRegion, Guid featureId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var featurePoco = _mapper.ViewModelMapper.Map<PromisedFeatureContent>(feature);
        featurePoco.IdPromisedFeatureRegion = featureRegion;
        featurePoco.IdPromisedFeature = featureId;
        db.PromisedFeatureContents.Add(featurePoco);
        db.SaveChanges();
        return Data(_mapper.ViewModelMapper.Map<PostFeature>(featurePoco));
    }

    [Route("Admin/UpdateProducts")]
    [HttpPost]
    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult UpdateFeature(PostFeature feature)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var featurePoco = _mapper.ViewModelMapper.Map<PromisedFeatureContent>(feature);
        db.PromisedFeatureContents.Update(featurePoco);
        db.SaveChanges();
        return Data(_mapper.ViewModelMapper.Map<PostFeature>(featurePoco));
    }

    [Route("UserProducts")]
    [RevokableAuthorize]
    [HttpGet]
    public IActionResult UserProducts()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var featuresMap = db.PromisedFeatureToAppUserMaps
            .Where(f => f.IdAppUser == userId)
            .ToArray();
        var hasFeatures = featuresMap.Select(f => f.IdFeature).ToArray();
        if (!hasFeatures.Any())
        {
            return Data(new GetFeature[0]);
        }

        var features = db.PromisedFeatureContents
            .Where(f => hasFeatures.Contains(f.IdPromisedFeature))
            .ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetFeature[]>(features));
    }

    [Route("UserProducts/Aggregated")]
    [RevokableAuthorize]
    [HttpGet]
    public IActionResult UserProductsAggregated()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var userId = User.GetUserId();
        var aggregated = db.PaymentOrders
            .Where(f => f.IdAppUser == userId && f.IsOrderSuccess && f.IsOrderDone)
            .GroupBy(f => f.IdPromisedFeatureContent)
            .Select(g => new FeatureOverviewAggregatedFeatures
            {
                IdPromisedFeatureContent = g.Key,
                Ammount = g.Count()
            })
            .ToArray();
        return Data(aggregated);
    }
}