namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Extensions;

using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.Configuration;

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

    public static AddressSyncOptions GetAddressSyncOptions(this IConfiguration configuration)
    {
        var addressSyncOptions = configuration.GetSection(nameof(AddressSyncOptions)).Get<AddressSyncOptions>();

        if (addressSyncOptions == null)
            throw new ArgumentNullException(nameof(addressSyncOptions));

        addressSyncOptions.ThrowIfInValid();

        return addressSyncOptions;
    }

    private static void ThrowIfInValid(this AddressSyncOptions opt)
    {
        Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.BaseUrl, nameof(opt.BaseUrl));

        Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.ApiKey, nameof(opt.ApiKey));

        Throw<ArgumentNullException>.IfNullOrWhiteSpace(opt.SlackWebhook, nameof(opt.SlackWebhook));
    }
}
