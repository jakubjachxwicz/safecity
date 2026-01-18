using SafeCityAPI.Models;

namespace SafeCityAPI.DTOs;

public class UpdateReportRequest
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public ReportCategory? Category { get; set; }
    public string? Description { get; set; }
}