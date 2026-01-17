namespace SafeCityAPI.DTOs;

public class AdminStatsResponse
{
    public int TotalReports { get; set; }
    public int ReportsLast24h { get; set; }
    public int FalseReports { get; set; }
    public int VerifiedReports { get; set; }
    public List<TopAreaStat> TopAreas { get; set; } = new();
    public List<HourlyStat> ReportsByHour { get; set; } = new();
}

public class TopAreaStat
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class HourlyStat
{
    public int Hour { get; set; }
    public int Count { get; set; }
}