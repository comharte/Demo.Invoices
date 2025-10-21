namespace Demo.Invoices.API.Hosting.Middleware;

public class TeapotMiddleware
{
    private readonly RequestDelegate _next;

    public TeapotMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 418;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new TeapotResponse()));
    }
}

public class TeapotResponse
{
    public string Message { get; set; } = "Server refuses to brew coffee because it is, permanently, a teapot.";

    public string NumberOfTeapotsLeft { get; set; } = "42";
}