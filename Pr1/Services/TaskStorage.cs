using System.Text.Json;
using MinApi.Models;

namespace MinApi.Services;

public class TaskStorage
{
    private readonly string  _filePath = Path.Combine("Data", "tasks.json");

    public async Task<List<TaskModel>> LoadTasksAsync()
    {
        if (!File.Exists(_filePath))
            return new List<TaskModel>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new List<TaskModel>();
    }

    public async Task SaveTasksAsync(List<TaskModel> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions {WriteIndented = true});
        await File.WriteAllTextAsync(_filePath, json);
    }
}