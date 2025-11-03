using Demo.Invoices.API.Hosting.Security;
using ApplicationException = Demo.Invoices.API.Application.ApplicationException;

namespace Demo.Invoices.API.Hosting.Middleware;

public class ContextMiddleware
{
    public class ExecutionContext : IExecutionContext
    {
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        public string RequestId { get; set; } = string.Empty;

        public string CorrelationId { get; set; } = string.Empty;
    }

    private readonly RequestDelegate _next;

    public ContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// This is simple example of how hosting layer can initialize application context objects for all HTTP requests.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, UserContext userContext, ExecutionContext executionContext)
    {
        userContext.Authenticate(context.User);

        executionContext.CancellationToken = context.RequestAborted;

        if (context.Request.Headers.TryGetValue("x-request-id", out var values))
        {
            executionContext.RequestId = values.Single()!.ToString();
        }

        await _next(context);
    }
}