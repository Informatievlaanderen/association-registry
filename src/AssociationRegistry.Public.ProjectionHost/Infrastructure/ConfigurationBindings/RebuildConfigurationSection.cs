namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.ConfigurationBindings;

public class RebuildConfigurationSection
{
    public const string SectionName = "RebuildConfiguration";
    public int TimeoutInMinutes { get; set; } = 60 * 2;
}
