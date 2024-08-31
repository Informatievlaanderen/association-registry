namespace AssociationRegistry.ScheduledTaskHost.Infrastructure.Extensions;

using AssociationRegistry.Framework;
using Hosts.Configuration.ConfigurationBindings;
using Invocables;
using Microsoft.Extensions.Configuration;

internal static class ConfigurationExtensions
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

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}");
        }
    }

    public static AddressSynchronisationOptions GetAddressSynchronisationOptions(this IConfiguration configuration)
    {
        var addressSyncOptions = configuration
                         .GetSection(AddressSynchronisationOptions.SectionName)
                         .Get<AddressSynchronisationOptions>();

        if (addressSyncOptions == null)
            throw new ArgumentNullException(nameof(addressSyncOptions));

        addressSyncOptions.ThrowIfInValid();

        return addressSyncOptions;
    }

    private static void ThrowIfInValid(this AddressSynchronisationOptions opt)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.BaseUrl, nameof(opt.BaseUrl));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.ApiKey, nameof(opt.ApiKey));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.SlackWebhook, nameof(opt.SlackWebhook));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.CronExpression, nameof(opt.CronExpression));
    }

    public static PowerBiExportOptions GetPowerBiExportOptions(this IConfiguration configuration)
    {
        var powerBiExportOptions = configuration
                                .GetSection(PowerBiExportOptions.SectionName)
                                .Get<PowerBiExportOptions>();

        if (powerBiExportOptions == null)
            throw new ArgumentNullException(nameof(powerBiExportOptions));

        powerBiExportOptions.ThrowIfInValid();

        return powerBiExportOptions;
    }

    private static void ThrowIfInValid(this PowerBiExportOptions opt)
    {
        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.BucketName, nameof(opt.BucketName));

        Throw<ArgumentNullException>
           .IfNullOrWhiteSpace(opt.CronExpression, nameof(opt.CronExpression));
    }
}
