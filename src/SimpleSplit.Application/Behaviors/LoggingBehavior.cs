using System.Diagnostics;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using SimpleSplit.Application.Base;
using SimpleSplit.Common;

namespace SimpleSplit.Application.Behaviors
{
    // We are using "complex" open generic pipeline behaviors, so we need to use Autofac!!
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : Request<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogDebug("[{RequestName:l}] Executing =>\r\n---> R E Q U E S T <---\r\n{@Request}\r\n-----------------------",
                requestName,
                request);

            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var response = await next();
                sw.Stop();

                // TODO Review logic!!!
                if (response is Result responseResult)
                {
                    if (responseResult.HasErrors)
                    {
                        var formattedMessage = AddIndent(response.Errors.ToArray());
                        _logger.LogError(
                            $"[{requestName}] Request result has errors, ({sw.ElapsedMilliseconds}ms)\r\n{formattedMessage}");
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"[{requestName}] Executed successfully, ({sw.ElapsedMilliseconds}ms)");
                    }
                }
                else
                {
                    // No details to display
                    _logger.LogInformation($"Request executed, {requestName}: {request} [{sw.ElapsedMilliseconds}ms]");
                }
                return response;
            }
            catch (Exception x)
            {
                var all     = x.GetAllMessages();
                var message = String.Join(Environment.NewLine, all);
                _logger.LogError(x, "Request execution failed: {Message}\r\n{@Request}", message, request);

                throw;
            }
        }

        private static string AddIndent(string[] messages)
        {
            var sb = new StringBuilder();
            foreach (var line in messages.Where(l => !String.IsNullOrEmpty(l)))
                sb.AppendLine($"    {line}");

            return sb.ToString().Trim('\r', '\n');
        }
    }
}