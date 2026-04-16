using MyWorksheet.Shared.WebApi;
using MyWorksheet.Webpage.Helper.Roles;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers.Api.Administration;

[ApiController]
[RevokableAuthorize(Roles = Roles.AdminRoleName)]
[Route("api/ServerInfoApi")]
public class ServerInfoApiControllerBase : ApiControllerBase
{
    public ServerInfoApiControllerBase()
    {

    }
}