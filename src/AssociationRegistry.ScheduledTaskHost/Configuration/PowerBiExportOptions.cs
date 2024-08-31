namespace AssociationRegistry.ScheduledTaskHost.Configuration;

public record PowerBiExportOptions(string BucketName, bool Demo, string CronExpression)
{
    public const string SectionName = "PowerBiExport";
}
