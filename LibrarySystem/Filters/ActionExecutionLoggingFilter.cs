using Microsoft.AspNetCore.Mvc.Filters;

namespace LibrarySystem.Filters
{
    public class ActionExecutionLoggingFilter(ILogger<ActionExecutionLoggingFilter> logger) : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Before action {Action} with args {@Args}",
                context.ActionDescriptor.DisplayName, context.ActionArguments);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("After action {Action}", context.ActionDescriptor.DisplayName);
        }
    }
}