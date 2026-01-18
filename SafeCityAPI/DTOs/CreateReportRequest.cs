using SafeCityAPI.Models;

namespace SafeCityAPI.DTOs;

public class CreateReportRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ReportCategory Category { get; set; } = ReportCategory.Other;
    public string? Description { get; set; }
}