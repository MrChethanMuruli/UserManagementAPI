using System.IdentityModel.Tokens.Jwt;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token) || !IsValidToken(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid or missing token.");
            return;
        }

        await _next(context);
    }

    private bool IsValidToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}
// This middleware checks for a valid JWT token in the Authorization header of incoming requests.