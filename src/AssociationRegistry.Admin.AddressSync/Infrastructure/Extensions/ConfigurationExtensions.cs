namespace AssociationRegistry.Admin.AddressSync.Infrastructure.Extensions;

using ConfigurationBindings;
using Framework;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static PostgreSqlOptions GetPostgreSqlOptions(this IConfiguration configuration)
    {
        var postgreSqlOptions = configuration
                               .GetSection(PostgreSqlOptions.SectionName)
                               .Get<PostgreSqlOptions>();

        ThrowIfInvalid();

        return postgreSqlOptions!;

        void ThrowIfInvalid()
        {
            if (postgreSqlOptions == null)
                throw new ArgumentNullException(nameof(postgreSqlOptions));

            const string sectionName = nameof(PostgreSqlOptions);

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Database, $"{sectionName}.{nameof(PostgreSqlOptions.Database)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Host, $"{sectionName}.{nameof(PostgreSqlOptions.Host)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Username, $"{sectionName}.{nameof(PostgreSqlOptions.Username)}");

            Throw<ArgumentNullException>
               .IfNullOrWhiteSpace(postgreSqlOptions.Password, $"{sectionName}.{nameof(PostgreSqlOptions.Password)}");
        }
    }
}
