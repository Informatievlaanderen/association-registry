namespace AssociationRegistry.PowerBi.ExportHost.Infrastructure.Extensions;

using Configuration;
using Framework;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{

    public static PowerBiExportOptionsSection GetPowerBiExportOptions(this IConfiguration configuration)
    {
        var powerBiExportOptions = configuration
                                  .GetSection(PowerBiExportOptionsSection.SectionName)
                                  .Get<PowerBiExportOptionsSection>();

        powerBiExportOptions.ThrowIfInvalid();

        return powerBiExportOptions!;
    }

    private static void ThrowIfInvalid(this PowerBiExportOptionsSection? powerBiExportOptions)
    {
        if (powerBiExportOptions == null)
            throw new ArgumentNullException(nameof(powerBiExportOptions));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(powerBiExportOptions.BucketName, $"{PowerBiExportOptionsSection.SectionName}.{nameof(PowerBiExportOptionsSection.BucketName)}");
    }
}
