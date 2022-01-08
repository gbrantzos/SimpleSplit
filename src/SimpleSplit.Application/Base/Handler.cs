using MediatR;
using Microsoft.Extensions.Logging;

namespace SimpleSplit.Application.Base
{
    public abstract class Handler<TRequest, TResult> : IRequestHandler<TRequest, Result<TResult>>
        where TRequest : Request<TResult>
    {
        private readonly ILogger _logger;
        private List<string> _errorMessages = new List<string>();

        protected Handler(ILogger logger = null) => _logger = logger;

        public async Task<Result<TResult>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await HandleCore(request, cancellationToken);
                if (_errorMessages.Count >= 1)
                    return Result.FromError<TResult>(_errorMessages);

                return result;
            }
            catch (Exception x)
            {
                _logger?.LogWarning(x, $"Unhandled exception in {GetType().Name}");
                return Result.FromException<TResult>(x);
            }
        }

        protected abstract Task<TResult> HandleCore(TRequest request, CancellationToken cancellationToken);

        protected Task<TResult> Failure(string message) => Failure(new string[] { message });
        protected Task<TResult> Failure(string[] messages)
        {
            _errorMessages = messages.ToList();
            return Task.FromResult<TResult>(default);
        }
    }
}
