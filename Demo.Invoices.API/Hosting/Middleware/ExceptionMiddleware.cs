using ApplicationException = Demo.Invoices.API.Application.ApplicationException;

namespace Demo.Invoices.API.Hosting.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApplicationException aex)
        {
            await HandleException(context, aex.Message, StatusCodes.Status400BadRequest);
        }
        catch
        {
            await HandleException(context, "An unexpected error occurred.", StatusCodes.Status500InternalServerError);
        }
    }

    private async Task HandleException(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Code = statusCode,
            Error = message
        }));
    }
}