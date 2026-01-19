namespace SafeCityMobile.Reporting;

public class FormattedReport : Report
{
    public FormattedReport(Report report)
    {
        Id = report.Id;
        ReportedAt = report.ReportedAt;
        Latitude = report.Latitude;
        Longitude = report.Longitude;
        Category = report.Category;
        Message = report.Message;
        UserId = report.UserId;
        IpAddress = report.IpAddress;
    }

    public string FormattedCategory =>
        MappedReportCategory.GetReportCategories()
        .Single(c => c.Category == Category).Text;
}
