namespace Services;

public static class ServiceCollectionExtensions
{
    public static void AddStudentPortalServices(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddTransient<IEnvironmentReportService, EnvironmentReportService>();
    }
}