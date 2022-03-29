using MediatR;
using Microsoft.Extensions.Logging;

namespace SimpleSplit.Application.Base
{
    public abstract class Handler<TRequest, TResult> : IRequestHandler<TRequest, Result<TResult>>
        where TRequest : Request<TResult>
    {
        private readonly ILogger _logger;
        private List<string> _errorMessages = new();

        protected Handler(ILogger logger = null) => _logger = logger;

        public async Task<Result<TResult>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await HandleCore(request, cancellationToken);
                return _errorMessages.Count > 0 ? Result.FromError<TResult>(_errorMessages) : result;
            }
            catch (Exception x)
            {
                _logger?.LogWarning(x, $"Unhandled exception in {GetType().Name}");
                return Result.FromException<TResult>(x);
            }
        }

        protected abstract Task<TResult> HandleCore(TRequest request, CancellationToken cancellationToken);

        protected Task<TResult> Failure(string message) => Failure(new[] { message });
        protected Task<TResult> Failure(IEnumerable<string> messages)
        {
            _errorMessages = messages.ToList();
            return Task.FromResult<TResult>(default);
        }
    }

    public abstract class Handler<TRequest> : Handler<TRequest, bool>
        where TRequest : Request<bool>
    {
        protected Handler(ILogger logger = null) : base(logger) { }
    }
}
