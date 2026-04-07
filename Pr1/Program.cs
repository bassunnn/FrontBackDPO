using MinApi.Services;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<TaskStorage>();


var app = builder.Build();


app.MapGet("/api/tasks", async (TaskStorage storage) => {
    var tasks = await storage.LoadTasksAsync();
    return Results.Ok(tasks);
});

app.Run();