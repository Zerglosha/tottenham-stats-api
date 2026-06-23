namespace TottenhamStatsAPI.Helpers;

public static class ApiErrors
{
    public static IResult NotFound(string resourceName, int id)
    {
        return Results.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: $"{resourceName} not found",
            detail: $"{resourceName} with id {id} was not found.");
    }
}