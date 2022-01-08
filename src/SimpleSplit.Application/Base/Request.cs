using MediatR;

namespace SimpleSplit.Application.Base
{
    /// <summary>
    /// Base class for commands/queries. Describes a request that returns <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned</typeparam>
    public abstract class Request<TResult> : IRequest<Result<TResult>> { }

    /// <summary>
    /// Simplified request with no result.
    /// </summary>
    public abstract class Request : Request<bool> { }
}
