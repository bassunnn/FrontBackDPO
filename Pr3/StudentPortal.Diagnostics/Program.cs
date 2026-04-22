using System.Text;
using Middleware;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// регистрация сервисов
builder.Services.AddStudentPortalServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ErrorHandlingMiddleware должен быть первым
app.UseMiddleware<ErrorHandlingMiddleware>();

// Inline middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"Начало обработки: {DateTime.Now.ToLongTimeString()}");

    await next.Invoke(context);

    Console.WriteLine($"Конец обработки: {DateTime.Now.ToLongTimeString()}");
});

// UseWhen
app.UseWhen(context =>
    context.Request.Query.ContainsKey("trace") &&
    context.Request.Query["trace"] == "true",
    appBuilder =>
    {
        appBuilder.Use(async (context, next) =>
        {
            Console.WriteLine("trace == true");

            context.Response.Headers.Append("X-Debug-Trace", "true");

            await next.Invoke(context);
        });
    });

// MapWhen
app.MapWhen(context =>
    context.Request.Query.ContainsKey("format") &&
    context.Request.Query["format"] == "plain",
    appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            await context.Response.WriteAsync("Здесь используется MapWhen");
        });
    });

// ветка tools
app.Map("/tools", appBuilder =>
{
    appBuilder.Map("/time", timeBuilder =>
    {
        timeBuilder.Run(async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            var service = context.RequestServices.GetRequiredService<IDateTimeService>();

            await context.Response.WriteAsync($"Текущее время: {service.GetTime()}");
        });
    });

    appBuilder.Map("/date", dateBuilder =>
    {
        dateBuilder.Run(async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            var service = context.RequestServices.GetRequiredService<IDateTimeService>();

            await context.Response.WriteAsync($"Дата: {service.GetDate()}");
        });
    });

    appBuilder.Map("/info", infoBuilder =>
    {
        infoBuilder.Run(async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            await context.Response.WriteAsync("Информация о сервере StudentPortal.Diagnostics");
        });
    });
});

// защищённая ветка
app.Map("/secure", appBuilder =>
{
    appBuilder.UseToken("study2026");

    appBuilder.Map("/report", report =>
    {
        report.Run(async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";

            await context.Response.WriteAsync("Secure report");
        });
    });

    appBuilder.Map("/admin", admin =>
    {
        admin.Map("/report", adminReport =>
        {
            adminReport.Run(async context =>
            {
                context.Response.ContentType = "text/plain; charset=utf-8";

                await context.Response.WriteAsync("Admin secure report");
            });
        });
    });
});

// env
app.MapGet("/env", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    var env = app.Environment;

    var sb = new StringBuilder();

    sb.AppendLine("<html><body>");
    sb.AppendLine("<h2>Environment Info</h2>");

    sb.AppendLine($"Environment: {env.EnvironmentName}<br>");
    sb.AppendLine($"Application: {env.ApplicationName}<br>");
    sb.AppendLine($"ContentRootPath: {env.ContentRootPath}<br>");
    sb.AppendLine($"WebRootPath: {env.WebRootPath}<br>");

    if (env.IsDevelopment())
    {
        sb.AppendLine("<p style='color:green'>Запущено в режиме Development</p>");
    }
    else
    {
        sb.AppendLine("<p style='color:red'>Запущено в режиме Production</p>");
    }

    sb.AppendLine("</body></html>");

    await context.Response.WriteAsync(sb.ToString());
});

// вывод сервисов
app.MapGet("/di/services", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    var services = builder.Services;

    var sb = new StringBuilder();

    sb.AppendLine("<html>");
    sb.AppendLine("<body>");
    sb.AppendLine("<h2>Registered Services</h2>");

    sb.AppendLine("<table border='1' cellpadding='5'>");
    sb.AppendLine("<tr>");
    sb.AppendLine("<th>ServiceType</th>");
    sb.AppendLine("<th>Lifetime</th>");
    sb.AppendLine("<th>ImplementationType</th>");
    sb.AppendLine("</tr>");

    foreach (var serv in services.Take(20))
    {
        sb.AppendLine("<tr>");
        sb.AppendLine($"<td>{serv.ServiceType.Name}</td>");
        sb.AppendLine($"<td>{serv.Lifetime}</td>");
        sb.AppendLine($"<td>{serv.ImplementationType}</td>");
        sb.AppendLine("</tr>");
    }

    sb.AppendLine("</table>");
    sb.AppendLine("</body>");
    sb.AppendLine("</html>");

    await context.Response.WriteAsync(sb.ToString());
});

// стартовая страница
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/plain; charset=utf-8";

    var sb = new StringBuilder();

    sb.AppendLine("StudentPortal.Diagnostics");
    sb.AppendLine();
    sb.AppendLine("Доступные маршруты:");
    sb.AppendLine("/tools/time");
    sb.AppendLine("/tools/date");
    sb.AppendLine("/tools/info");
    sb.AppendLine("/secure/report?token=study2026");
    sb.AppendLine("/secure/admin/report?token=study2026");
    sb.AppendLine("/env");
    sb.AppendLine("/di/services");
    sb.AppendLine("/swagger");

    await context.Response.WriteAsync(sb.ToString());
});

app.Run();