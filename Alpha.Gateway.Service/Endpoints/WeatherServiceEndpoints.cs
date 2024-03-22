
using Alpha.Common.Identity;
using Alpha.Common.WeatherService;
using Alpha.Gateway.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Gateway.Endpoints;

internal static class WeatherServiceEndpoints
{
    public static void MapWeatherServiceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet ("/weather/api/weather", (IWeatherService isrv, HttpRequest request) => isrv.Weather() );
    }
}
