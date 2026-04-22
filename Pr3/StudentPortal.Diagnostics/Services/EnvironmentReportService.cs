using System.Text;

namespace Services;
public class EnvironmentReportService : IEnvironmentReportService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    
    public EnvironmentReportService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }
    
    public string GetEnvironmentInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Diagnostic Information ===\n");
        sb.AppendLine($"Application: {_environment.ApplicationName}");
        sb.AppendLine($"Environment: {_environment.EnvironmentName}");
        sb.AppendLine($"Content Root: {_environment.ContentRootPath}");
        sb.AppendLine($"Current Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"OS: {Environment.OSVersion}");
        sb.AppendLine($"Machine Name: {Environment.MachineName}");
        sb.AppendLine($"Processor Count: {Environment.ProcessorCount}");
        sb.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
        
        return sb.ToString();
    }

    public string GetEnviromentName()
    {
        return _environment.EnvironmentName;
    }
}