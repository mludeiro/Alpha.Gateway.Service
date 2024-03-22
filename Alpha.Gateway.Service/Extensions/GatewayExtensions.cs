using Refit;

namespace Alpha.Gateway.Extensions;

public static class MappingExtensions
{
    public static async Task<IResult> ToResult<T>(this Task<Refit.ApiResponse<T>> apiResponseTask)
    {
        var apiResponse = await apiResponseTask;

        if(apiResponse is null)
            return Results.StatusCode(StatusCodes.Status204NoContent);

        if(apiResponse.IsSuccessStatusCode)
            return Results.Ok(apiResponse.Content);

        return Results.StatusCode((int)apiResponse.StatusCode);
    }
}