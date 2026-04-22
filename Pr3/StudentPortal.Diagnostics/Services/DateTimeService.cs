namespace Services;

public class DateTimeService : IDateTimeService
{
    public string GetDate() => DateTime.Now.ToLongTimeString();
    public string GetTime() => DateTime.Now.ToShortDateString();
}