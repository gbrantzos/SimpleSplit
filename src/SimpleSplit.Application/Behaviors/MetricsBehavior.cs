using System.Diagnostics;
using MediatR;

namespace SimpleSplit.Application.Behaviors
{
    public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = await next();
            sw.Stop();

            var requestType = typeof(TRequest).Name;
            SimpleSplitMetrics
                .RequestsCounter
                .WithLabels(requestType)
                .Inc();
            SimpleSplitMetrics
                .RequestsDuration
                .WithLabels(requestType)
                .Observe(sw.Elapsed.TotalSeconds);

            return result;
        }
    }
}