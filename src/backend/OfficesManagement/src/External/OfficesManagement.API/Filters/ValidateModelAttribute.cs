using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
namespace OfficesManagement.API.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments)
        {
            var argumentValue = argument.Value;
            if (argumentValue == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentValue.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator == null) continue;

            var validationContext = new ValidationContext<object>(argumentValue);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });

                return;
            }
        }

        await next();
    }
}
