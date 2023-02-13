namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using System;
using ConfigurationBindings;
using Framework;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptionsSection GetPostgreSqlOptionsSection(this IConfiguration configuration)
    {
        var postgreSqlOptionsSection = configuration
            .GetSection(PostgreSqlOptionsSection.Name)
            .Get<PostgreSqlOptionsSection>();
        postgreSqlOptionsSection.ThrowIfInvalid();
        return postgreSqlOptionsSection;
    }

    private static void ThrowIfInvalid(this PostgreSqlOptionsSection postgreSqlOptions)
    {
        const string sectionName = nameof(PostgreSqlOptionsSection);
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Database)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Host)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Username)}");
        Throw<ArgumentNullException>
            .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptionsSection.Password)}");
    }

    public static string GetBaseUrl(this IConfiguration configuration)
        => configuration.GetValue<string>("BaseUrl").TrimEnd('/');
}
