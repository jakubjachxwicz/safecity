namespace SafeCityAPI.DTOs;

public class ReportResponse
{
    public Guid Id { get; set; }
    public DateTime ReportedAt { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Message { get; set; }
    public Guid? UserId { get; set; }  // Nullable 
    public string IpAddress { get; set; } = string.Empty;
}