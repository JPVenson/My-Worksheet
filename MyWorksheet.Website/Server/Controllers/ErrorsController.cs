using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ApiControllerBase
{
    [Route("error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context.Error; // Your exception
        var code = 500; // Internal Server Error by default
        Response.StatusCode = code; // You can use HttpStatusCode enum instead
        return new ContentResult()
        {
            StatusCode = code,
            Content = exception.ToString()
        };
    }
}
