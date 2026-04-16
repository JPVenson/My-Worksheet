using System;
using System.Linq;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Payment;

[Route("api/PaymentInfoApi")]
[RevokableAuthorize]
public class PaymentInfoApiController : RestApiControllerBase<PaymentInfo, PaymentInfoModel>
{
    public PaymentInfoApiController(IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper) : base(dbContextFactory, mapper)
    {
    }

    [HttpGet]
    [Route("Get")]
    public IActionResult GetForUser()
    {
        using var db = EntitiesFactory.CreateDbContext();
        return Data(MapperService.ViewModelMapper.Map<PaymentInfoModel[]>(db.PaymentInfos
            .Where(e => e.IdAppUser == User.GetUserId())
            .ToArray()));
    }

    [HttpGet("GetSingle")]
    public override IActionResult GetSingle(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var info = db.PaymentInfos.Find(id);
        if (info == null || info.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(MapperService.ViewModelMapper.Map<PaymentInfoModel>(info));
    }

    [HttpGet]
    [Route("GetFields")]
    public IActionResult GetForUser(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var info = db.PaymentInfos.Find(id);
        if (info == null || info.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(MapperService.ViewModelMapper.Map<PaymentInfoContentModel[]>(db.PaymentInfoContents
            .Where(e => e.IdPaymentInfo == id)
            .ToArray()));
    }

    [HttpPost]
    [Route("Create")]
    public IActionResult CreatePaymentInfo([FromBody] UpdatePaymentInfoModel model)
    {
        var userId = User.GetUserId();
        using var db = EntitiesFactory.CreateDbContext();
        var typeExisits = db.PaymentInfos.Where(e => e.IdAppUser == userId && e.PaymentType == model.PaymentInfoModel.PaymentType).FirstOrDefault();

        if (typeExisits != null)
        {
            return BadRequest("PaymentType.Errors/MustBeUnique".AsTranslation());
        }

        using var transaction = db.Database.BeginTransaction();
        var info = MapperService.ViewModelMapper.Map<PaymentInfo>(model.PaymentInfoModel);
        info.IdAppUser = userId;
        info.PaymentInfoId = Guid.NewGuid();
        db.Add(info);
        foreach (var paymentInfoContentModel in model.ContentModels)
        {
            var content = MapperService.ViewModelMapper.Map<PaymentInfoContent>(paymentInfoContentModel.Entity);
            content.PaymentInfoContentId = Guid.NewGuid();
            content.IdPaymentInfo = info.PaymentInfoId;
            content.IdAppUser = userId;
            db.Add(content);
        }
        db.SaveChanges();
        transaction.Commit();
        return Data(MapperService.ViewModelMapper.Map<PaymentInfoModel>(info));
    }

    [HttpPost]
    [Route("Update")]
    public IActionResult UpdatePaymentInfos([FromBody] UpdatePaymentInfoModel model)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var info = db.PaymentInfos.Find(model.PaymentInfoModel.PaymentInfoId);
        if (info == null || info.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        var paymentInfoId = info.PaymentInfoId;
        MapperService.ViewModelMapper.Map<PaymentInfoModel, PaymentInfo>(model.PaymentInfoModel, info);

        using var transaction = db.Database.BeginTransaction();

        info.PaymentInfoId = paymentInfoId;
        info.IdAppUser = User.GetUserId();
        foreach (var modelContentModel in model.ContentModels)
        {
            if (modelContentModel.Type == EntityListState.Added)
            {
                var paymentInfoContent = MapperService.ViewModelMapper.Map<PaymentInfoContent>(modelContentModel.Entity);
                paymentInfoContent.IdAppUser = User.GetUserId();
                paymentInfoContent.IdPaymentInfo = info.PaymentInfoId;
                db.Add(paymentInfoContent);
            }
            else if (modelContentModel.Type == EntityListState.Deleted)
            {
                db.PaymentInfoContents.Where(e => e.PaymentInfoContentId == modelContentModel.Id).ExecuteDelete();
            }
            else
            {
                var paymentInfoContent = MapperService.ViewModelMapper.Map<PaymentInfoContent>(modelContentModel.Entity);
                paymentInfoContent.IdAppUser = User.GetUserId();
                paymentInfoContent.IdPaymentInfo = info.PaymentInfoId;
                db.Update(paymentInfoContent);
            }
        }

        db.SaveChanges();
        transaction.Commit();
        return Data(MapperService.ViewModelMapper.Map<PaymentInfoModel>(info));
    }
}