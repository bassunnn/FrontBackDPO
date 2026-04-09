using Pr1.Services;
using Pr1.Models;

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<TaskStorage>();

var app = builder.Build();


app.MapGet("/api/tasks", async (TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    return Results.Ok(tasks);
});

app.MapGet("/api/tasks/{id:int}", async (int id, TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

app.MapPost("/api/tasks", async (TaskCreateRequest request, TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    var newId = tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1;

    var newTask = new TaskModel
    {
        Id = newId,
        Title = request.Title,
        Description = request.Description,
        IsCompleted = false
    };

    tasks.Add(newTask);
    await storage.SaveTasksAsync(tasks);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);

});

app.MapPut("api/tasks/{id:int}", async (int id, TaskUpdateRequest request, TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();

    task.Title = request.Title;
    task.Description = request.Description;
    task.IsCompleted = request.IsCompleted;

    await storage.SaveTasksAsync(tasks);
    return Results.Ok(task);
});

app.MapDelete("/api/tasks/{id:int}", async (int id, TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();

    tasks.Remove(task);
    await storage.SaveTasksAsync(tasks);
    return Results.NoContent();
});

app.MapGet("/api/tasks/completed", async (TaskStorage storage) =>
{
    var tasks = await storage.LoadTasksAsync();
    var completedTasks = tasks.Where(t => t.IsCompleted = true).ToList();
    return Results.Ok(completedTasks);
});


app.Run();

public record TaskCreateRequest(string Title, string Description);
public record TaskUpdateRequest(string Title, string Description, bool IsCompleted);