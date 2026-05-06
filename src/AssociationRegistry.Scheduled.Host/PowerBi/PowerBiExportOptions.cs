namespace AssociationRegistry.Scheduled.Host.PowerBi;

public class PowerBiExportOptions
{
    public string BucketName { get; set; }

    public string Cron { get; set; } = "0 0 3 * * ?";
}
