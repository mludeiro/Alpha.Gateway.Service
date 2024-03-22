
using Alpha.Common.Identity;
using Alpha.Gateway.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Gateway.Endpoints;

internal static class IdentityServiceEndpoints
{
    public static void MapIdentityServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var account = app.MapGroup("/identity/api/account");
        account.MapPost("/login", (IIdentityService isrv, AccountLogin al) => isrv.Login(al).ToResult() );
        account.MapPost("/register", (IIdentityService isrv, AccountRegister ar) => isrv.Register(ar).ToResult());
        account.MapGet ("/me", (IIdentityService isrv, HttpRequest request) => isrv.Me(request.Headers.Authorization!).ToResult() );
    }
}