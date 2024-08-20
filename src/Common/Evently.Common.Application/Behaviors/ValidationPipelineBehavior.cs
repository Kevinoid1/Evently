using System.Reflection;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Evently.Common.Application.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
       ValidationFailure[] validationFailures = await ValidateRequestAsync(request);

       if (validationFailures.Length == 0)
       {
           return await next();
       }

       if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
       {
           Type resultType = typeof(TResponse).GetGenericArguments()[0];

           // TResponse result = (TResponse)Activator.CreateInstance(typeof(TResponse).GetGenericTypeDefinition().MakeGenericType(resultType))!
           MethodInfo? failureMethod =
               typeof(Result<>).MakeGenericType(resultType).GetMethod(nameof(Result<object>.ValidationFailure));

           if (failureMethod is not null)
           {
               // first parameter is null since it is a static method
               return (TResponse)failureMethod.Invoke(null, [CreateValidationError(validationFailures)]);
           }
       }
       else if (typeof(TResponse) == typeof(Result))
       {
           return (TResponse)Result.Failure(CreateValidationError(validationFailures));
       }

       throw new ValidationException(validationFailures);
    }
    
    private async Task<ValidationFailure[]> ValidateRequestAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return [];
        }
        
        var context = new ValidationContext<TRequest>(request);
        ValidationResult[] validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context)));
        
        ValidationFailure[] validationFailures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToArray();

        return validationFailures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new(validationFailures.Select(vf => Error.Problem(vf.ErrorCode,
                vf.ErrorMessage))
            .ToArray());

}
