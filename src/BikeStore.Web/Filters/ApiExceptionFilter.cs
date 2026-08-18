using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BikeStore.Web.Filters;

public sealed class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ApiException exception) return;

        context.ExceptionHandled = true;
        context.Result = new RedirectToActionResult(
            "Error",
            "Home",
            new { message = exception.Message });
    }
}
