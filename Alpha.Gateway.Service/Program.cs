using Alpha.Tools.Consul;
using Alpha.Common.Identity;
using Alpha.Common.TokenService;
using Alpha.Common.WeatherService;
using Alpha.Gateway.Authentication;
using Alpha.Gateway.Endpoints;
using Refit;
using Alpha.Gateway.Logging;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<AlphaAuthenticationDelegatinHandler>();
        builder.Services.AddTransient<ConsulRegistryHandler>();
        builder.Services.AddTransient<HttpLoggingHandler>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddRefitClient<ITokenService>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://token.service:8080"))
            .AddHttpMessageHandler<ConsulRegistryHandler>();

        builder.Services.AddRefitClient<IIdentityService>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://identity.service:8080"))
            .AddHttpMessageHandler<ConsulRegistryHandler>()
            .AddHttpMessageHandler<AlphaAuthenticationDelegatinHandler>();

        builder.Services.AddRefitClient<IWeatherService>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://weather.service:8080"))
            .AddHttpMessageHandler<ConsulRegistryHandler>()
            .AddHttpMessageHandler<AlphaAuthenticationDelegatinHandler>();

        builder.Services.ConsulServicesConfig(builder.Configuration.GetSection("Consul").Get<ConsulConfig>()!);

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