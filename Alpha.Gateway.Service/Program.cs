using Alpha.Tools.Consul;
using Alpha.Common.Identity;
using Alpha.Common.TokenService;
using Alpha.Common.WeatherService;
using Alpha.Gateway.Authentication;
using Alpha.Gateway.Endpoints;
using Refit;
using Alpha.Gateway.Logging;
using Dapr.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<AlphaAuthenticationDelegatinHandler>();
        builder.Services.AddTransient<HttpLoggingHandler>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddRefitClient<ITokenService>()
            .ConfigureHttpClient(options => { options.BaseAddress = new Uri($"http://token"); })
            .AddHttpMessageHandler(_ => new InvocationHandler());

        builder.Services.AddRefitClient<IIdentityService>()
            .ConfigureHttpClient(options => { options.BaseAddress = new Uri($"http://identity"); })
            .AddHttpMessageHandler(_ => new InvocationHandler())
            .AddHttpMessageHandler<AlphaAuthenticationDelegatinHandler>();

        builder.Services.AddRefitClient<IWeatherService>()
            .ConfigureHttpClient(options => { options.BaseAddress = new Uri($"http://weather"); })
            .AddHttpMessageHandler(_ => new InvocationHandler())
            .AddHttpMessageHandler<AlphaAuthenticationDelegatinHandler>();


        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.MapGet("/", () => "Alpha Gateway Service");
        app.MapIdentityServiceEndpoints();
        app.MapWeatherServiceEndpoints();

        // Token service should not be acceded from out of the could !!!

        app.MapHealthChecks("/health");
        app.Run();
    }
}