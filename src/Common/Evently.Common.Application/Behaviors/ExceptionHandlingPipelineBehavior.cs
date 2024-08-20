using Evently.Common.Application.Exceptions;
using Evently.Common.Domain.Abstractions;
using MediatR;
using Serilog;

namespace Evently.Common.Application.Behaviors;

internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest:class
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception for {RequestName}", typeof(TRequest).Name);
            throw new EventlyException(typeof(TRequest).Name, innerException: ex);
        }
    }
}
