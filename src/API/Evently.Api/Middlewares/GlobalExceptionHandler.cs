using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Evently.Api.Middlewares;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
       Log.Error(exception, "An unhandled exception occurred." );

       var problemDetails = new ProblemDetails
       {
           Status = StatusCodes.Status500InternalServerError,
           Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
           Title = "Server Error",
           Extensions = { { "TraceId", httpContext.TraceIdentifier } }
       };

       httpContext.Response.StatusCode = problemDetails.Status.Value;
       await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

       return true;
    }
}
