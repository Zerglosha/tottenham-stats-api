using System.ComponentModel.DataAnnotations;

namespace TottenhamStatsAPI.Filters;

public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is null) return Results.BadRequest();

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);

        var isValid = Validator.TryValidateObject(
            request,
            validationContext,
            validationResults,
            true);

        if (isValid) return await next(context);

        var errors = validationResults
            .SelectMany(result =>
                result.MemberNames.Select(memberName => new
                {
                    MemberName = memberName,
                    ErrorMessage = result.ErrorMessage ?? "Invalid value"
                }))
            .GroupBy(error => error.MemberName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        return Results.ValidationProblem(errors);
    }
}