using System;
using Microsoft.AspNetCore.Authorization;

namespace MyWorksheet.Shared.WebApi;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RevokableAuthorizeAttribute : AuthorizeAttribute
{
    //public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    //{
    //	if (!actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
    //	{
    //		if (!(actionContext.RequestContext.Principal?.Identity is ClaimsIdentity claimsIdentity))
    //		{
    //			this.HandleUnauthorizedRequest(actionContext);
    //			actionContext.Response.StatusCode = HttpStatusCode.Unauthorized;
    //			return;
    //		}

    //		if (!claimsIdentity.IsAuthenticated)
    //		{
    //			this.HandleUnauthorizedRequest(actionContext);
    //			actionContext.Response.StatusCode = HttpStatusCode.Forbidden;
    //			return;
    //		}

    //		var tokenManager = IoC.Resolve<TokenManager>();
    //		if (!tokenManager.AuthIdentity(claimsIdentity))
    //		{
    //			this.HandleUnauthorizedRequest(actionContext);
    //			actionContext.Response.StatusCode = HttpStatusCode.Forbidden;
    //			return;
    //		}
    //	}

    //	await base.OnAuthorizationAsync(actionContext, cancellationToken);
    //}
}