namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

public class DubbelImportOptions
{
    public string SourceBucket { get; set; } = null!;

    public string ReportBucket { get; set; } = null!;

    public bool UseLocalStack { get; set; }
}
