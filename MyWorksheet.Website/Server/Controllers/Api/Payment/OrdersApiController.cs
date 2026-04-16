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

[Route("api/OrdersApi")]
[RevokableAuthorize]
public class OrdersApiControllerBase : ApiControllerBase
{
    private readonly IMapperService _mapper;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;

    public OrdersApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    [HttpGet]
    [Route("Admin/All")]
    public IActionResult GetAllOrdersForUser(Guid userId)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var orders = db.PaymentOrders.Where(f => f.IdAppUser == userId).ToArray();
        return Data(_mapper.ViewModelMapper.Map<GetOrder[]>(orders));
    }

    [RevokableAuthorize]
    [HttpGet]
    [Route("All")]
    public IActionResult GetAllOrders()
    {
        return GetAllOrdersForUser(User.GetUserId());
    }

    [Route("RedeemFeature")]
    [HttpPost]
    public IActionResult PlaceFeatureOrder(int feature, int paymentProvider)
    {
        return NotFound();
    }
}