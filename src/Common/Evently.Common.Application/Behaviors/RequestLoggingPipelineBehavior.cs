using Evently.Common.Domain.Abstractions;
using MediatR;
using Serilog;
using Serilog.Context;

namespace Evently.Common.Application.Behaviors;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string moduleName = GetModuleName(typeof(TRequest).FullName!);
        string requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("Module", moduleName))
        {
            Log.Information("Processing request {RequestName}", requestName);
            
            TResponse response = await next();
            if (response.IsFailure)
            {
                using (LogContext.PushProperty("Error", response.Error, true))
                {
                    Log.Error("Completed request {RequestName} with error", requestName);
                }
            }
            else
            {
                Log.Information("Completed request {RequestName}", requestName);
            }
            
            return response;
        }
    }

    private string GetModuleName(string requestClassName) => requestClassName.Split('.')[2];
}
