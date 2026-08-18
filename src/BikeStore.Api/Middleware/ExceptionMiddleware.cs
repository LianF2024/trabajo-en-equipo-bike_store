using BikeStore.Application.Common;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception exception)
        {
            var status = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                BusinessException => StatusCodes.Status400BadRequest,
                SqlException => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };
            if (status == StatusCodes.Status500InternalServerError) logger.LogError(exception, "Error no controlado. TraceId: {TraceId}", context.TraceIdentifier);
            else logger.LogWarning(exception, "Solicitud rechazada. TraceId: {TraceId}", context.TraceIdentifier);

            var detail = exception is SqlException
                ? "No fue posible conectar con SQL Server. Revise el nombre de la instancia y la cadena BikeStore en appsettings.json."
                : status == StatusCodes.Status500InternalServerError
                    ? "Use el identificador de seguimiento para solicitar soporte."
                    : exception.Message;

            var title = exception switch
            {
                SqlException => "SQL Server no disponible.",
                _ when status == StatusCodes.Status500InternalServerError => "Ocurrió un error interno.",
                _ => exception.Message
            };

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = context.TraceIdentifier }
            });
        }
    }
}
