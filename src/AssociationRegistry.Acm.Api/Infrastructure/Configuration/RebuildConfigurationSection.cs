namespace AssociationRegistry.Acm.Api.Infrastructure.Configuration;

public class RebuildConfigurationSection
{
    public const string SectionName = "RebuildConfiguration";
    public int TimeoutInSeconds { get; set; } = 60 * 5;
}
