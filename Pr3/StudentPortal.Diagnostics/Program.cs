var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Use(async (context, next) => {
    Console.WriteLine($"Начало обработки: {DateTime.Now.ToLongTimeString()}");
    await next.Invoke(context);
    Console.WriteLine($"Конец обработки: {DateTime.Now.ToLongTimeString()}");
});

app.UseWhen(context.Request.Path == "/tools/time", appBuilder => {
    appBuilder.Use(async (context, next) => {
        if (context.Request.Query["trace"] == "true")
        {
            Console.WriteLine("trace == true");
            await next.Invoke(context);
        }
        else
        {
            context.Response.StatusCode = 403;
        }
    });
});

app.Run(async (context) => {
    await context.Response.WriteAsync("Hello");
});

app.Run();
