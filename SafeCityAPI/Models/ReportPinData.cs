namespace SafeCityAPI.Models;

// Report data meant to be sent to client - cannot contain sensitive info such as IP
public class ReportPinData
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double HoursElapsed { get; set; }
    public ReportType ReportType { get; set; }
}

public enum ReportType
{
    Anonymous = 0,
    LoggedIn = 1
}