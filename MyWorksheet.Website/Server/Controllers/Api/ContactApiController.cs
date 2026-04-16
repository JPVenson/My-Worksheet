using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using MyWorksheet.Website.Server.Helper;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Google.ReCapture2;
using MyWorksheet.Website.Server.Services.Mail;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Server.Util.Auth;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Controllers.Api;

[Route("Api/ContactApi")]
public class ContactApiControllerBase : ApiControllerBase
{
    private readonly IMailServiceProvider _mailService;
    private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
    private readonly IMapperService _mapper;
    private readonly IOptions<MailSettings> _mailSettings;
    private readonly ActivatorService _activatorService;

    public ContactApiControllerBase(IMailServiceProvider mailService, IDbContextFactory<MyworksheetContext> dbContextFactory, IMapperService mapper, IOptions<MailSettings> mailSettings,
        ActivatorService activatorService)
    {
        _mailService = mailService;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _mailSettings = mailSettings;
        _activatorService = activatorService;
    }

    [RevokableAuthorize(Roles = Roles.AdminRoleName)]
    public IActionResult Get(int page = 1, int take = 25,
        string name = null)
    {
        using var db = _dbContextFactory.CreateDbContext();
        if (string.IsNullOrEmpty(name))
        {
            return Data(_mapper.ViewModelMapper.Map<PageResultSet<ContactEntry>>(db.ContactEntries
                .OrderBy(f => f.ContactEntryId)
                .ForPagedResult(page, take)));
        }

        var q = db.ContactEntries.Where(e => e.Email.Contains(name) || e.Message.Contains(name) || e.Name.Contains(name)).OrderBy(e => e.ContactEntryId);

        var pages = q.ForPagedResult(page, take);

        return Data(_mapper.ViewModelMapper.Map<PageResultSet<ContactEntry>>(pages));
    }

    public string[] ContactTypes { get; set; } =
    {
        "General Request",
        "Password Request",
        "Bug Report",
        "Test Account"
    };

    [HttpGet]
    [Route("ContactTypes")]
    public IActionResult GetContactTypes()
    {
        return Data(ContactTypes);
    }

    [HttpPost]
    [Route("Contact")]
    public async Task<IActionResult> Post(PostContactApiModel contactMessage)
    {
        if (!ContactTypes.Contains(contactMessage.ContactType))
        {
            return BadRequest("The selected Contact type is Invalid");
        }

        var mess = GoogleReCapcha.Validate(contactMessage.RecaptureMessage,
            Request.HttpContext.Connection.RemoteIpAddress.ToString());

        if (!mess)
        {
            return BadRequest("Common/GoogleVertficationFailed".AsTranslation());
        }

        if (User?.Identity?.Name != null)
        {
            contactMessage.Name = User.Identity.Name;
        }

        var entity = _mapper.ViewModelMapper.Map<ContactEntry>(contactMessage);
        entity.SenderIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
        using var db = _dbContextFactory.CreateDbContext();
        db.Add(entity);
        db.SaveChanges();
        var mail = _activatorService.ActivateType<ContactRequestMail>(entity.Name, entity.Email, entity.Message, entity.ContactType);
        var mailAck =
            _activatorService.ActivateType<OutgoingContactRequestAckMail>(entity.Name, entity.Email, entity.Message, entity.ContactType);
        var later = _mailService.ApplicationMailService.SendMail(mail, User.GetUserId(),
            _mailSettings.Value.Recive.MainMailAddress);
        await _mailService.ApplicationMailService.SendMail(mailAck, User.GetUserId(), entity.Email);
        await later;

        return Data();
    }
}