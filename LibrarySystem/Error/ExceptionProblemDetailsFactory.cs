using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Error
{
    public static class ExceptionProblemDetailsFactory
    {
        public static ProblemDetails Create(Exception exception, HttpContext context, IHostEnvironment environment)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Something went wrong on the server.",
                Instance = context.Request.Path,
                Detail = environment.IsDevelopment() ? exception.Message : "Please try again later."
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            return problem;
        }
    }
}