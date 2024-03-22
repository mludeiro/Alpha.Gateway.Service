using System.Net;
using System.Net.Http.Headers;
using Alpha.Common.TokenService;
using Microsoft.Extensions.Caching.Memory;
using Refit;

namespace Alpha.Gateway.Authentication;

public class AlphaAuthenticationDelegatinHandler( ITokenService tokenService, IMemoryCache memCache, 
    IHttpContextAccessor httpContextAccessor, ILogger<AlphaAuthenticationDelegatinHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = request.Headers.Authorization?.ToString();

        if( string.IsNullOrEmpty(authHeader) )
        {
            var mainRequest = httpContextAccessor?.HttpContext?.Request;
            if (mainRequest == null)
            {
                throw new InvalidOperationException("HTTP context or request not available.");
            }
            authHeader = mainRequest.Headers.Authorization.ToString();
            if( !string.IsNullOrEmpty(authHeader) )
                request.Headers.Add("Authorization", authHeader);
        }

        if( !string.IsNullOrEmpty(authHeader) )
        {
            var cacheKey = $"AlphaAuthenticationDelegatinHandler/token/{authHeader}";
            var cached = memCache.Get<ApiResponse<object>>(cacheKey);

            if(cached is null)
            {
                cached = await tokenService.CheckToken(authHeader);
                memCache.Set(cacheKey, cached, DateTimeOffset.Now.AddSeconds(60));
            }

            if( !cached.IsSuccessStatusCode )
            {
                logger.LogError("Authentication rejected by Token Service");

                return new HttpResponseMessage {
                    StatusCode = HttpStatusCode.Unauthorized,
                    RequestMessage = request
                };
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}