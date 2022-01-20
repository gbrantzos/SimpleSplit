using FluentValidation;
using MediatR;
using SimpleSplit.Application.Base;

namespace SimpleSplit.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : Request<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults =
                await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(e => e.ErrorMessage)
                .ToList();
            if (failures.Count != 0)
            {
                var messages = new[] { "Validation Errors" }.Concat(failures);
                return Result.FromError<TResponse>(messages);
            }

            return await next();
        }
    }
}