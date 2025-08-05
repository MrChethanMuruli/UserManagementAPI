using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Log request
        Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        stopwatch.Stop();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        Console.WriteLine($"Outgoing Response: {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)");

        await responseBody.CopyToAsync(originalBodyStream);
    }
}
