namespace Middleware;

public class TokenMiddleware
{
    private readonly RequestDelegate next;
    string pattern;

    public TokenMiddleware(RequestDelegate next, string pattern)
    {
        this.next = next;
        this.pattern = pattern;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Query.ContainsKey("token") && context.Request.Query["token"] == pattern)
        {
            await next.Invoke(context);
        }
        else
        {
            context.Response.StatusCode = 403;
        }
    }
}