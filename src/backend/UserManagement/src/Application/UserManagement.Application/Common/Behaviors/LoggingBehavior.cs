using MediatR;
using Microsoft.Extensions.Logging;
namespace UserManagement.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request: {RequestType} - {Request}", typeof(TRequest).Name, request);

        var response = await next();

        _logger.LogInformation("Handled request: {RequestType} - {Response}", typeof(TRequest).Name, response);

        return response;
    }

}
