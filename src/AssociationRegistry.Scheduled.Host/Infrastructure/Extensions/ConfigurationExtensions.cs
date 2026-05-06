namespace AssociationRegistry.Scheduled.Host.Infrastructure.Extensions;

using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Bewaartermijnen;
using Microsoft.Extensions.Configuration;
using PowerBi;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetPostgreSqlOptions(this IConfiguration configuration)
    {
        var postgreSqlOptions = configuration
            .GetSection(PostgreSqlOptionsSection.SectionName)
            .Get<PostgreSqlOptionsSection>();

        ThrowIfInvalid();

        return postgreSqlOptions!;

        void ThrowIfInvalid()
        {
            if (postgreSqlOptions == null)
                throw new ArgumentNullException(nameof(postgreSqlOptions));

            const string sectionName = PostgreSqlOptionsSection.SectionName;

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(
                postgreSqlOptions.Database,
                $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}"
            );

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(
                postgreSqlOptions.Host,
                $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}"
            );

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(
                postgreSqlOptions.Username,
                $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}"
            );

            Throw<ArgumentNullException>.IfNullOrWhiteSpace(
                postgreSqlOptions.Password,
                $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}"
            );
        }
    }

    public static BewaartermijnOptions GetBewaartermijnenOptions(this IConfiguration configuration)
    {
        var bewaartermijnOptions = configuration.GetSection(nameof(BewaartermijnOptions)).Get<BewaartermijnOptions>();

        if (bewaartermijnOptions == null)
            throw new ArgumentNullException(nameof(bewaartermijnOptions));

        bewaartermijnOptions.ThrowIfInValid();

        return bewaartermijnOptions;
    }

    public static PowerBiExportOptions GetPowerBiExportOptions(this IConfiguration configuration)
    {
        var powerBiExportOptions = configuration
                                  .GetSection(nameof(PowerBiExportOptions))
                                  .Get<PowerBiExportOptions>();

        powerBiExportOptions.ThrowIfInvalid();

        return powerBiExportOptions!;
    }

    private static void ThrowIfInvalid(this PowerBiExportOptions? powerBiExportOptions)
    {
        if (powerBiExportOptions == null)
            throw new ArgumentNullException(nameof(powerBiExportOptions));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(powerBiExportOptions.BucketName, $"{nameof(powerBiExportOptions)}.{nameof(PowerBiExportOptions.BucketName)}");
    }

    private static void ThrowIfInValid(this BewaartermijnOptions opt)
    {
        Throw<ArgumentNullException>.IfNull(opt.SlackWebhook, nameof(opt.SlackWebhook));
    }
}
