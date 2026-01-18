namespace SafeCityMobile.Reporting;

public class ReportRequestDto
{
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public string? Description { get; set; } = string.Empty;
}
