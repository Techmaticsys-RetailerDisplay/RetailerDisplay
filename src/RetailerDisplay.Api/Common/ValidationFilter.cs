using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Common;

/// <summary>Runs any registered FluentValidation validator for action arguments before the action executes.</summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _services;

    public ValidationFilter(IServiceProvider services) => _services = services;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_services.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);
                if (!result.IsValid)
                {
                    var message = string.Join(" ", result.Errors.Select(e => e.ErrorMessage));
                    throw new AppException(message, 400);
                }
            }
        }

        await next();
    }
}
