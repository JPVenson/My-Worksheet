using System.Linq;
using System;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Util;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[ApiController]
[RevokableAuthorize()]
[Route("api/EMailAccountApi")]
public class EMailAccountApiControllerBase : RestApiControllerBase<MailAccount, EMailAccountViewModel, EMailAccountViewModel>
{
    private readonly IBlobManagerService _blobManagerService;

    public EMailAccountApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> contextFactory,
        IBlobManagerService blobManagerService) : base(contextFactory, mapper)
    {
        _blobManagerService = blobManagerService;
    }

    [HttpGet]
    [Route("GetHistory")]
    public IActionResult GetHistory(Guid mailAccountId)
    {
        using var db = EntitiesFactory.CreateDbContext();
        var mailAccount = db.MailAccounts.Find(mailAccountId);
        if (mailAccount == null || mailAccount.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        return Data(MapperService.ViewModelMapper.Map<MailSendViewModel[]>(db.MailSends.Where(e => e.IdMailAccount == mailAccountId)));
    }

    [HttpPost]
    [Route("Create")]
    public override ValueTask<IActionResult> Create([FromBody] EMailAccountViewModel model)
    {
        using var db = base.EntitiesFactory.CreateDbContext();
        var mailAccount = MapperService.ViewModelMapper.Map<MailAccount>(model);
        mailAccount.IdAppUser = User.GetUserId();
        mailAccount.Password = ChallangeUtil.EncryptPassword(model.Password, User.Identity.Name);
        db.MailAccounts.Add(mailAccount);
        db.SaveChanges();
        return ValueTask.FromResult(Data(MapperService.ViewModelMapper.Map<EMailAccountViewModel>(mailAccount)));
    }

    [HttpPost]
    [Route("Update")]
    public override async ValueTask<IActionResult> Update([FromBody] EMailAccountViewModel model, Guid id)
    {
        using var db = base.EntitiesFactory.CreateDbContext();
        var existingAccount = db.MailAccounts.Find(model.MailAccountId);
        if (existingAccount == null || existingAccount.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        if (model.Password != existingAccount.Password)
        {
            model.Password = ChallangeUtil.EncryptPassword(model.Password, User.Identity.Name);
        }

        var mailAccount = MapperService.ViewModelMapper.Map(model, existingAccount);
        mailAccount.IdAppUser = User.GetUserId();

        foreach (var mailSend in db.MailSends.Where(e => e.IdMailAccount == existingAccount.MailAccountId).ToArray())
        {
            await _blobManagerService.DeleteData(mailSend.IdContent, User.GetUserId());
            db.StorageEntries.Where(e => e.StorageEntryId == mailSend.IdContent).ExecuteDelete();
            if (mailSend.IdAttachment.HasValue)
            {
                await _blobManagerService.DeleteData(mailSend.IdAttachment.Value, User.GetUserId());
                db.StorageEntries.Where(e => e.StorageEntryId == mailSend.IdAttachment.Value).ExecuteDelete();
            }
        }

        db.Update(mailAccount);
        db.SaveChanges();
        return Data(MapperService.ViewModelMapper.Map<EMailAccountViewModel>(mailAccount));
    }

    [HttpPost]
    [Route("Delete")]
    public async Task<IActionResult> Delete(Guid mailAccountId)
    {
        using var db = base.EntitiesFactory.CreateDbContext();
        var existingAccount = db.MailAccounts.Find(mailAccountId);
        if (existingAccount == null || existingAccount.IdAppUser != User.GetUserId())
        {
            return Unauthorized("Common/InvalidId".AsTranslation());
        }

        foreach (var mailSend in db.MailSends.Where(f => f.IdMailAccount == existingAccount.MailAccountId).ToArray())
        {
            await _blobManagerService.DeleteData(mailSend.IdContent, User.GetUserId());
            db.StorageEntries.Where(e => e.StorageEntryId == mailSend.IdContent).ExecuteDelete();
            if (mailSend.IdAttachment.HasValue)
            {
                await _blobManagerService.DeleteData(mailSend.IdAttachment.Value, User.GetUserId());
                db.StorageEntries.Where(e => e.StorageEntryId == mailSend.IdAttachment.Value).ExecuteDelete();
            }
        }

        db.MailAccounts.Where(e => e.MailAccountId == mailAccountId).ExecuteDelete();
        db.SaveChanges();
        return Data();
    }
}