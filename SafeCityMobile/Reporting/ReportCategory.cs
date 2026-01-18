namespace SafeCityMobile.Reporting;

public class MappedReportCategory
{
    public required ReportCategory Category { get; set; }
    public required string Text { get; set; }


    public static List<MappedReportCategory> GetReportCategories()
    {
        return new List<MappedReportCategory>()
        {
            new() {Category = ReportCategory.Traffic, Text = "Utrudnienie na drodze" },
            new() {Category = ReportCategory.Trash, Text = "Składowisko śmieci" },
            new() {Category = ReportCategory.Fight, Text = "Bójka" },
            new() {Category = ReportCategory.IllegalParking, Text = "Źle zaparkowany samochód" },
            new() {Category = ReportCategory.IllegalGathering, Text = "Nielegalne zgromadzenie" },
            new() {Category = ReportCategory.Drone, Text = "Dron" },
            new() {Category = ReportCategory.Other, Text = "Inne" }
        };
    }
}

public enum ReportCategory
{
    Traffic,
    Trash,
    Fight,
    IllegalParking,
    IllegalGathering,
    Drone,
    Other,
}