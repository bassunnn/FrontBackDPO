var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Use(async (context, next) => {
    Console.WriteLine($"Начало обработки: {DateTime.Now.ToLongTimeString()}");
    await next.Invoke(context);
    Console.WriteLine($"Конец обработки: {DateTime.Now.ToLongTimeString()}");
});

app.UseWhen(context => context.Request.Query.ContainsKey("trace") &&
 context.Request.Query["trace"] == "true", appBuilder => {
    appBuilder.Use(async (context, next) => {
        Console.WriteLine("trace == true");
        context.Response.Headers.Append("X-Debug-Trace", "true");
        await next.Invoke(context);
    });
});

app.MapWhen(context => context.Request.Query.ContainsKey("format") &&
 context.Request.Query["format"] == "plain", appBuilder =>
{
    appBuilder.Run(async (context) =>
    {
        context.Response.ContentType = "text/plain charset=utf-8";
        await context.Response.WriteAsync("Здесь используется MapWhen");
    });
});

app.Map("/tools", appBuilder =>
{
    appBuilder.Map("/time", timeBuilder =>
    {
        timeBuilder.Run(async (context) =>
        {
            await context.Response.WriteAsync($"Текущее время: {DateTime.Now.ToLongTimeString()}");
        });
    });

    appBuilder.Map("/date", dateBuilder =>
    {
        dateBuilder.Run(async (context) =>
        {
            await context.Response.WriteAsync($"Дата: {DateTime.Now.ToShortDateString()}");
        });
    });

    appBuilder.Map("/info", infoBuilder =>
    {
        infoBuilder.Run(async (context) =>
        {
            await context.Response.WriteAsync("Информация о сервере");
        });
    });
});



app.Run(async (context) => {
    await context.Response.WriteAsync("Hello");
});

app.Run();
