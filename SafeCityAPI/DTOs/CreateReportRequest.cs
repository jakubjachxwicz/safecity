namespace SafeCityAPI.DTOs;

public class CreateReportRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }
}