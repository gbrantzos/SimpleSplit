//using System.Diagnostics;
//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using SimpleSplit.Application.Base;

//namespace Split.Application.Base.Pipeline
//{
//    public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//        where TRequest : IRequest<TResponse>
//    {
//        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//        {
//            var sw = new Stopwatch();
//            sw.Start();

//            var result = await next();
//            sw.Stop();

//            var requestType = typeof(TRequest).Name;
//            SplitMetrics
//                .RequestsCounter
//                .WithLabels(requestType)
//                .Inc();
//            SplitMetrics
//                .RequestsDuration
//                .WithLabels(requestType)
//                .Observe(sw.Elapsed.TotalSeconds);

//            return result;
//        }
//    }
//}