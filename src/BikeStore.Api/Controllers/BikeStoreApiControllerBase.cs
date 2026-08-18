using BikeStore.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BikeStore.Api.Controllers;

public abstract class BikeStoreApiControllerBase : ControllerBase
{
    protected async Task<ActionResult<T>> ExecuteAsync<T>(
        Func<Task<T>> operation,
        Func<T, ActionResult<T>> success)
    {
        try
        {
            return success(await operation());
        }
        catch (NotFoundException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status404NotFound);
        }
        catch (ConflictException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status409Conflict);
        }
        catch (BusinessException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status400BadRequest);
        }
        catch (SqlException)
        {
            return SqlServerProblem();
        }
    }

    protected async Task<IActionResult> ExecuteAsync(
        Func<Task> operation,
        Func<IActionResult> success)
    {
        try
        {
            await operation();
            return success();
        }
        catch (NotFoundException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status404NotFound);
        }
        catch (ConflictException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status409Conflict);
        }
        catch (BusinessException exception)
        {
            return ExpectedProblem(exception, StatusCodes.Status400BadRequest);
        }
        catch (SqlException)
        {
            return SqlServerProblem();
        }
    }

    private ObjectResult ExpectedProblem(Exception exception, int statusCode)
        => ProblemResult(statusCode, exception.Message, exception.Message);

    private ObjectResult SqlServerProblem()
        => ProblemResult(
            StatusCodes.Status503ServiceUnavailable,
            "SQL Server no disponible.",
            "No fue posible conectar con SQL Server. Revise el nombre de la instancia y la cadena BikeStore en appsettings.json.");

    private ObjectResult ProblemResult(int statusCode, string title, string detail)
        => StatusCode(statusCode, new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = HttpContext.Request.Path,
            Extensions = { ["traceId"] = HttpContext.TraceIdentifier }
        });
}
