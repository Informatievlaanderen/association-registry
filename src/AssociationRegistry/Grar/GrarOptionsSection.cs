namespace AssociationRegistry.Grar;

public class GrarOptionsSection
{
    public const string SectionName = "GrarOptions";
    public int Timeout { get; set; } = 30;
    public string BaseUrl { get; set; }
}
