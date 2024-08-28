namespace AssociationRegistry.PowerBi.ExportHost.Configuration;

public class PowerBiExportOptionsSection
{
    public const string SectionName = "PowerBiExport";

    public string BucketName { get; set; }
    public bool Demo { get; set; }
}
