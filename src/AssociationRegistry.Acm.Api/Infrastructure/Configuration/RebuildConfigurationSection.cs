namespace AssociationRegistry.Acm.Api.Infrastructure.Configuration;

public class RebuildConfigurationSection
{
    public const string SectionName = "RebuildConfiguration";
    public int TimeoutInMinutes { get; set; } = 60 * 2;
}
